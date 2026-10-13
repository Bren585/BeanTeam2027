using Unity.Behavior;
using UnityEngine;

[RequireComponent(typeof(CharacterBase))]
[RequireComponent(typeof(SteeringController))]
[RequireComponent(typeof(BehaviorGraphAgent))]
public class CharacterAIAgent : MonoBehaviour
{
    // ビヘイビアエージェント
    private BehaviorGraphAgent behaviorGraphAgent;

    // キャラクターベース
    private CharacterBase owner;

    // ステアリング制御コントローラ
    private SteeringController steeringController;
    public SteeringController SteeringController => steeringController;

    // デバッグ用のテキスト表示
    public DebugText DebugText { get; private set; }

	// 色情報
	public Color BaseColor { get; private set; }
	
	//------------------------------ getter -----------------------------------
	// キャラクターがボールを持っているか
	public bool HasBall => owner.hasBall;
    // キャラのチーム番号
    public int TeamNumber => owner.teamNo; 

    protected void Awake()
    {
        // コンポーネントを取得
        owner = GetComponent<CharacterBase>();
        steeringController = GetComponent<SteeringController>();
		behaviorGraphAgent = GetComponent<BehaviorGraphAgent>();

		// デバッグテキストコンポーネントを追加
		DebugText = gameObject.AddComponent<DebugText>();
	}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
