using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Enable Pass", story: "Are there ally can receive this pass from [CharacterComponent] Inverse [InverseButton]", category: "Conditions", id: "a43b610fdb7b3ffe0c59da6f83161fbb")]
public partial class EnablePassCondition : Condition
{
	[SerializeReference] public BlackboardVariable<SampleCharacter> CharacterComponent;
	[SerializeReference] public BlackboardVariable<bool> InverseButton;



	public override bool IsTrue()
    {
		// 動けるキャラクターを探す
		IReadOnlyList<SampleCharacter> ActiveCharacters = MatchManager.Instance.GetActiveCharacters(CharacterComponent.Value.TeamType);

        bool result = PassUtility.CanPassToAnyAlly(ActiveCharacters, CharacterComponent);

        return (InverseButton) ? !result : InverseButton;

	}

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
