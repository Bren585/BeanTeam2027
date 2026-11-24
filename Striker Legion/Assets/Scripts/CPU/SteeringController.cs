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
	Wander,
}

// キャラクターのステアリング制御を行うクラス
public class SteeringController : MonoBehaviour
{
	private const float MoveWeight = 0.6f;
	private const float AvoidWeight = 0.4f;
	private const float RayLength = 10.0f;
	private const float SphereRadius = 3.0f;

	// ワンダリング（徘徊）用
	private float wanderDistance = 6.0f; // 前方円の中心までの距離
	private float wanderRadius = 3.0f;   // 円の半径
	private float wanderJitter = 1.5f;   // 目標点の揺らぎ強さ
	private bool wanderWithAvoid = true;  // 徘徊中に回避を有効にするか
	private Vector3 wanderOrigin;               // 徘徊の基準点
	private bool setOriginOnWanderStart = true; // 徘徊開始時に現在地を基準点にするか
	private float wanderMaxRadius = 15.0f;      // 基準点から離れられる最大半径
	private float returnToOriginWeight = 1.5f;  // 範囲外に出たときの引き戻し強さ


	private Vector3 wanderTargetOffset = Vector3.forward * 3.0f;
	private bool hasWanderInitialized = false;

	private SteeringMode steeringMode = SteeringMode.Avoid;

	// 目標位置
	private Vector3 targetPosition;
    public Vector3 TargetPosition{
        get { return targetPosition; }
        set { targetPosition = value; }
    }

	// 速度
	[SerializeField]private Vector3 velocity;
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

	// 外部（AIなど）から基準点を明示的に指定したい場合
	public void SetWanderOrigin(Vector3 origin, bool autoUpdateOrigin = false)
	{
		wanderOrigin = origin;
		setOriginOnWanderStart = autoUpdateOrigin;
	}
	// 移動開始用
	public void StartMove(SteeringMode mode) {
		//Debug.Log(name+":ステアリング開始");
		isMove = true;
		steeringMode = mode;

		if (mode == SteeringMode.Wander)
		{
			if (setOriginOnWanderStart)
			{
				wanderOrigin = transform.position;
			}
			InitWanderOffset();
		}
	}
	public void CancelMove()	{
		//Debug.Log(name + ":ステアリング終了");
		isMove = false;
		velocity = Vector3.zero;
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

		// ワンダリング時は毎フレーム徘徊方向を計算して更新
		if (steeringMode == SteeringMode.Wander)
		{
			Vector3 wanderDirection = CalculateWanderDirection();
			UpdateVelocity(wanderDirection);
			return;
		}

		// ターゲットまでの移動ベクトル
		Vector3 moveDirection = targetPosition - transform.position;
		//Vector3 MoveDirection = Vector3.Normalize(TargetPosition - transform.position);
		moveDirection.y = 0.0f;

		// 規定の距離まで移動したら、移動終了
		if (moveDirection.magnitude < minMagnitude)
        {

			CancelMove();

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

			case SteeringMode.Wander:
				if (wanderWithAvoid)
				{
					Vector3 wanderAvoidVel = AvoidOpponentVector(direction.normalized);
					velocity = SeekVector(wanderAvoidVel);
				}
				else
				{
					velocity = SeekVector(direction);
				}
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

	private void InitWanderOffset()
	{
		float angle = Random.Range(0f, Mathf.PI * 2f);
		wanderTargetOffset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * wanderRadius;
		hasWanderInitialized = true;
	}
	private Vector3 CalculateWanderDirection()
	{
		if (!hasWanderInitialized)
		{
			InitWanderOffset();
		}

		// 現在の進行方向（止まっている場合は transform.forward）
		Vector3 forward = velocity.sqrMagnitude > 0.01f
			? new Vector3(velocity.x, 0f, velocity.z).normalized
			: new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;

		if (forward.sqrMagnitude < 0.001f)
		{
			forward = Vector3.forward;
		}

		// ランダムな微小ベクトルを加えて円周上に拘束（ジッター）
		Vector3 jitter = new Vector3(
			Random.Range(-1f, 1f),
			0f,
			Random.Range(-1f, 1f)
		) * (wanderJitter * Time.deltaTime * 10f);

		wanderTargetOffset += jitter;
		wanderTargetOffset.y = 0f;
		wanderTargetOffset = wanderTargetOffset.normalized * wanderRadius;

		// 前方円の中心 + 円周上のオフセット
		Vector3 circleCenter = forward * wanderDistance;
		Vector3 wanderDirection = (circleCenter + wanderTargetOffset).normalized;
		wanderDirection.y = 0f;

		// 基準点からの距離を確認し、許容半径を超えていたら基準点方向へ引き戻す
		if (wanderMaxRadius > 0f)
		{
			Vector3 toOrigin = wanderOrigin - transform.position;
			toOrigin.y = 0f;
			float distFromOrigin = toOrigin.magnitude;

			if (distFromOrigin > wanderMaxRadius && distFromOrigin > 0.001f)
			{
				// 超過割合に応じて引き戻しを強める（0〜1以上にクランプ）
				float excessRatio = Mathf.Clamp01((distFromOrigin - wanderMaxRadius) / wanderMaxRadius);
				Vector3 pullDirection = toOrigin / distFromOrigin;

				wanderDirection = (wanderDirection + pullDirection * (returnToOriginWeight * (0.5f + excessRatio))).normalized;
			}
		}

		targetPosition = transform.position + wanderDirection * wanderDistance;
		return wanderDirection;
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
		if (targetPosition != Vector3.zero)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(targetPosition, 1f);
		}

		// キャラクターの位置から移動ベクトルを線で描画
		//Gizmos.color = Color.yellow;
		//Vector3 from = transform.position + Vector3.up * 2.0f;
		//Gizmos.DrawLine(from, from + velocity.normalized * 10.0f);

	}
}
