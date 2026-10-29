using System;
using System.Collections.Generic;
using UnityEngine;
using static GameBall;

[CreateAssetMenu(fileName = "./CPU/ScoreEvaluator/ScoreFrontToBaller", menuName = "Evaluators/ScoreFrontToBaller")]
[Serializable]
public class ScoreFrontToBaller : ScoreEvaluatorBase
{
	public override float CalculateScore(Vector3 worldPos, CharacterTeam team, in IReadOnlyList<PitchCharacter> pitchCharacters, EvaluationContext context)
	{
		// ボールを持っているキャラがいなければ、終了
		if (MatchManager.Instance.CurrentBallHolder == null)
			return 0.0f;

		Vector3 forward = MatchManager.Instance.CurrentBallHolder.transform.forward;
		Vector3 direction = worldPos - MatchManager.Instance.CurrentBallHolder.transform.position;
		
		// 正面ベクトルと方向ベクトルを内積して結果を求める
		float dot = Vector3.Dot(forward, direction.normalized);

		return Mathf.Clamp01(dot * weight);
	}
}