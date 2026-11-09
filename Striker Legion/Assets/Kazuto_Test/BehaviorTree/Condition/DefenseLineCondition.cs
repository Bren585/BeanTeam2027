using System;
using Unity.Behavior;
using Unity.VisualScripting;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "DefenseLine Condition", story: "BallHolder Intrusion Defense Line [PositionBaseName] this agent [CharacterAIAgent] is inverse [Inverse]", category: "Conditions", id: "f1aeb94b46feca90df0c3c1bf938d89f")]
public partial class DefenseLineCondition : Condition
{
	[SerializeReference] public BlackboardVariable<CharacterAIAgent> CharacterAIAgent;

    [SerializeReference] public BlackboardVariable<PositioningName> PositionBaseName;
    [SerializeReference] public BlackboardVariable<bool> Inverse;

	public override bool IsTrue()
    {
        bool success = (Inverse.Value) ? false : true;

        // ボールを持っているキャラがいないなら、侵入されていない
        //if (MatchManager.Instance.CurrentBallHolder == null)
        //    return !success;

		// ボールを持っているキャラのチームが自分と同じなら、侵入されていない
        if(MatchManager.Instance.CurrentBallHolder.team == CharacterAIAgent.Value.Team)
			return !success;

        if(MatchManager.Instance.BallTransform == null)
			return !success;

        Vector3 ballPosition = MatchManager.Instance.BallTransform.position;

		// 基準から符号を決定
		bool sign = CharacterAIAgent.Value.BasePosition(PositionBaseName.Value).z > 0.0f;

        if (sign)
        {
			// 基準が正の時、基準点よりもボール保持者が後ろにいる場合、侵入されている
			if (CharacterAIAgent.Value.BasePosition(PositionBaseName.Value).z < ballPosition.z)
                return success;
        }
		else
		{
			// 基準が負の時、基準点よりもボール保持者が前にいる場合、侵入されている
			if (CharacterAIAgent.Value.BasePosition(PositionBaseName.Value).z > ballPosition.z)
				return success;
		}
		return !success;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
