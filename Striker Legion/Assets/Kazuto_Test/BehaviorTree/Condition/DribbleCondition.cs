using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Dribble Condition", story: "[CharacterAIAgent] Branching Logic for Dribbling", category: "Conditions", id: "7f5e84450b2135283fa3ed23a4b36d5a")]
public partial class DribbleCondition : Condition
{
	[SerializeReference] public BlackboardVariable<CharacterAIAgent> CharacterAIAgent;
	public override bool IsTrue()
    {
        // キャラのチームを取得
        CharacterTeam team = CharacterAIAgent.Value.Team;

		// 敵チームのゴールの位置を取得
		Vector3 opponentGoalPosition = MatchManager.Instance.GetGoalPosition(team == CharacterTeam.Player ? CharacterTeam.Enemy : CharacterTeam.Player);

        // オフェンスの基準座標
        Vector3 forwardBasePosition = CharacterAIAgent.Value.BasePosition(PositioningName.Offense);

        // オフェンスの基準座標までのレイを生成
        Ray ray = new Ray(GameObject.transform.position, forwardBasePosition - GameObject.transform.position);
        bool isHit = Physics.SphereCast(ray, 3.0f, out RaycastHit hit);

        if(hit.collider != null)
		{
            // ヒット対象がキャラクター以外であれば、trueを返す
            if (hit.collider.tag != "Character")
                return true;
			// ヒット対象が同一レイヤーであれば、trueを返す
			if (hit.collider.gameObject.layer == GameObject.layer)
                return true;
		}

        Debug.Log(GameObject.name + " DribbleCondition: " + isHit + ", HitObject: " + (hit.collider != null ? hit.collider.name : "null"));
		return !isHit;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
