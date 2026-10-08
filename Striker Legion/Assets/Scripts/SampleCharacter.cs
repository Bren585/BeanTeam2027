using Unity.Behavior;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class SampleCharacter : MonoBehaviour
{
	// キャラクターのロール
	private CharacterRole characterRole;

	public CharacterRole CharacterRole{
		get { return characterRole; }
	}
	

	// ボールのオブジェクト
	[SerializeField] private GameObject ballObject;

	// ボール保持者かどうかのフラグ
	[SerializeField] public bool isHoldingBall = false;

	// ポジショニングをする基準のトランスフォーム
	private Transform positioningBaseTransform;

	// 色情報
	private Color baseColor;
	public Color BaseColor {
		get { return baseColor; }
	}

	// 行動後硬直時間
	private float stanTime = 0.0f;

	// 硬直時間の最大値
	private const float MaxStanTime = 1.0f;

	// キャラクターのチーム
	public CharacterTeam TeamType { get; private set; }

	// BehaviorGraphAgentの参照
	private BehaviorGraphAgent behaviorGraphAgent;
	// BehaviorGraphAgentの取得用プロパティ
	public BehaviorGraphAgent BehaviorGraphAgent{
        get { return behaviorGraphAgent; }
	}

	// キャラクターのセンサー
	CharacterSensor characterSensor;
	// キャラクターセンサーの取得用プロパティ
	public CharacterSensor CharacterSensor{
		get { return characterSensor; }
	}

	// ステアリング制御用のコントローラー
	SteeringController steeringController;
	public SteeringController SteeringController{
		get { return steeringController; }
	}

	void Awake()
	{
		// キャラクターセンサーを生成
		characterSensor = gameObject.AddComponent<CharacterSensor>();
		// ステアリング制御コントローラを生成
		steeringController = gameObject.AddComponent<SteeringController>();

		
		// BehaviorGraphAgentの取得
		behaviorGraphAgent = GetComponent<BehaviorGraphAgent>();

		// 初期化前に動かないようにする
		behaviorGraphAgent.enabled = false;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		// BehaviorGraphAgentの取得
		//behaviorGraphAgent = GetComponent<BehaviorGraphAgent>();
        if(behaviorGraphAgent == null)
		{
			Debug.Log(name + ": BehaviorGraphAgentの取得に失敗");
		}
		if (ballObject == null)
        {
            Debug.Log(name + ": ボールのオブジェクトの登録忘れ");
        }

		// BehaviorGraphの値を初期設定する
		UpdateBehaviorGraph();
		// ビヘイビアツリー開始
		behaviorGraphAgent.enabled = true;

		// 色を保存する
		baseColor = gameObject.GetComponent<Renderer>().material.GetColor("_BaseColor");
        Debug.Log(name + ": BaseColor=" + baseColor);

		// キャラクターのチームを割り振る
		TeamType = (LayerMask.LayerToName(gameObject.layer) == "Player") ? CharacterTeam.Player : CharacterTeam.Enemy;
	}

	// キャラクターのパラメータを外部の値で初期化用
	public void InitializeData(CharacterRole role, Transform positioningBase)
	{
		characterRole = role;
		positioningBaseTransform = positioningBase;

		// BehaviorGraphの値を初期設定する
		UpdateBehaviorGraph();
	}

	// Update is called once per frame
	void Update()
	{
		// 硬直時間の処理
		if (stanTime > 0.0f)
		{
			stanTime -= Time.deltaTime;
			if (stanTime < 0.0f)
			{
				Debug.Log(gameObject.name + "スタン解消");
				stanTime = 0.0f;
			}
		}

		if (isHoldingBall)
		{
			ballObject.transform.SetPositionAndRotation(transform.position + Vector3.up * 0.5f, transform.rotation);
			//Debug.Log("ボールの位置更新");
		}

		// BehaviorGraphの値を更新する
		UpdateBehaviorGraph();
	}

	private void UpdateBehaviorGraph()
	{
		if (behaviorGraphAgent)
		{
			// BehaviorGraphAgentの変数を更新
			behaviorGraphAgent.SetVariableValue("HavingBall", isHoldingBall);
			float value = GetDistanceToBall();
			//Debug.Log(name + "のボールとの距離：" + value);
			behaviorGraphAgent.SetVariableValue("DistanceFromBall", value);
			behaviorGraphAgent.SetVariableValue("StanTime", stanTime);
			behaviorGraphAgent.SetVariableValue("CharacterRole", characterRole);
			behaviorGraphAgent.SetVariableValue("DistanceFromOpponentGoal", GetDistanceToGoal());
			//behaviorGraphAgent.SetVariableValue("DistanceFromOpponentGoal", characterSensor.GetDistanceToGoal(false));
		}
	}

	float GetDistanceToBall()
	{
		if (ballObject == null)
		{
			Debug.Log(name + ": ボールのオブジェクトが未登録");
			return float.MaxValue;
		}

		float value = Vector3.Distance(transform.position, ballObject.transform.position);
		return value;
	}

	// ボールをステアリング制御のターゲットとする
	public void SetTargetIsBall()
	{

		steeringController.TargetPosition = ballObject.transform.position;
	}

	public void StartStan(float stan = MaxStanTime)
	{
		stanTime = stan;

		if (behaviorGraphAgent.GetVariableID("StanTime", out Unity.Behavior.GraphFramework.SerializableGUID targetVar))
		{
			behaviorGraphAgent.SetVariableValue(targetVar, stanTime);
		}
		//behaviorGraphAgent.SetVariableValue("StanTime", stanTime);
	}

	public void StartMoveBasePos(float limitMax)
	{
		// 移動開始フラグを立てる
		steeringController.StartMove();

		// 円状の乱数を作成する
		Vector2 random = Random.insideUnitCircle;
		Vector3 offset = new Vector3(random.x, 0.0f, random.y) * limitMax;

		// ロールで決定された場所を目指す
		steeringController.TargetPosition = positioningBaseTransform.position + offset;	
	}

	// ゴールまでの距離
	public float GetDistanceToGoal()
	{
		Transform Goal = (TeamType == CharacterTeam.Player) ? MatchManager.Instance.PlayerGoal : MatchManager.Instance.EnemyGoal;
		
		return Vector3.Distance(transform.position, Goal.position);
	}
}
