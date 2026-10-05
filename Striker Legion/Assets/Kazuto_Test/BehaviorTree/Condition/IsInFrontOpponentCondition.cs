using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "IsInFrontOpponent", story: "Is There Opponent In Front Of [CharacterComponent]", category: "Conditions", id: "f3a5d238b6c2f0a41b5f313d940d0ec6")]
public partial class IsInFrontOpponentCondition : Condition
{
    [SerializeReference] public BlackboardVariable<SampleCharacter> CharacterComponent;

    public override bool IsTrue()
    {
        return CharacterComponent.Value.CharacterSensor.IsEnemyInRange(LayerMask.LayerToName(CharacterComponent.Value.gameObject.layer), 1.0f);
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
