using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public enum SteeringMode
{
	Seek,
	Arrive,
	Avoid,
}

// キャラクターのステアリング制御を行うクラス
public class SteeringController : MonoBehaviour
{
	private const float MoveWeight = 0.6f;
	private const float AvoidWeight = 0.4f;
	private const float RayLength = 8.0f;
	private const float SphereRadius = 1.0f;

	private SteeringMode sterringMode = SteeringMode.Avoid;

	// 目標位置
	private Vector3 targetPosition;
    public Vector3 TargetPosition{
        get { return targetPosition; }
        set { targetPosition = value; }
    }

	// 速度
	private Vector3 velocity;
	public Vector3 Velocity => velocity;

	// 親の速度
	[SerializeField] private Vector3 parentVelocity;
	[SerializeField] public float minMagnitude = 1.0f;
	public void SetParentVelocity(Vector3 v) => parentVelocity = v;

	// 最大速度
	private const float MaxSpeed = 20.0f;

	// 移動を許容する距離
	private const float AcceptableDistance = 1.0f;

    // 移動するかのフラグ
    bool isMove = false;
    public bool IsMove => isMove;

    // 移動開始用
    public void StartMove(SteeringMode mode) {
		isMove = true;
		sterringMode = mode;
    }
	public void CancelMove()	{
		isMove = false;
	}


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {

	}

    // Update is called once per frame
    void Update()
    {
        if (!isMove)
            return;

        // ターゲットまでの移動ベクトル
		Vector3 MoveDirection = TargetPosition - transform.position;
		//Vector3 MoveDirection = Vector3.Normalize(TargetPosition - transform.position);
		MoveDirection.y = 0.0f;

		// 規定の距離まで移動したら、移動終了
		if (MoveDirection.magnitude < minMagnitude)
        {
            //Debug.Log("ステアリング終了");
            isMove = false;
        }

		// 移動処理
		//velocity = AvoidOpponentVector(MoveDirection.normalized);
		//transform.position += AvoidOpponentVector(MoveDirection);
		UpdateVelocity(MoveDirection);
	}

	void UpdateVelocity(Vector3 Direction)
	{
		switch (sterringMode)
		{
			case SteeringMode.Seek:
				velocity = SeekVector(Direction);
			break;

			case SteeringMode.Arrive:
				velocity = ArriveVector(Direction, Direction.magnitude);
			break;

			case SteeringMode.Avoid:
				velocity = AvoidOpponentVector(Direction.normalized);
			break;
		}
	}

    // 敵対キャラクターを避ける
    Vector3 AvoidOpponentVector(Vector3 MoveDirection)
    {
		// 回避ベクトルを取得(長さは本来、移動速度の最大値)
		Vector3 HitVector = Vector3.Normalize(SphereCastHitVector(transform.position, MoveDirection, SphereRadius, RayLength));
		HitVector.y = 0.0f;

        // 移動ベクトルと回避ベクトルを合成
        Vector3 Direction = Vector3.Normalize(MoveDirection * MoveWeight + HitVector * AvoidWeight);
        
        // 速度計算
        return Direction * (MaxSpeed) * Time.deltaTime;
	}

	Vector3 SeekVector(Vector3 MoveDirection)
	{
		Vector3 NeedVector = MoveDirection.normalized * MaxSpeed;

		return NeedVector - parentVelocity;
	}

	Vector3 ArriveVector(Vector3 moveDirection, float magnitude)
	{
		const float SplitSpeed = 2.0f;


		// 負の値が出ないようにする
		float safeMagnitude = Mathf.Max(0f, magnitude);
		if (safeMagnitude < minMagnitude)
			return Vector3.zero;
		float splitMagnitude = safeMagnitude / SplitSpeed;

		// 今回使用する係数
		float speed = (splitMagnitude > MaxSpeed) ? MaxSpeed : splitMagnitude;

		// 単位ベクトルに補正して速度を掛ける
		Vector3 velocity = moveDirection.normalized * speed;

		return velocity - parentVelocity;
	}

    Vector3 SphereCastHitVector(Vector3 Position, Vector3 Direction, float Radius, float Length)
    {
        // スフィアの元になるレイ
		Ray ray = new Ray(Position, Direction);

        // 対象ノレイヤー
		int Layer = LayerMask.GetMask("Player", "Enemy");

		// ヒットしなかったらそのまま進んでほしいので、0.0fを返す
		if (!Physics.SphereCast(ray, Radius, out RaycastHit hitInfo, Length, Layer))
			return Vector3.zero;
	
        // 自分自身に当たらないようにする
        if (hitInfo.transform == gameObject.transform)
            return Vector3.zero;
		// タグが別であれば、対象外とする
		if (hitInfo.collider.tag != gameObject.tag)
		    return Vector3.zero;


		// 法線で返す
		return hitInfo.normal;
	}

	// インスペクターでこのオブジェクト（キャラクター）が選択されている時だけ呼ばれる
	private void OnDrawGizmosSelected()
	{
		// 移動方向の向き（目標位置がない場合は前方）
		Vector3 dir = (TargetPosition != Vector3.zero)
			? (TargetPosition - transform.position).normalized
			: transform.forward;

		// ギズモの色を設定（半透明の黄色）
		Gizmos.color = new Color(1.0f, 0.92f, 0.015f, 0.5f);

		// 1. SphereCast の進行方向を表すレイを描画
		//Gizmos.DrawRay(transform.position, dir * RayLength);

		// 2. SphereCast の終点位置に球体を描画（判定の大きさを確認）
		//Gizmos.DrawWireSphere(transform.position + dir * RayLength, SphereRadius);

		// 3. 目的地の位置に小さな球を描画
		if (TargetPosition != Vector3.zero)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(TargetPosition, 1f);
		}
	}
}
