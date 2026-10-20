using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Increment", story: "Add dTime to [Time]", category: "Action", id: "6d5e60af780a2f45d3b3ffb85482f2b3")]
public partial class IncrementAction : Action
{
    [SerializeReference] public BlackboardVariable<float> Time;
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Time += (BlackboardVariable<float>)UnityEngine.Time.deltaTime;
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

