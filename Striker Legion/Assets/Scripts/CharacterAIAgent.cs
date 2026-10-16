using Unity.AppUI.UI;
using Unity.Behavior;
using UnityEditor.EditorTools;
using UnityEngine;

[RequireComponent(typeof(CharacterBase))]
[RequireComponent(typeof(SteeringController))]
[RequireComponent(typeof(BehaviorGraphAgent))]
public class CharacterAIAgent : MonoBehaviour
{
    // ビヘイビアエージェント
    private BehaviorGraphAgent behaviorGraphAgent;

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

	//------------------------------ getter -----------------------------------
    public SteeringController SteeringController => steeringController;

    public CharacterSensor CharacterSensor => characterSensor;

	// キャラクターがボールを持っているか
	public bool HasBall => owner.hasBall;
    // キャラのチーム番号
    public int TeamNumber => owner.teamNo;

    // パス関数（CharacterBaseから呼び出し）
    public void Pass() => owner.Shoot();

    // シュート関数（CharacterBaseから呼び出し）
    public void Shoot() => owner.Shoot();

    // パスが可能か
    public bool EnablePass() => owner.hasBall && owner.PassTarget() != null;
    public CharacterBase PassTarget() => owner.PassTarget();

    public void Tackle() => owner.Tackle();

    protected void Awake()
    {
        // コンポーネントを取得
        owner = GetComponent<CharacterBase>();
        steeringController = GetComponent<SteeringController>();
		behaviorGraphAgent = GetComponent<BehaviorGraphAgent>();

		// デバッグテキストコンポーネントを追加
		DebugText = gameObject.AddComponent<DebugText>();
		characterSensor = gameObject.AddComponent<CharacterSensor>();
	}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DebugText.SetDefaultText(owner.name);
    }

    public void Initialize(Vector3 basePosition, CharacterRole role)
    {
        positioningBase = basePosition;
        Role = role;
    }

	// Update is called once per frame
	void Update()
    {
        // 操舵レイヤーの更新処理
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

        if (steeringController.Velocity.magnitude < 0.01f)
            return;
        owner.Move(steeringController.Velocity);
        //owner.Move(steeringController.Velocity.normalized);
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
		steeringController.TargetPosition = positioningBase + offset;
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
}
