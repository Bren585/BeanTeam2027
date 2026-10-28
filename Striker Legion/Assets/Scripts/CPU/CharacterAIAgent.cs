using System.Collections.Generic;
using TMPro;
using Unity.AppUI.UI;
using Unity.Behavior;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterBase))]
[RequireComponent(typeof(SteeringController))]
[RequireComponent(typeof(BehaviorGraphAgent))]
[RequireComponent(typeof(NavMeshAgent))]
public class CharacterAIAgent : MonoBehaviour
{
    // ビヘイビアエージェント
    private BehaviorGraphAgent behaviorGraphAgent;

    // ナビメッシュエージェント
    private NavMeshAgent navMeshAgent;

    // キャラセンサー
    private CharacterSensor characterSensor;

    // キャラクターベース
    private CharacterBase owner;

	// ポジショニングの基準座標
	Dictionary<PositioningName, Vector3> positioningBase = new Dictionary<PositioningName, Vector3>();
	//private Vector3 positioningBase;

	// ステアリング制御コントローラ
	private SteeringController steeringController;

	// キャラのピッチ上のスコアを管理する
	private CharacterPitchScore pitchScore;

    // デバッグ用のテキスト表示
    public DebugText DebugText { get; private set; }

	// 色情報
	public Color BaseColor { get; private set; }
	
    public CharacterRole Role { get; private set; }

	// 一応、終点座標を保持できるようにしてある。ほとんどデバッグ用
	Vector3 endTargetPosition = Vector3.zero;

	//------------------------------ getter -----------------------------------
    public SteeringController SteeringController => steeringController;

    public CharacterSensor CharacterSensor => characterSensor;

	public Vector3 BasePosition(PositioningName name) => positioningBase[name];

	public Vector3 BestPosition => pitchScore.GetBestPosition();
	public Vector3 GetClosestPointOnBestGrid(Vector3 position) => pitchScore.GetClosestPointOnBestGrid(position);

	public bool IsUpdateScore => pitchScore.IsUpdate;

	// キャラクターがボールを持っているか
	public bool HasBall => owner.hasBall;
    // キャラのチーム番号
    public int TeamNumber => owner.teamNo;

    // パス関数（CharacterBaseから呼び出し）
    public void Pass() => owner.KickBall();

    // シュート関数（CharacterBaseから呼び出し）
    public void Shoot() => owner.KickBall();

    // パスが可能か
    public bool EnablePass() => owner.EnablePass();
    public CharacterBase PassTarget() => owner.PassTarget();

    public void Tackle() => owner.Tackle();

	// 追跡座標を設定
	public void SetTargetPosition(Vector3 position)
	{
		navMeshAgent.SetDestination(position);
		endTargetPosition = position;
	}

    protected void Awake()
    {
        // コンポーネントを取得
        owner = GetComponent<CharacterBase>();
        steeringController = GetComponent<SteeringController>();
		behaviorGraphAgent = GetComponent<BehaviorGraphAgent>();
		navMeshAgent = GetComponent<NavMeshAgent>();

		// デバッグテキストコンポーネントを追加
		DebugText = gameObject.AddComponent<DebugText>();
		characterSensor = gameObject.AddComponent<CharacterSensor>();
		pitchScore = gameObject.AddComponent<CharacterPitchScore>();

        navMeshAgent.updatePosition = true;
		navMeshAgent.isStopped = true;
	}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    public void Initialize(Dictionary<PositioningName, Vector3> positioningBase, CharacterRole role)
    {
        this.positioningBase = positioningBase;
        Role = role;
		DebugText.SetDefaultText(owner.name +" " + Role);
		pitchScore.Initialize(role, (CharacterTeam)(TeamNumber));
	}

	// Update is called once per frame
	void Update()
    {
		// 操舵レイヤーの更新処理
		//FindAndMoveToPosition();
		UpdateSteering(owner.velocity);
        // BlackBoard変数の更新処理
        UpdateBlackBoard();

       
	}

