using System;
using System.Collections.Generic;
using UnityEngine;
using static GameBall;

[CreateAssetMenu(fileName = "./CPU/ScoreEvaluator/ScoreEnablePass", menuName = "Evaluators/ScoreEnablePass")]
[Serializable]
public class ScoreEnablePass : ScoreEvaluatorBase
{
	public override float CalculateScore(Vector3 worldPos, CharacterTeam team, in IReadOnlyList<PitchCharacter> pitchCharacters, EvaluationContext context)
	{
		if (MatchManager.Instance.CurrentBallHolder == null)
			return 0.0f;
		// 所属チームで比較
		if (MatchManager.Instance.CurrentBallHolder.teamNo != (int)team)
			return 0.0f;

		Vector3 ballPos = MatchManager.Instance.CurrentBallHolder.transform.position;
		Vector3 direction = (worldPos - ballPos);
		//Vector3 direction = (ballPos - cellPos);

		// 同じチームのキャラを除外する
		LayerMask layer = ~(1 << MatchManager.Instance.CurrentBallHolder.gameObject.layer);

		float radius = context.GridSize.x / 2.0f;
		Ray ray = new Ray(ballPos, direction.normalized);
		if (Physics.SphereCast(ray, radius, out RaycastHit hitInfo, direction.magnitude, layer))
		{
			if (hitInfo.transform.tag == "Character")
				return 0.0f;
		}

		return weight;
	}
}