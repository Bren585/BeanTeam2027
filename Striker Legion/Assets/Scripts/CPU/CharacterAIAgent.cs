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
    private Vector3 positioningBase;

    // ステアリング制御コントローラ
    private SteeringController steeringController;

    // デバッグ用のテキスト表示
    public DebugText DebugText { get; private set; }

	// 色情報
	public Color BaseColor { get; private set; }
	
    public CharacterRole Role { get; private set; }

	Vector3 endTargetPosition = Vector3.zero;

	//------------------------------ getter -----------------------------------
    public SteeringController SteeringController => steeringController;

    public CharacterSensor CharacterSensor => characterSensor;

	public Vector3 BasePosition => positioningBase;

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

        navMeshAgent.updatePosition = true;
		navMeshAgent.isStopped = true;
	}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    public void Initialize(Vector3 basePosition, CharacterRole role)
    {
        positioningBase = basePosition;
        Role = role;
		DebugText.SetDefaultText(owner.name +" " + Role);
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
            return;

        // 親の速度を元に計算する
        steeringController.SetParentVelocity(ParentVelocity);

		// 操舵コントローラの目標座標をナビメッシュから取得
		steeringController.TargetPosition = navMeshAgent.steeringTarget;

		// 速力計算
		steeringController.UpdateVelocity();

		if (steeringController.Velocity.magnitude < 0.01f)
            return;

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
	public void StartMoveBasePos(float limitMax)
	{
		// 移動開始フラグを立てる
		steeringController.StartMove(SteeringMode.Arrive);

		// 円状の乱数を作成する
		Vector2 random = Random.insideUnitCircle;
		Vector3 offset = new Vector3(random.x, 0.0f, random.y) * limitMax;

		// ロールで決定された場所を目指す
		SetTargetPosition(positioningBase + offset);
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

	private void OnDrawGizmosSelected()
	{
		// 3. 目的地の位置に小さな球を描画
		if (navMeshAgent != null)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawSphere(navMeshAgent.steeringTarget, 1f);
			//Gizmos.color = Color.green;
			//Gizmos.DrawSphere(navMeshAgent.pathEndPosition, 1f);

			//foreach(Vector3 point in navMeshAgent.path.corners)
			//{
			//	Gizmos.color = Color.black;
			//	Gizmos.DrawSphere(point, 1f);
			//}
		}
		//Gizmos.color = Color.red;
		//Gizmos.DrawSphere(endTargetPosition, 1f);
		
	}
}
