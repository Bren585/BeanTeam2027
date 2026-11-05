using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Log", story: "Condition Log [Text] return [boolean]", category: "Conditions", id: "2d79ede35135fbea3a6eae484a55e457")]
public partial class LogCondition : Condition
{
    [SerializeReference] public BlackboardVariable<string> Text;
    [SerializeReference] public BlackboardVariable<bool> Boolean;

    public override bool IsTrue()
    {
        // Conditionの時にログを表示するためのもの
        Debug.Log(Text.Value, GameObject);

        // そのまま引数を渡す
        return Boolean.Value;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
