using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Enable Shoot", story: "[CharacterAIAgent] Enable Shoot to Goal", category: "Conditions", id: "bce88fdd2486b65239e6cbfee53fb7fd")]
public partial class EnableShootCondition : Condition
{
	[SerializeReference] public BlackboardVariable<CharacterAIAgent> CharacterAIAgent;

	public override bool IsTrue()
    {
		return CharacterAIAgent.Value.IsShootable;
        //return true;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
