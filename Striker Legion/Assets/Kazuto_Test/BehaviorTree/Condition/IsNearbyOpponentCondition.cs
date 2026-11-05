using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Is Nearby Opponent", story: "Are there any enemies within [Radius] near the [CharacterAIAgent]", category: "Conditions", id: "8313257fb70ddfa72c43b0b0ab2a8dcd")]
public partial class IsNearbyOpponentCondition : Condition
{
    [SerializeReference] public BlackboardVariable<CharacterAIAgent> CharacterAIAgent;
    [SerializeReference] public BlackboardVariable<float> Radius;

    public override bool IsTrue()
    {
        return CharacterAIAgent.Value.CharacterSensor.IsNearbyOpponent(Radius);
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
