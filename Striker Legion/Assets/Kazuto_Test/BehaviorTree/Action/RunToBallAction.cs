using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Run To Ball", story: "Run toward the ball", category: "Action", id: "314c1c737d373737e7978bb9653fb73f")]
public partial class RunToBallAction : ActionBase
{
    Transform ballTransform;
    protected override Status OnStart()
    {
		// 基底クラスの初期化
		base.OnStart();

		// ボールのTransformを取得
		ballTransform = MatchManager.Instance.BallTransform;

        if(ballTransform == null)
			return Status.Failure;

        // ステアリング開始
        characterAIAgent.SteeringController.StartMove(SteeringMode.Arrive);

        characterAIAgent.SetTargetPosition(ballTransform.position);

        characterAIAgent.DebugText.SetText("ボールに向かって走る");

		return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // ボールのTransformがNullなら失敗
        if(ballTransform == null)
			return Status.Failure;

		// ボールを目標として設定
		characterAIAgent.SetTargetPosition(ballTransform.position);

        // 目標地点到着で成功
        if(!characterAIAgent.SteeringController.IsMove)
			return Status.Success;

        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

