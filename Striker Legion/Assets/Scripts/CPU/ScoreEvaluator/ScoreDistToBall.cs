using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements.Experimental;
using static GameBall;

[CreateAssetMenu(fileName = "./CPU/ScoreEvaluator/ScoreDistToBall", menuName = "Evaluators/ScoreDistToBall")]
[Serializable]
public class ScoreDistToBall : ScoreEvaluatorBase
{
	public override float CalculateScore(Vector3 worldPos, CharacterTeam team, in IReadOnlyList<PitchCharacter> pitchCharacters, EvaluationContext context)
	{
		float score = 0.0f;
		float closerLength = context.GridSize.x;
		Vector3 ballPosition = MatchManager.Instance.BallTransform.position;
		// ボールまでの距離
		float distToBall = Vector3.Distance(worldPos, ballPosition);

		float minDistance = float.MaxValue;

		// ボールに最も近い敵のキャラクターの距離を取得
		foreach (var character in pitchCharacters)
		{
			// 所属チームが同じならスキップ
			if (character.team == team)
				continue;

			// 距離測定
			float dist = Vector3.Distance(ballPosition, character.transform.position);

			// 距離が影響範囲外ならスキップ
			if (dist > minDistance)
				continue;

			minDistance = dist;
		}

		// 近すぎる場合は逆にスコアを下げる
		if (distToBall < closerLength)
			score = Mathf.Clamp01(distToBall / closerLength);
		else
			// 逆数でゴールに近いほどスコアが高くなるようにする
			score = Mathf.Clamp01(1.0f - (distToBall / context.TotalSize.y));

		// ボールの近くに敵がいる場合は、スコアが高い場所がより高くなるようにする
		if (minDistance < context.GridSize.y)
			score = Easing.InOutQuad(score);

		// ウェイト調整
		return score * weight;
	}
}