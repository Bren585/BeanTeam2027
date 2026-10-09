using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Shoot", story: "Shoot To Goal", category: "Action", id: "798e82a05e8af16932bdd520544b352a")]
public partial class ShootAction : ActionBase
{

    protected override Status OnStart()
    {
        base.OnStart();

		// 行動をデバッグで表示
		characterComponent.DebugText.SetText("シュートする");

		return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

