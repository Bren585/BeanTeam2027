using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static GameBall;

[CreateAssetMenu(fileName = "./CPU/ScoreEvaluator/ScoreSide", menuName = "Evaluators/ScoreSide")]
// サイドのスコアが高くなる傾向があるため、修正する
public class ScoreSide : ScoreEvaluatorBase
{
	public override float CalculateScore(Vector3 worldPos, CharacterTeam team, in IReadOnlyList<PitchCharacter> pitchCharacters, EvaluationContext context)
	{
		// 座標からグリッドの番号を取得する
		GetCellIndexByWorldPosition(worldPos, context, out int x, out int z);

		// グリッドのセンター
		int halfCols = context.GridCols / 2;

		// サイドに近いほどスコアが低くなるようにする
		float score = Mathf.Clamp01(1.0f - (Mathf.Abs(x - halfCols) / halfCols));

		return score * weight;
	}

	void GetCellIndexByWorldPosition(Vector3 worldPosition, EvaluationContext context,out int x, out int z)
	{
		float cellWidth = context.TotalSize.x / context.GridCols;
		float cellHeight = context.TotalSize.y / context.GridRows;

		Vector3 vec = worldPosition - context.GridOrigin;

		x = Mathf.Clamp(Mathf.FloorToInt(vec.x / cellWidth),  0,  context.GridCols - 1);
		z = Mathf.Clamp(Mathf.FloorToInt(vec.z / cellHeight), 0, context.GridRows - 1);
	}
}
