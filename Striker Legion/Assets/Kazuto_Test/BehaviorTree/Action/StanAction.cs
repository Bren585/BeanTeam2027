using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "StanAction", story: "Unable to Action While Stan [Time]", category: "Action", id: "9ee0565aba92322148eb03a2d2d0ae28")]
public partial class StanAction : Action
{
    [SerializeReference] public BlackboardVariable<float> Time;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // タイマーが0以下になるまでスタン中とする
        if (Time > 0.0f)
            return Status.Running;

        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

