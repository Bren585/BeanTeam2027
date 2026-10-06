using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "MFUtilityShoot", story: "Utility [Midfielder] shoot", category: "Conditions", id: "1c2886f55f7af9002c3ecc24105f71b6")]
public partial class MfUtilityShootCondition : Condition
{
    // 評価対象のミッドフィルダー(BehaviorGraphで指定)
    [SerializeReference] public BlackboardVariable<SampleCharacter> Midfielder;

    public override bool IsTrue()
    {
        // 評価を下すスコア
        // 高いとミッドフィルダーがシュートを可能性が高い
        float score = 0.0f;

        // フォワードを取得する
        SampleCharacter Forward = Midfielder.Value.CharacterSensor.GetAllyByRole(CharacterRole.Forward);

		// フォワードが存在しない場合はシュートをするためにtrueを返す
		if (Forward == null)
            return true;

        // フォワードとゴールまでの距離
        float distanceToGoalFromForward = Forward.CharacterSensor.GetDistanceToGoal(false);
        // ミッドフィルダーのゴールまでの距離
        float distanceToGoalFromMidFielder = Midfielder.Value.CharacterSensor.GetDistanceToGoal(false);

        // フォワードのほうがゴールまで遠い場合、スコアを加算する
        if (distanceToGoalFromForward > distanceToGoalFromMidFielder)
            score += 0.3f;

        // パスが成功するかどうか
        bool IsPassSuccess = Midfielder.Value.CharacterSensor.IsPassSuccess(Forward.transform);

        if (IsPassSuccess)
            score += 0.6f;

        float rand = UnityEngine.Random.Range(0.0f, 1.0f);

        // 乱数よりスコアのほうが高ければ、シュートをする
        if (score < rand)
            return true;

		return false;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