    void UpdateSteering(Vector3 ParentVelocity)
    {
		if (!steeringController.IsMove)
		{
			owner.Move(Vector3.zero);
			return;
		}
        // 親の速度を元に計算する
        steeringController.SetParentVelocity(ParentVelocity);

		// 操舵コントローラの目標座標をナビメッシュから取得
		steeringController.TargetPosition = navMeshAgent.steeringTarget;

		// 速力計算
		steeringController.UpdateVelocity();

		if (steeringController.Velocity.magnitude < 0.01f)
		{
			owner.Move(Vector3.zero);
			return;
		}
		//navMeshAgent.SetDestination(steeringController.CurrentPosition);
		//navMeshAgent.SetDestination(steeringController.TargetPosition);
        
		//navMeshAgent.velocity = steeringController.Velocity;
		//owner.Move((navMeshAgent.steeringTarget - transform.position).normalized * 1.0f);
		//owner.Move(navMeshAgent.desiredVelocity);
		//owner.Move(navMeshAgent.desiredVelocity);

        

		owner.Move(steeringController.Velocity);
		//owner.Move(steeringController.Velocity.normalized);
	}

	void FindAndMoveToPosition()
	{
        if (MatchManager.Instance.CurrentBallHolder == null)
            return;
        Vector3 p = MatchManager.Instance.CurrentBallHolder.transform.position;

		// 2. NavMesh.SamplePositionで、その地点から最も近い「NavMesh上の歩ける場所」を探す
		NavMeshHit hit;
		float maxDistance = 2.0f; // 座標からNavMeshを探す許容距離

		//if (NavMesh.SamplePosition(p, out hit, maxDistance, NavMesh.AllAreas))
		//{
		//    Debug.Log("追跡対象がある");
		//	// 3. 見つかった有効なNavMesh上の座標を目的地に設定
		//	//navMeshAgent.SetDestination(hit.position);
		//
		//    owner.Move((hit.position - transform.position).normalized * 5.0f);
		//}
		navMeshAgent.SetDestination(p);
        //navMeshAgent.velocity

		owner.Move((navMeshAgent.nextPosition - transform.position).normalized * 5.0f);
	}

	// UnityBehaviorのBlackBoardを更新する
	void UpdateBlackBoard()
    {
		if (behaviorGraphAgent)
		{
			// BehaviorGraphAgentの変数を更新
			behaviorGraphAgent.SetVariableValue("HavingBall", owner.hasBall);
            if(owner.hasBall)
             	behaviorGraphAgent.SetVariableValue("TeamHavingBall", true);
            else if (MatchManager.Instance.CurrentBallHolder)
               	behaviorGraphAgent.SetVariableValue("TeamHavingBall", owner.teamNo == MatchManager.Instance.CurrentBallHolder.teamNo);
            else
               	behaviorGraphAgent.SetVariableValue("TeamHavingBall", false);
            
			behaviorGraphAgent.SetVariableValue("DistanceFromBall", GetDistanceToBallHolder());
			behaviorGraphAgent.SetVariableValue("Is Down", owner.isDown);
			behaviorGraphAgent.SetVariableValue("CharacterRole", Role);
			behaviorGraphAgent.SetVariableValue("DistanceFromOpponentGoal", GetDistanceToGoal());
		}
        if (!behaviorGraphAgent)
            Debug.Log(name + ":BehaviorGraphAgentを持っていない");
	}



    // ポジショニングの基準座標への移動開始
	public void StartMoveBasePos(float limitMax, PositioningName name)
	{
		// 移動開始フラグを立てる
		steeringController.StartMove(SteeringMode.Arrive);

		// 円状の乱数を作成する
		Vector2 random = Random.insideUnitCircle;
		Vector3 offset = new Vector3(random.x, 0.0f, random.y) * limitMax;

		// ロールで決定された場所を目指す
		SetTargetPosition(positioningBase[name] + offset);
		//steeringController.TargetPosition = positioningBase + offset;
	}

