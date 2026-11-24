using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move Keeper", story: "Move Keeper Action", category: "Action", id: "2f5572955fffa9e83f6e98b9f0fe2451")]
public partial class MoveKeeperAction : ActionBase
{
	// ゴール中心から最低限前に出る距離
	private float minForwardOffset = 10.0f;

	// ゴール中心から最大限前に出る距離
	private float maxForwardOffset = 20.0f;

	// この距離以下になったら最大まで前に出る
	private float maxApproachDistance = 15f;

	// ゴールラインから後ろに下がらないための最小距離（ゴール内に入らない制限
	private float goalLineClampMargin = 3.0f;

	Vector3 goalPosition;

    protected override Status OnStart()
    {
		// 基底クラスの初期化
		if (base.OnStart() == Status.Failure)
			return Status.Failure;

        // 自身のゴール座標を取得
		goalPosition = MatchManager.Instance.GetGoalPosition(characterAIAgent.Team);

        // 移動目標を設定
        Vector3 target = CalculateTargetPosition(MatchManager.Instance.BallTransform.position, GameObject.transform.position);

		characterAIAgent.SetTargetPosition(target);

        // 移動開始処理
        characterAIAgent.SteeringController.StartMove(SteeringMode.Arrive);

        characterAIAgent.DebugText.SetText("キーパーのポジショニング\ntarget:" + target);

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
		// 移動終了で成功を返す
		if (!characterAIAgent.SteeringController.IsMove)
			return Status.Success;

		// 移動目標を更新
		characterAIAgent.SetTargetPosition(CalculateTargetPosition(MatchManager.Instance.BallTransform.position, GameObject.transform.position));

		return Status.Success;
    }

    protected override void OnEnd()
    {
    }

    Vector3 CalculateTargetPosition(Vector3 ballPosition)
    {
        Vector3 ballToGoal = GameObject.transform.position - ballPosition;
        //Vector3 ballToGoal = goalPosition - ballPosition;

        Vector3 selfToGoal = GameObject.transform.position - goalPosition;
        //Vector3 selfToGoal = goalPosition - GameObject.transform.position;

        float dot = Vector3.Dot(ballToGoal, selfToGoal);

        Vector3 target = GameObject.transform.position + ballToGoal.normalized * dot;
        //Vector3 target = goalPosition + ballToGoal.normalized * dot;

        return target;
    }

	public Vector3 CalculateTargetPosition(Vector3 ballCarrierPos, Vector3 currentKeeperPos)
	{
		// 水平面（XZ）で計算するためY成分を揃える（3Dアクション/スポーツゲーム向け）
		Vector3 flatGoalPos = new Vector3(goalPosition.x, 0f, goalPosition.z);
		Vector3 flatCarrierPos = new Vector3(ballCarrierPos.x, 0f, ballCarrierPos.z);

		Vector3 toCarrier = flatCarrierPos - flatGoalPos;
		float distanceToCarrier = toCarrier.magnitude;

		// 敵がゴール中心とほぼ同座標にある場合のゼロ除算・不安定化防止
		if (distanceToCarrier < 0.001f)
		{
			return new Vector3(goalPosition.x, currentKeeperPos.y, goalPosition.z);
		}

		// ゴール中心 -> ボール保持者 への単位ベクトル
		Vector3 directionToCarrier = toCarrier / distanceToCarrier;

		// 敵が近づくほど（distanceToCarrierが小さいほど）前に出る距離を増やす
		float t = 1f - Mathf.Clamp01(distanceToCarrier / maxApproachDistance);
		float forwardOffset = Mathf.Lerp(minForwardOffset, maxForwardOffset, t);

		// 敵とゴールの間を超えないように上限を設ける（線分上に収める）
		float maxAllowedOffset = Mathf.Max(0f, distanceToCarrier - goalLineClampMargin);
		forwardOffset = Mathf.Min(forwardOffset, maxAllowedOffset);

		Vector3 targetFlatPos = flatGoalPos + directionToCarrier * forwardOffset;

		return new Vector3(targetFlatPos.x, currentKeeperPos.y, targetFlatPos.z);
	}
}

