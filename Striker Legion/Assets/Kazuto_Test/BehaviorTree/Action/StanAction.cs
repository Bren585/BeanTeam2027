using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "StanAction", story: "Unable to Action While Stan [IsDown]", category: "Action", id: "9ee0565aba92322148eb03a2d2d0ae28")]
public partial class StanAction : ActionBase
{
    [SerializeReference] public BlackboardVariable<bool> IsDown;

    protected override Status OnStart()
    {
        base.OnStart();

		// 行動をデバッグで表示
		characterAIAgent.DebugText.SetText("ダウン中");

		return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // ダウンフラグが立っている間は行動不能とする
        if (IsDown)
        //if (Time > 0.0f)
            return Status.Running;

        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

