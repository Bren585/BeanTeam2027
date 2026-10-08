using UnityEngine;

// キャラクターのステアリング制御を行うクラス
public class SteeringController : MonoBehaviour
{
    // 目標位置
    private Vector3 targetPosition;
    public Vector3 TargetPosition{
        get { return targetPosition; }
        set { targetPosition = value; }
    }

    // 速度
    private Vector3 velocity;

    // 最大速度
    private const float MaxSpeed = 5.0f;

	// 移動を許容する距離
	private const float AcceptableDistance = 0.1f;

    // 移動するかのフラグ
    bool isMove = false;

    // 移動開始用
    public void StartMove() {
		Debug.Log("ステアリング開始");

		isMove = true;
    }

    // 移動していないかどうかを返す
    public bool IsMove() { 
        return isMove;
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

        // 移動処理(仮だから単純にする。経路探索が必要な可能性大)
		transform.position = Vector3.MoveTowards(transform.position, TargetPosition, MaxSpeed * Time.deltaTime);

        // 規定の距離まで移動したら、移動終了
        if (Vector3.Distance(transform.position, TargetPosition) < AcceptableDistance)
        {
            Debug.Log("ステアリング終了");
            isMove = false;
        }
	}
}
