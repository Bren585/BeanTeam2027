using Unity.Behavior;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class SampleCharacter : MonoBehaviour
{
    // ボールのオブジェクト
    [SerializeField] private GameObject ballObject;

	// ボール保持者かどうかのフラグ
	[SerializeField] public bool isHoldingBall = false;

	// 行動後硬直時間
	private float stanTime = 0.0f;

	// 硬直時間の最大値
	private const float MaxStanTime = 1.0f;

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

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		// BehaviorGraphAgentの取得
		behaviorGraphAgent = GetComponent<BehaviorGraphAgent>();
        if(behaviorGraphAgent == null)
		{
			Debug.Log(name + ": BehaviorGraphAgentの取得に失敗");
		}
		if (ballObject == null)
        {
            Debug.Log(name + ": ボールのオブジェクトの登録忘れ");
        }

		// キャラクターセンサーを生成
		characterSensor = gameObject.AddComponent<CharacterSensor>();
	}

	// Update is called once per frame
	void Update()
	{
		// 硬直時間の処理
		if(stanTime > 0.0f)
		{
			stanTime -= Time.deltaTime;
			if (stanTime < 0.0f)
				stanTime = 0.0f;
		}

		if (isHoldingBall)
		{
			ballObject.transform.SetPositionAndRotation(transform.position + Vector3.up * 0.5f, transform.rotation);
			//Debug.Log("ボールの位置更新");
		}

		if (behaviorGraphAgent)
		{
			// BehaviorGraphAgentの変数を更新
			behaviorGraphAgent.SetVariableValue("HavingBall", isHoldingBall);
			behaviorGraphAgent.SetVariableValue("DistanceFromBall", GetDistanceToBall());
			behaviorGraphAgent.SetVariableValue("StanTime", stanTime);
		}
	}

	float GetDistanceToBall()
	{
		if (ballObject == null)
		{
			Debug.Log(name + ": ボールのオブジェクトが未登録");
			return float.MaxValue;
		}
		return Vector3.Distance(transform.position, ballObject.transform.position);
	}

	public void StartStan()
	{
		stanTime = MaxStanTime;
	}
}
