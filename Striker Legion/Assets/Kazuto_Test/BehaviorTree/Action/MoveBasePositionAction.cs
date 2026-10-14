using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MoveBasePosition", story: "Move to Position Selected by Role while [Timer]: Random Limit[RandomLimit]", category: "Action", id: "4254191743734e158d0f3e00bc6c6270")]
public partial class MoveBasePositionAction : ActionBase
{
    [SerializeReference] public BlackboardVariable<float> RandomLimit;

	[SerializeReference] public BlackboardVariable<float> Timer;

    float currentTimer = 0.0f;

	protected override Status OnStart()
    {
        // 基底クラスの初期化
        base.OnStart();

        // 決められた座標への移動開始
        characterComponent.StartMoveBasePos(RandomLimit);

		// 行動をデバッグで表示
		characterComponent.DebugText.SetText("指定された位置に移動");

        // タイマーが未設定の場合、一秒を設定する
        if (Timer == null || Timer.Value <= 0.0f)
            currentTimer = 1.0f;
        else
            currentTimer = Timer.Value;

		return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // 移動終了で成功
        if(!characterComponent.SteeringController.IsMove)
            return Status.Success;

        // 設定された時間経過で成功を返す
        if (currentTimer < 0.0f)
            return Status.Success;

		currentTimer -= Time.deltaTime;

        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

