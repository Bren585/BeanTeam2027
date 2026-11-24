using System;
using System.Xml.Linq;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Wandering", story: "Wandering [Role] Position while [timer]", category: "Action", id: "2d0f3e1743739f04ff3bf98729bc3aaa")]
public partial class WanderingAction : ActionBase
{
	[SerializeReference] public BlackboardVariable<PositioningName> Role;

	[SerializeReference] public BlackboardVariable<float> timer;

    float counter = 0.0f;

	protected override Status OnStart()
    {
        if (base.OnStart() == Status.Failure)
            return Status.Failure;

        // 徘徊開始
        base.characterAIAgent.StartWanderingBasePos(Role);

        counter = 0.0f;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // 時間カウント
        counter += Time.deltaTime;

        // 時間経過で止める
        if(timer < counter)
        {
            base.characterAIAgent.SteeringController.CancelMove();
            return Status.Success;
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

