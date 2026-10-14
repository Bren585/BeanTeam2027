using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Enable Pass", story: "Are there ally can receive this pass from [CharacterAIAgent] Inverse [InverseButton]", category: "Conditions", id: "a43b610fdb7b3ffe0c59da6f83161fbb")]
public partial class EnablePassCondition : Condition
{
	[SerializeReference] public BlackboardVariable<CharacterAIAgent> CharacterAIAgent;
	[SerializeReference] public BlackboardVariable<bool> InverseButton;



	public override bool IsTrue()
    {
#if false
        // 動けるキャラクターを探す
        IReadOnlyList<SampleCharacter> ActiveCharacters = MatchManager.Instance.GetActiveCharacters(CharacterComponent.Value.TeamType);

        bool result = PassUtility.CanPassToAnyAlly(ActiveCharacters, CharacterComponent);
        return (InverseButton) ? !result : InverseButton;
      
#else
        return CharacterAIAgent.Value.EnablePass();

#endif
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
