using UnityEngine;

public class PitchHeatmapVisualizer : MonoBehaviour
{
	Vector2 pitchSize = new Vector2(68f, 105f);
	int gridX = 14;
	int gridZ = 20;

	[Header("Visualization Settings")]
	[Tooltip("ヒートマップの色グラデーション")]
	public Gradient heatmapGradient = new Gradient();

	[Range(0f, 1f)]
	public float surfaceAlpha = 0.4f; // セルの透明度

	public bool showBestPosition = true;

	private float[,] gridScores;
	private Vector3 currentBestPos;

	public void Initialize(Vector2 pitchSize, int gridX, int gridZ)
	{
		this.gridX = gridX;
		this.gridZ = gridZ;
		this.pitchSize = pitchSize;

		gridScores = new float[gridX, gridZ];
		InitDefaultGradient();
	}

	//void Awake()
	//{
	//	gridScores = new float[gridX, gridZ];
	//	InitDefaultGradient();
	//}

	// デフォルトのカラーグラデーション（青 -> 緑 -> 黄 -> 赤）をセットアップ
	void InitDefaultGradient()
	{
		if (heatmapGradient.colorKeys.Length > 0) return;

		GradientColorKey[] gck = new GradientColorKey[4];
		gck[0] = new GradientColorKey(Color.blue, 0.0f);   // 低スコア（危険・価値なし）
		gck[1] = new GradientColorKey(Color.cyan, 0.33f);
		gck[2] = new GradientColorKey(Color.yellow, 0.66f);
		gck[3] = new GradientColorKey(Color.red, 1.0f);    // 高スコア（最良・パス先最適）

		GradientAlphaKey[] gak = new GradientAlphaKey[2];
		gak[0] = new GradientAlphaKey(1.0f, 0.0f);
		gak[1] = new GradientAlphaKey(1.0f, 1.0f);

		heatmapGradient.SetKeys(gck, gak);
	}

	void OnDrawGizmos()
	{
		// 描画データがない場合は事前に作成（Editモード用）
		if (gridScores == null || gridScores.GetLength(0) != gridX)
		{
			gridScores = new float[gridX, gridZ];
			InitDefaultGradient();
		}

		float cellWidth = pitchSize.x / gridX;
		float cellHeight = pitchSize.y / gridZ;

		float maxScore = -1f;
		Vector3 bestPos = Vector3.zero;

		// 1. 各セルのヒートマップ描画
		for (int x = 0; x < gridX; x++)
		{
			for (int z = 0; z < gridZ; z++)
			{
				Vector3 cellPos = GetCellWorldPosition(x, z, cellWidth, cellHeight);
				float score = gridScores[x, z];

				// スコアに基づいてグラデーション色を取得
				Color cellColor = heatmapGradient.Evaluate(score);

				// 面の描画（半透明）
				Gizmos.color = new Color(cellColor.r, cellColor.g, cellColor.b, surfaceAlpha);
				Gizmos.DrawCube(cellPos, new Vector3(cellWidth * 0.95f, 0.01f, cellHeight * 0.95f));

				// 枠線の描画（不透明でくっきり表示）
				Gizmos.color = new Color(cellColor.r, cellColor.g, cellColor.b, 0.8f);
				Gizmos.DrawWireCube(cellPos, new Vector3(cellWidth * 0.95f, 0.01f, cellHeight * 0.95f));

				// 最高スコアの更新チェック
				if (score > maxScore)
				{
					maxScore = score;
					bestPos = cellPos;
				}
			}
		}

		// 2. ベストポジションの強調表示
		if (showBestPosition && maxScore >= 0f)
		{
			Gizmos.color = Color.magenta;
			// 高さのある大きな枠線を表示
			Gizmos.DrawWireCube(bestPos + Vector3.up * 1f, new Vector3(cellWidth, 2f, cellHeight));
			// スフィア（球体）を設置
			Gizmos.DrawSphere(bestPos + Vector3.up * 2f, 0.5f);
		}
	}

	Vector3 GetCellWorldPosition(int x, int z, float w, float h)
	{
		float worldX = (x * w) - (pitchSize.x / 2f) + (w / 2f);
		float worldZ = (z * h) - (pitchSize.y / 2f) + (h / 2f);
		return new Vector3(worldX, transform.position.y, worldZ);
	}

	// 外部からスコア配列を更新するためのメソッド
	public void UpdateScores(float[,] scores)
	{
		gridScores = scores;
	}
}