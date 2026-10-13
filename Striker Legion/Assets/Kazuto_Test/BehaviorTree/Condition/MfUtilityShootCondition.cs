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
        SampleCharacter Forward = MatchManager.Instance.GetAllyByRole(Midfielder.Value.TeamType, CharacterRole.Forward);
        //SampleCharacter Forward = Midfielder.Value.CharacterSensor.GetAllyByRole(CharacterRole.Forward);

		// フォワードが存在しない場合はシュートをするためにtrueを返す
		if (Forward == null)
            return true;

        // フォワードがスタン中なら、自身でシュートをする
        if (Forward.IsStan())
            return true;

        // フォワードとゴールまでの距離
        float distanceToGoalFromForward = Forward.GetDistanceToGoal();
		// ミッドフィルダーのゴールまでの距離
		float distanceToGoalFromMidFielder = Midfielder.Value.GetDistanceToGoal();

		// フォワードのほうがゴールまで遠い場合、スコアを加算する
		if (distanceToGoalFromForward > distanceToGoalFromMidFielder)
            score += 0.3f;

		// パスが成功するかどうか
		String LayerStr = GameObject.layer == LayerMask.NameToLayer("Player") ? "Enemy" : "Player";
		bool IsPassSuccess = PassUtility.IsPassSuccess(GameObject.transform, Forward.transform, LayerMask.NameToLayer(LayerStr));
		//bool IsPassSuccess = Midfielder.Value.CharacterSensor.IsPassSuccess(Forward.transform);

        // パスが成功しないようであれば、シュートがいいとしてスコア上昇
		if (!IsPassSuccess)
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
