using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "MFUtilityPass", story: "[Midfielder] Compare Utility Score of Pass and Dribble", category: "Conditions", id: "06742bcbb93c4d6a229a7412f59c544a")]
public partial class MfUtilityPassCondition : Condition
{
    [SerializeReference] public BlackboardVariable<SampleCharacter> Midfielder;

    public override bool IsTrue()
    {
        // 結果を決めるスコア
        float score = 0.0f;

        // もっとも近い味方をパスの対象として取得する
		bool isSuccess = Midfielder.Value.CharacterSensor.GetClosestCharacter(LayerMask.LayerToName(GameObject.layer), float.MaxValue, out SampleCharacter TargetCharacter);

		// 取得に失敗するか、対象がヌルの場合はパスができないと判断してfalseを返す
		if (!isSuccess || TargetCharacter == null)
            return false;

        // パスの対象のゴールまでの距離
        float TargetGoalDistance = TargetCharacter.GetDistanceToGoal();
        //float TargetGoalDistance = TargetCharacter.CharacterSensor.GetDistanceToGoal(false);

		// パスが成功するかどうか
		bool IsPassSuccess = Midfielder.Value.CharacterSensor.IsPassSuccess(TargetCharacter.transform);

        if (TargetGoalDistance > 4.0f)
            score += 0.4f;

        if(IsPassSuccess)
            score += 0.6f;

		float rand = UnityEngine.Random.Range(0.0f, 1.0f);

		// 乱数よりスコアのほうが高ければ、パスをする
		if (score < rand)
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
