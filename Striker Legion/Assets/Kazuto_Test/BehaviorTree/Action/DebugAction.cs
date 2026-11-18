using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "DebugAction", story: "Dummy Action for Debugging Only [DebugBool]", category: "Action", id: "33bef1dd9aad6c487287c8ff82e492dd")]
public partial class DebugAction : ActionBase
{
    [SerializeReference] public BlackboardVariable<bool> DebugBool;

    protected override Status OnStart()
    {
        if (base.OnStart() == Status.Failure)
            return Status.Failure;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Debug.Log("DebugAction実行");
        return (DebugBool.Value) ? Status.Success : Status.Failure;
    }

    protected override void OnEnd()
    {
    }
}

