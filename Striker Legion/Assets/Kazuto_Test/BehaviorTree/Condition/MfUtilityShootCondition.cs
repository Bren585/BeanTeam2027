using System;
using Unity.AppUI.Redux;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "MFUtilityShoot", story: "Utility [Midfielder] shoot", category: "Conditions", id: "1c2886f55f7af9002c3ecc24105f71b6")]
public partial class MfUtilityShootCondition : Condition
{
    // 評価対象のミッドフィルダー(BehaviorGraphで指定)
    [SerializeReference] public BlackboardVariable<CharacterAIAgent> Midfielder;

    public override bool IsTrue()
    {
#if true
        // 評価を下すスコア
        // 高いとミッドフィルダーがパスを可能性が高い
        float score = 0.3f;

        // フォワードを取得する
        CharacterBase Forward = Midfielder.Value.PassTarget();

        // フォワードが存在しない場合はシュートをするためにtrueを返す
        if (Forward == null)
        {
            return true;
        }
        // フォワードがスタン中なら、自身でシュートをする
        if (Forward.isDown)
            return true;

        // ゴールの座標
        Vector3 GoalPosition = MatchManager.Instance.GetGoalPosition(Midfielder.Value.TeamNumber);

		// フォワードとゴールまでの距離
		float distanceToGoalFromForward = (Forward.transform.position - GoalPosition).magnitude;
		// ミッドフィルダーのゴールまでの距離
		float distanceToGoalFromMidFielder = (Midfielder.Value.transform.position - GoalPosition).magnitude;
		//float distanceToGoalFromMidFielder = Midfielder.Value.GetDistanceToGoal();

		// フォワードのほうがゴールまで遠い場合、スコアを加算する
		if (distanceToGoalFromForward > distanceToGoalFromMidFielder)
            score += 0.6f;

        float rand = UnityEngine.Random.Range(0.0f, 1.0f);

        //Debug.Log("MFシュート判定：Score=" + score + "Rand = " +rand);

        // 乱数よりスコアのほうが低ければ、シュートをする
        if (score < rand)
            return true;

		return false;
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
