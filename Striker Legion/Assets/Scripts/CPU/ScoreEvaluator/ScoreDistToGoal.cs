using System;
using System.Collections.Generic;
using UnityEngine;
using static GameBall;

[CreateAssetMenu(fileName = "./CPU/ScoreEvaluator/ScoreDistToGoal", menuName = "Evaluators/ScoreDistToGoal")]
[Serializable]
public class ScoreDistToGoal : ScoreEvaluatorBase
{
	public override float CalculateScore(Vector3 worldPos, CharacterTeam team, CharacterRole role, in IReadOnlyList<PitchCharacter> pitchCharacters, EvaluationContext context)
	{
		CharacterTeam opponentTeam = (team == CharacterTeam.Player) ? CharacterTeam.Enemy : CharacterTeam.Player;
		// ゴールまでの距離
		float distToGoal = Vector3.Distance(worldPos, MatchManager.Instance.GetGoalPosition(opponentTeam));
		// 逆数でゴールに近いほどスコアが高くなるようにする
		float goalScore = Mathf.Clamp01(1.0f - (distToGoal / context.TotalSize.y));

		// ウェイト調整
		//return 0.0f;
		return goalScore * SelectWeight(role);
	}
}