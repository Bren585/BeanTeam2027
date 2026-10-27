using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.TextCore.Text;

public class CharacterPitchScore : MonoBehaviour
{
	[SerializeField] public bool IsDebugGizmo = false;

	// ピッチ上のスコア。PitchEvaluatorからもらう
	float[,] gridScores;

    // 現在いるグリッドのスコア
    float currentGridScore;

    // ピッチのグリッドを構成している情報
    int gridCols;
    int gridRows;
	private Vector3 gridOrigin;
	private Vector2 gridSize = new Vector2(10f, 20f);



	// このコンポーネントを持っているキャラのポジション
	CharacterRole ownerRole;

    // このコンポーネントを持っているキャラの所属チーム
    CharacterTeam ownerTeam;

    // 初期化処理
	public void Initialize(CharacterRole role, CharacterTeam team)
	{
        // このコンポーネントを持っているキャラの情報を受け取る
        ownerRole = role;
        ownerTeam = team;

        // グリッドの情報を取得
        MatchManager.Instance.GetPitchInfo(out gridCols, out gridRows, out gridSize, out gridOrigin);

        // 評価変数のメモリ確保
		gridScores = new float[gridCols, gridRows];
		//Debug.Log($"{gameObject.name}: cols{gridCols},rows{gridRows}");
	}

	// Update is called once per frame
	void Update()
    {
        RefreshEvaluation();
    }

    void RefreshEvaluation()
    {
		// メモリ確保前か確認
		if (gridScores.Length <= 0)
			return;
        // グリッドの計算結果を受け取り
        MatchManager.Instance.GetGridScores(gridScores, ownerTeam, ownerRole);

		// このコンポーネントを持っているキャラとの距離でグリッドを再評価する
		for (int z = 0; z < gridRows; z++)
		{
			//color.b = (float)z / (float)gridRows;
			for (int x = 0; x < gridCols; x++)
			{
				// 計算中のグリッドの座標
				Vector3 gridPosition = GetCellWorldPosition(x, z);

				// 現在のスコア
				float currentScore = gridScores[x, z];

				// グリッドまでの距離で再度計算
				currentScore += ScoreDistanceToGrid(gridPosition, 0.3f);

				// 計算結果で更新
				gridScores[x, z] = currentScore;
			}
		}
	}

	float ScoreDistanceToGrid(Vector3 gridPosition, float weight)
	{
		// この値以上離れている場合は最高得点にする
		float MaxLength = gridSize.y *gridRows;

		// このグリッドまでの距離
		float distance = (transform.position - gridPosition).magnitude;

		// 結果のスコア（近いと高く、遠いと低くなるようにする）
		float score = Mathf.Clamp01(1.0f- (distance / MaxLength));

		return score * weight;
	}

	public Vector3 GetBestPosition()
	{
		// スコアの最大値を保持する変数
		Vector3 bestPosition = Vector3.zero;
		// 最高点
		float bestScore = 0.0f;
		for (int z = 0; z < gridRows; z++)
		{
			//color.b = (float)z / (float)gridRows;
			for (int x = 0; x < gridCols; x++)
			{
				// 今までの最高点より小さい場合、飛ばす
				if (gridScores[x, z] < bestScore)
					continue;

				// 結果を更新
				bestScore = gridScores[x, z];
				bestPosition = GetCellWorldPosition(x, z);
			}
		}

		//Debug.Log($"{gameObject.name}:の最大スコア{bestScore}, 座標{bestPosition}");
		return bestPosition;
	}

	Vector3 GetCellWorldPosition(int x, int z)
	{
		float worldX = gridOrigin.x + (x * gridSize.x) + (gridSize.x / 2.0f);
		float worldZ = gridOrigin.z + (z * gridSize.y) + (gridSize.y / 2.0f);
		return new Vector3(worldX, 0, worldZ);
	}
	void GetCellIndexByWorldPosition(Vector3 worldPosition, out int x, out int z)
	{
		float cellWidth = gridSize.x / gridCols;
		float cellHeight = gridSize.y / gridRows;
		x = Mathf.Clamp(Mathf.FloorToInt((worldPosition.x + (gridSize.x / 2f)) / cellWidth), 0, gridCols - 1);
		z = Mathf.Clamp(Mathf.FloorToInt((worldPosition.z + (gridSize.y / 2f)) / cellHeight), 0, gridRows - 1);
	}


	void OnDrawGizmosSelected()
	{
		if (!IsDebugGizmo)
			return;
		Vector3 offset = new Vector3(0.0f, 1.0f, 0.0f);
		Vector3 size = new Vector3(gridSize.x, 1.0f, gridSize.y);
		UnityEngine.Color color = UnityEngine.Color.white;
		color.a = 1.0f;
		for (int z = 0; z < gridRows; z++)
		{
			//color.b = (float)z / (float)gridRows;
			for (int x = 0; x < gridCols; x++)
			{
				float score = gridScores[x, z];
				color.r = score;
				color.g = score;
				color.b = score;
				Gizmos.color = color;

				Vector3 GridVec = new Vector3(x * gridSize.x, 0.0f, z * gridSize.y);
				Gizmos.DrawCube(GetCellWorldPosition(x, z) + offset, size);
				//Gizmos.DrawCube(gridOrigin + GridVec + offset, size);
				//Gizmos.DrawSphere(GetCellWorldPosition(x, z), 1.0f);
				//Gizmos.DrawSphere(gridOrigin + GridVec + offset, 1.0f);
			}
		}
	}
}
