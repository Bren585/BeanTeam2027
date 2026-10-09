using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Ball Clear", story: "Kick the ball far away to a safe place", category: "Action", id: "6155d7aa7ab61c9ca5cf5d439ddd3f08")]
public partial class BallClearAction : ActionBase
{

    protected override Status OnStart()
    {
        base.OnStart();

        // 行動をデバッグで表示
		characterComponent.DebugText.SetText("ボールをクリアする");

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