	// ゴールまでの距離
	public float GetDistanceToGoal()
	{
		Transform Goal = (owner.teamNo == 0) ? MatchManager.Instance.EnemyGoal : MatchManager.Instance.PlayerGoal;

		return Vector3.Distance(transform.position, Goal.position);
	}

    private float GetDistanceToBallHolder()
    {
        // ヌルチェック
        if (MatchManager.Instance.CurrentBallHolder == null)
            return float.MaxValue;
        Vector3 Distance = MatchManager.Instance.CurrentBallHolder.transform.position - transform.position;

        return Distance.magnitude;
    }

	public void GetAllowedDirectionRange2D(Vector2 forward, float halfAngleRad, out Vector2 leftBoundary, out Vector2 rightBoundary)
	{
		// 正面の絶対角度（ラジアン）を算出
		float baseAngle = Mathf.Atan2(forward.y, forward.x);

		// 左右の限界角度を計算
		float leftAngle = baseAngle + halfAngleRad;
		float rightAngle = baseAngle - halfAngleRad;

		// 角度から単位ベクトルを復元
		leftBoundary = new Vector2(Mathf.Cos(leftAngle), Mathf.Sin(leftAngle));
		rightBoundary = new Vector2(Mathf.Cos(rightAngle), Mathf.Sin(rightAngle));
	}

	public void GetAllowedDirectionRangeByQuaternion(Vector3 forward, float halfAngleDeg, out Vector3 leftBoundary, out Vector3 rightBoundary)
	{
		// 正面ベクトルを基準に、指定角度だけ左と右に回転させたベクトルを作る
		Quaternion leftRot = Quaternion.Euler(0f, -halfAngleDeg, 0f);
		Quaternion rightRot = Quaternion.Euler(0f, halfAngleDeg, 0f);

		// ※Unityの座標系や回転方向に合わせて軸を調整してください
		leftBoundary = leftRot * forward;
		rightBoundary = rightRot * forward;
	}

	public void GetAllowedDirectionRange3D(Vector3 forward, float halfAngleRad, out Vector3 leftBoundary, out Vector3 rightBoundary)
	{
		// XZ平面における基準の絶対角度（ラジアン）を算出
		float baseAngle = Mathf.Atan2(forward.z, forward.x);

		// 左右の限界角度を計算
		float leftAngle = baseAngle + halfAngleRad;
		float rightAngle = baseAngle - halfAngleRad;

		// 角度からXZ平面上の単位ベクトルを復元（Y成分は 0）
		leftBoundary = new Vector3(Mathf.Cos(leftAngle), 0f, Mathf.Sin(leftAngle));
		rightBoundary = new Vector3(Mathf.Cos(rightAngle), 0f, Mathf.Sin(rightAngle));
	}

	private void OnDrawGizmosSelected()
	{
		// 3. 目的地の位置に小さな球を描画
		if (navMeshAgent != null)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawSphere(navMeshAgent.steeringTarget, 1f);
			//Gizmos.color = Color.green;
			//Gizmos.DrawSphere(navMeshAgent.pathEndPosition, 1f);

			//foreach (Vector3 point in navMeshAgent.path.corners)
			//{
			//	Gizmos.color = Color.gray;
			//	Gizmos.DrawSphere(point, 1f);
			//}
		}

		// キャラクターの視界を描画する
		GetAllowedDirectionRange3D(transform.forward, Mathf.PI / 4.0f, out Vector3 leftBoundary, out Vector3 rightBoundary);
		Gizmos.color = Color.black;
		// 描画する長さを調整
		leftBoundary = leftBoundary.normalized * 10.0f;
		rightBoundary = rightBoundary.normalized * 10.0f;

		// 本来の座標から少し上を始点にする
		Vector3 from = transform.position + Vector3.up;
		// 描画処理
		Gizmos.DrawLine(from, from + leftBoundary);
		Gizmos.DrawLine(from, from + rightBoundary);

		Gizmos.color = Color.red;
		Gizmos.DrawSphere(endTargetPosition, 1.0f);

		//Gizmos.color = Color.red;
		//Gizmos.DrawSphere(endTargetPosition, 1f);

	}
}
