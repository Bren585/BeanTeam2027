using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "MFUtilityPass", story: "[Midfielder] Compare Utility Score of Pass and Dribble", category: "Conditions", id: "06742bcbb93c4d6a229a7412f59c544a")]
public partial class MfUtilityPassCondition : Condition
{
    [SerializeReference] public BlackboardVariable<SampleCharacter> Midfielder;

    public override bool IsTrue()
    {
       
		// 動けるキャラクターを探す
		IReadOnlyList<SampleCharacter> ActiveCharacters = MatchManager.Instance.GetActiveCharacters(Midfielder.Value.TeamType);

		
        // 最も低かったスコア
        float MinScore = PassUtility.CostPassToAnyAlly(ActiveCharacters, Midfielder);

        // 乱数の取得
		float rand = UnityEngine.Random.Range(0.0f, 1.0f);

		// 乱数よりスコアのほうが高ければ、パスをする
		if (MinScore < rand)
			return false;

		return true;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
