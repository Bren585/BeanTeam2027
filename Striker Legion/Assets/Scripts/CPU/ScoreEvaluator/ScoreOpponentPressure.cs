using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static GameBall;

[CreateAssetMenu(fileName = "./CPU/ScoreEvaluator/ScoreOpponentPressure", menuName = "Evaluators/ScoreOpponentPressure")]
// 周囲の敵キャラの数によって評価する
public class ScoreOpponentPressure : ScoreEvaluatorBase
{
	public override float CalculateScore(Vector3 worldPos, CharacterTeam team, in IReadOnlyList<PitchCharacter> pitchCharacters, EvaluationContext context)
	{
		float pressure = 1.0f;
		float influenceRadius = context.GridSize.x + context.GridSize.y; // 影響を及ぼす半径

		// 登録されている全キャラクターから、
		// 相手チームのキャラを探して距離に応じたプレッシャーを加算
		foreach (var character in pitchCharacters)
		{
			// 所属チームが同じならスキップ
			if (character.team == team)
				continue;

			// 距離測定
			float dist = Vector3.Distance(worldPos, character.transform.position);

			// 距離が影響範囲外ならスキップ
			if (dist > influenceRadius)
				continue;

			// 近ければ近いほど強いプレッシャー（逆数や減衰関数）
			pressure -= Mathf.Clamp01(1.0f - (dist / influenceRadius));
		}

		pressure = Mathf.Clamp01(pressure);
		return pressure * weight;
	}
}
