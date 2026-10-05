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
}


// ピッチの状態を評価する
public class PitchEvaluator : MonoBehaviour
{
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

		for (int x = 0; x < gridX; x++)
		{
			for (int z = 0; z < gridZ; z++)
			{
				// セルの中央ワールド座標を計算
				Vector3 cellPos = GetCellWorldPosition(x, z, cellWidth, cellHeight);

				// プレイヤーチームのスコア計算
				Players.gridScores[x, z] = CalculateCellScore(cellPos, Players.teamType);

				// 敵チームのスコア計算
				Enemies.gridScores[x, z] = CalculateCellScore(cellPos, Enemies.teamType);
			}
		}
	}

	float CalculateCellScore(Vector3 cellPos, TeamType teamType)
	{
		// 敵対しているチームを元に計算する
		TeamObject HostileTeam = (teamType == TeamType.Player) ? Enemies : Players; 

		// ゴールへの近さ（ゴールに近いほどハイスコア）
		float distToGoal = Vector3.Distance(cellPos, HostileTeam.Goal.position);
		float goalScore = Mathf.Clamp01(1.0f - (distToGoal / pitchSize.y));

		// 敵との距離（周囲の敵が離れているほどハイスコア）
		float nearestEnemyDist = float.MaxValue;
		foreach (var enemy in HostileTeam.characters)
		{
			float d = Vector3.Distance(cellPos, enemy.position);
			if (d < nearestEnemyDist) nearestEnemyDist = d;
		}
		float spaceScore = Mathf.Clamp01(nearestEnemyDist / 10f); // 10m以上離れていれば満点

		// 重み付けして合算
		return (goalScore * 0.6f) + (spaceScore * 0.4f);
	}

	// 最もスコアが高いマスのワールド座標を取得する
	public Vector3 GetBestPosition(TeamType teamType)
	{
		// 敵対しているチームを元に計算する
		TeamObject Team = (teamType == TeamType.Player) ? Players : Enemies;

		float maxScore = -1f;
		Vector3 bestPos = Vector3.zero;
		float cellWidth = pitchSize.x / gridX;
		float cellHeight = pitchSize.y / gridZ;

		for (int x = 0; x < gridX; x++)
		{
			for (int z = 0; z < gridZ; z++)
			{
				if (Team.gridScores[x, z] > maxScore)
				{
					maxScore = Team.gridScores[x, z];
					bestPos = GetCellWorldPosition(x, z, cellWidth, cellHeight);
				}
			}
		}
		return bestPos;
	}

	Vector3 GetCellWorldPosition(int x, int z, float w, float h)
	{
		float worldX = (x * w) - (pitchSize.x / 2f) + (w / 2f);
		float worldZ = (z * h) - (pitchSize.y / 2f) + (h / 2f);
		return new Vector3(worldX, 0, worldZ);
	}

	// デバッグ表示用（Sceneビューで各マスの評価値を可視化）
	//void OnDrawGizmosSelected()
	//{
	//	if (gridScores == null) return;
	//	float cellWidth = pitchSize.x / gridX;
	//	float cellHeight = pitchSize.y / gridZ;

	//	for (int x = 0; x < gridX; x++)
	//	{
	//		for (int z = 0; z < gridZ; z++)
	//		{
	//			Vector3 pos = GetCellWorldPosition(x, z, cellWidth, cellHeight);
	//			float score = gridScores[x, z];
	//			Gizmos.color = Color.Lerp(Color.red, Color.green, score);
	//			Gizmos.DrawWireCube(pos, new Vector3(cellWidth * 0.9f, 0.1f, cellHeight * 0.9f));
	//		}
	//	}
	//}
}
