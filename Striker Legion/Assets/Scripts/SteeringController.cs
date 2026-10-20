using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public enum SteeringMode
{
	Simple,
	Seek,
	Arrive,
	Avoid,
}

// キャラクターのステアリング制御を行うクラス
public class SteeringController : MonoBehaviour
{
	private const float MoveWeight = 0.5f;
	private const float AvoidWeight = 0.5f;
	private const float RayLength = 10.0f;
	private const float SphereRadius = 3.0f;

	private SteeringMode steeringMode = SteeringMode.Avoid;

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
	private const float MaxSpeed = 10.0f;

	// 移動を許容する距離
	private const float AcceptableDistance = 1.0f;

    // 移動するかのフラグ
    bool isMove = false;
    public bool IsMove => isMove;

    // 移動開始用
    public void StartMove(SteeringMode mode) {
		isMove = true;
		steeringMode = mode;
    }
	public void CancelMove()	{
		isMove = false;
	}


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {

	}

    // 外部呼び出し用
    public void UpdateVelocity()
    {
        if (!isMove)
            return;

        // ターゲットまでの移動ベクトル
		Vector3 moveDirection = targetPosition - transform.position;
		//Vector3 MoveDirection = Vector3.Normalize(TargetPosition - transform.position);
		moveDirection.y = 0.0f;

		// 規定の距離まで移動したら、移動終了
		if (moveDirection.magnitude < minMagnitude)
        {
            //Debug.Log("ステアリング終了");
            isMove = false;
        }

		// 移動処理
		UpdateVelocity(moveDirection);
	}

	void UpdateVelocity(Vector3 direction)
	{
		
		switch (steeringMode)
		{
			case SteeringMode.Simple:
				velocity = direction;
			break;

			case SteeringMode.Seek:
				velocity = SeekVector(direction);
			break;

			case SteeringMode.Arrive:
				velocity = ArriveVector(direction, direction.magnitude);
			break;

			case SteeringMode.Avoid:
				Vector3 avoidVelocity = AvoidOpponentVector(direction.normalized);
				velocity = SeekVector(avoidVelocity);
			break;
		}
	}

    // 敵対キャラクターを避ける
    Vector3 AvoidOpponentVector(Vector3 moveDirection)
    {
		// 回避ベクトルを取得(長さは本来、移動速度の最大値)
		Vector3 hitVector = Vector3.Normalize(SphereCastHitVector(transform.position, moveDirection.normalized, SphereRadius, RayLength));
		hitVector.y = 0.0f;

        // 移動ベクトルと回避ベクトルを合成
        Vector3 direction = Vector3.Normalize(moveDirection.normalized * MoveWeight + hitVector * AvoidWeight);
        
        // 速度計算
        return direction * (MaxSpeed);
        //return direction * (MaxSpeed) * Time.deltaTime;
	}

	// 回避挙動後にターゲットの座標を変更する
	Vector3 FixedAvoidTargetPosition(Vector3 avoidVelocity, float beforeMagnitude)
	{
		return transform.position + avoidVelocity.normalized * beforeMagnitude;
	}

	Vector3 SeekVector(Vector3 moveDirection)
	{
		Vector3 needVector = moveDirection.normalized * MaxSpeed;

		return needVector - parentVelocity;
	}

	Vector3 ArriveVector(Vector3 moveDirection, float magnitude)
	{
		const float SplitSpeed = 1.1f;

		// 負の値が出ないようにする
		float safeMagnitude = Mathf.Max(0f, magnitude);
		if (safeMagnitude < minMagnitude)
		{
			isMove = false;
			return Vector3.zero;
		}
		float splitMagnitude = safeMagnitude / SplitSpeed;

		// 今回使用する係数
		float speed = (splitMagnitude > MaxSpeed) ? MaxSpeed : splitMagnitude;

		// 係数をかけて、遅すぎるのを緩和する
		float factor = 2.0f;

		// 単位ベクトルに補正して速度を掛ける
		Vector3 velocity = moveDirection.normalized * speed * factor;

		return velocity - parentVelocity;
	}

    Vector3 SphereCastHitVector(Vector3 position, Vector3 direction, float radius, float length)
    {
        // スフィアの元になるレイ
		Ray ray = new Ray(position, direction);

        // 対象ノレイヤー
		int layer = LayerMask.GetMask("Player", "Enemy");

		// ヒットしなかったらそのまま進んでほしいので、0.0fを返す
		if (!Physics.SphereCast(ray, radius, out RaycastHit hitInfo, length, layer))
			return Vector3.zero;
	
        // 自分自身に当たらないようにする
        if (hitInfo.transform == gameObject.transform)
            return Vector3.zero;
		// タグが別であれば、対象外とする
		if (hitInfo.collider.tag != gameObject.tag)
		    return Vector3.zero;

		//Debug.Log("ヒットした法線:" + hitInfo.normal + "名前:" + hitInfo.collider.name);

		// 法線で返す
		return hitInfo.normal;
	}

	// インスペクターでこのオブジェクト（キャラクター）が選択されている時だけ呼ばれる
	private void OnDrawGizmosSelected()
	{
		// 移動方向の向き（目標位置がない場合は前方）
		//Vector3 dir = (targetPosition != Vector3.zero)
		//	? (targetPosition - transform.position).normalized
		//	: transform.forward;

		//// ギズモの色を設定（半透明の黄色）
		//Gizmos.color = new Color(1.0f, 0.92f, 0.015f, 0.5f);

		//// 3. 目的地の位置に小さな球を描画
		//if (targetPosition != Vector3.zero)
		//{
		//	Gizmos.color = Color.green;
		//	Gizmos.DrawSphere(targetPosition, 1f);
		//}
	}
}
