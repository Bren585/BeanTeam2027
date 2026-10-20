using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// ゴールと選手をまとめてチームとする

public enum TeamType
{
	Player,
	Enemy,
}

[Serializable]
public struct TeamObject
{
	public Transform Goal;
	public List<Transform> characters;
	public float[,] gridScores;

	public TeamType teamType;

	public Vector3 bestPosition;
}


// ピッチの状態を評価する
public class PitchEvaluator : MonoBehaviour
{
	// シングルトンのインスタンス
	public static PitchEvaluator Instance { get; private set; }


	//[Header("Grid Settings")]
	public Vector2 pitchSize = new Vector2(10f, 20f); // 標準的なサッカー場サイズ
	public int gridX = 5;
	public int gridZ = 10;

	[Header("References")]
	// プレイヤーのチーム
	public TeamObject Players;
	// 敵のチーム
	public TeamObject Enemies;

	// デバッグ用の描画コンポーネント
	private PitchHeatmapVisualizer DebugVisualizer;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Debug.LogWarning("PitchEvaluatorのインスタンスが既に存在します。新しいインスタンスを破棄します。");
			Destroy(this.gameObject);
			return;
		}
		// インスタンスを設定
		// シーンを跨いだら消える
		Instance = this;
	}
	void Start()
	{
		Players.gridScores = new float[gridX, gridZ];
		Players.teamType = TeamType.Player;


		Enemies.gridScores = new float[gridX, gridZ];
		Enemies.teamType = TeamType.Enemy;

		DebugVisualizer = gameObject.AddComponent<PitchHeatmapVisualizer>();
		DebugVisualizer.Initialize(pitchSize, gridX, gridZ);
	}

	void Update()
	{
		EvaluatePitch();

		// デバッグ
		DebugVisualizer.UpdateScores(Players.gridScores);
	}

	// 全セルのスコアを計算・更新
	void EvaluatePitch()
	{
		float cellWidth = pitchSize.x / gridX;
		float cellHeight = pitchSize.y / gridZ;

		int[] playersMaxScoreIndex = new int[2];
		int[] enemiesMaxScoreIndex = new int[2];
		float playersMaxScore = -1f;
		float enemiesMaxScore = -1f;

		for (int x = 0; x < gridX; x++)
		{
			for (int z = 0; z < gridZ; z++)
			{
				// セルの中央ワールド座標を計算
				Vector3 cellPos = GetCellWorldPosition(x, z, cellWidth, cellHeight);

				// プレイヤーチームのスコア計算
				Players.gridScores[x, z] = CalculateCellScore(cellPos, Players.teamType);
				if (Players.gridScores[x, z] > playersMaxScore)
				{
					playersMaxScore = Players.gridScores[x, z];
					playersMaxScoreIndex[0] = x;
					playersMaxScoreIndex[1] = z;
				}

				// 敵チームのスコア計算
				Enemies.gridScores[x, z] = CalculateCellScore(cellPos, Enemies.teamType);
				if (Enemies.gridScores[x, z] > enemiesMaxScore)
				{
					enemiesMaxScore = Enemies.gridScores[x, z];
					enemiesMaxScoreIndex[0] = x;
					enemiesMaxScoreIndex[1] = z;
				}
			}
		}

		// 最適な位置を更新
		Players.bestPosition = GetCellWorldPosition(playersMaxScoreIndex[0], playersMaxScoreIndex[1], cellWidth, cellHeight);
		Enemies.bestPosition = GetCellWorldPosition(enemiesMaxScoreIndex[0], enemiesMaxScoreIndex[1], cellWidth, cellHeight);
	}

	float CalculateCellScore(Vector3 cellPos, TeamType teamType)
	{
		const float MaxDist = 10.0f;
		const float GoalWeight = 0.6f;
		const float SpaceWeight = 0.4f;

		// 敵対しているチームを元に計算する
		TeamObject HostileTeam = (teamType == TeamType.Player) ? Enemies : Players;

		// ゴールに近いほどハイスコア
		float distToGoal = Vector3.Distance(cellPos, HostileTeam.Goal.position);
		float goalScore = Mathf.Clamp01(1.0f - (distToGoal / pitchSize.y));

		// 周囲の敵が離れているほどハイスコア
		float nearestEnemyDist = float.MaxValue;
		foreach (var enemy in HostileTeam.characters)
		{
			float d = Vector3.Distance(cellPos, enemy.position);

			// 最も近い場所を更新
			if (d < nearestEnemyDist)
				nearestEnemyDist = d;
		}

		// 最大距離以上離れていれば満点
		float spaceScore = Mathf.Clamp01(nearestEnemyDist / MaxDist);

		// 重み付けして合算
		return (goalScore * GoalWeight) + (spaceScore * SpaceWeight);
	}

	// 最もスコアが高いマスのワールド座標を取得する
	public Vector3 GetBestPosition(TeamType teamType)
	{
		// 敵対しているチームを元に計算する
		TeamObject Team = (teamType == TeamType.Player) ? Players : Enemies;

		return Team.bestPosition;
	}

	public Vector3 CuluBestPosition(TeamType teamType, Vector3 Position)
	{
		// 敵対しているチームを元に計算する
		TeamObject Team = (teamType == TeamType.Player) ? Players : Enemies;

		// 引数の座標からグリッドの番号を取得
		int TargetX, TargetZ;
		GetCellIndexByWorldPosition(Position, out TargetX, out TargetZ);

		float MaxScore = 0.0f;
		Vector3 BestPosition = Vector3.zero;

		for(int x = 0; x < gridX; x++)
		{
			for (int z = 0; z < gridZ; z++)
			{
				// 対象のグリッドとの距離
				int distX = Math.Abs(TargetX - x);
				int distZ = Math.Abs(TargetZ - z);

				// 距離が近いほどスコアを高くする
			}
		}

		return BestPosition;
	}

	Vector3 GetCellWorldPosition(int x, int z, float w, float h)
	{
		float worldX = (x * w) - (pitchSize.x / 2f) + (w / 2f);
		float worldZ = (z * h) - (pitchSize.y / 2f) + (h / 2f);
		return new Vector3(worldX, 0, worldZ);
	}
	void GetCellIndexByWorldPosition(Vector3 worldPosition, out int x, out int z)
	{
		float cellWidth = pitchSize.x / gridX;
		float cellHeight = pitchSize.y / gridZ;
		x = Mathf.Clamp(Mathf.FloorToInt((worldPosition.x + (pitchSize.x / 2f)) / cellWidth), 0, gridX - 1);
		z = Mathf.Clamp(Mathf.FloorToInt((worldPosition.z + (pitchSize.y / 2f)) / cellHeight), 0, gridZ - 1);
	}
}
