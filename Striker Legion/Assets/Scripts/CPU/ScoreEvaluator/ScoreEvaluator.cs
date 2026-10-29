//using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;

public struct EvaluationContext
{
	public EvaluationContext(int cols, int rows, Vector2 gridSize, Vector2 totalSize, Vector3 gridOrigin)
	{
		gridCols = cols;
		gridRows = rows;
		this.gridOrigin = gridOrigin;
		this.gridSize = gridSize;
		this.totalSize = totalSize;
	}

	private int gridCols; // 横のマス数

	private int gridRows; // 縦のマス数

	private Vector3 gridOrigin; // グリッドの起点（左下などのワールド座標）
	private Vector2 gridSize;
	private Vector2 totalSize;

	public int GridCols => gridCols; // 横のマス数
	public int GridRows => gridRows; // 縦のマス数
	public Vector3 GridOrigin => gridOrigin; // グリッドの起点（左下などのワールド座標）
	public Vector2 GridSize => gridSize;
	public Vector2 TotalSize => totalSize;
}

[CreateAssetMenu(fileName = "./CPU/ScoreEvaluator/ScoreEvaluator", menuName = "Evaluators/ScoreEvaluatorBase")]
public abstract class ScoreEvaluatorBase:ScriptableObject
{
	// それぞれのポジションのウェイト
	[SerializeField, Range(0.0f, 1.0f)]
	protected float weight;

	// グリッドのスコアを計算して返す
	public abstract float CalculateScore(Vector3 worldPos, CharacterTeam team, in IReadOnlyList<PitchCharacter> pitchCharacters,EvaluationContext context);
}