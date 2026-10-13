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
#if false
        // 敵がいるかどうかをレイキャストで判定する
        return CharacterAIAgent.Value.CharacterSensor.IsEnemyInRange(LayerMask.LayerToName(CharacterComponent.Value.gameObject.layer), Distance.Value);
#else
        return true;
#endif
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
