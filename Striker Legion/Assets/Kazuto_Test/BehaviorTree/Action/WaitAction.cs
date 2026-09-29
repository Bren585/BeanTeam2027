using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Action Wait", 
    story: "Wait Action", 
    category: "Action",
    id: "e4d9ca109993b433944c8855b7e5d41c")]
public partial class ActionWaitAction : ActionBase
{

    protected override Status OnStart()
    {
        isInterrupted.Value = true;

        //Debug.Log("isInterrupted: " + isInterrupted.Value);

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

