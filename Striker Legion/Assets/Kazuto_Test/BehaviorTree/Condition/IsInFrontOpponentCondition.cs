using System;
using Unity.Behavior;
using Unity.VisualScripting;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "IsInFrontOpponent", story: "Is There Opponent Within [Distance] In Front Of [CharacterAIAgent]", category: "Conditions", id: "f3a5d238b6c2f0a41b5f313d940d0ec6")]
public partial class IsInFrontOpponentCondition : Condition
{
    [SerializeReference] public BlackboardVariable<CharacterAIAgent> CharacterAIAgent;
    [SerializeReference] public BlackboardVariable<float> Distance;

    public override bool IsTrue()
    {
        // 敵がいるかどうかをレイキャストで判定する
        return CharacterAIAgent.Value.CharacterSensor.IsEnemyInRange(LayerMask.LayerToName(CharacterAIAgent.Value.gameObject.layer), Distance.Value);
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
