using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MoveBasePosition", story: "Move to Position Selected by Role:[RandomLimit]", category: "Action", id: "4254191743734e158d0f3e00bc6c6270")]
public partial class MoveBasePositionAction : ActionBase
{
    [SerializeReference] public BlackboardVariable<float> RandomLimit;
	protected override Status OnStart()
    {
        // 基底クラスの初期化
        base.OnStart();

        // 決められた座標への移動開始
        characterComponent.StartMoveBasePos(RandomLimit);

		// 行動をデバッグで表示
		characterComponent.DebugText.SetText("指定された位置に移動");

		return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // 移動終了で成功
        if(!characterComponent.SteeringController.IsMove)
            return Status.Success;

        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

