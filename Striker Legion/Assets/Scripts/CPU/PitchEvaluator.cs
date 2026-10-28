using System;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public struct PitchCharacter
{
	// インストラクタ
	public PitchCharacter(CharacterTeam characterTeam, Transform t){
		team = characterTeam;
		transform = t;
	}

	// 所属チーム
	public CharacterTeam team { get; private set; }
	// 姿勢情報
	public Transform transform { get; private set; }
}

// ピッチの状態を評価する
public class PitchEvaluator : MonoBehaviour
{
	[Header("Gizmo Menu")]
	public bool isDrawGizmo = false;

	[Range(0.0f, 1.0f)]
	public float colorAlpha = 0.5f;

	public CharacterRole debugRole = CharacterRole.Forward;

	public CharacterTeam debugTeam = CharacterTeam.Player;

	[Header("Grid Settings")]
	[SerializeField] private MeshFilter pitchPlaneMeshFilter; // ピッチを表すPlaneのMeshFilter
	//[SerializeField] private float cellSize = 1.0f;           // 1マスの大きさ（例: 1メートル）

	// 計算されたグリッドのデータ
	private int gridCols; // 横のマス数
	private int gridRows; // 縦のマス数

	private Vector3 gridOrigin; // グリッドの起点（左下などのワールド座標）

	//[Header("Grid Settings")]
	public Vector2 gridSize = new Vector2(10f, 20f);

	[Range(0.0f, 1.0f)]
	public float goalWeight = 0.6f;
	[Range(0.0f, 1.0f)]
	public float pressureWeight = 0.4f;
	[Range(0.0f, 1.0f)]
	public float ballWeight = 0.3f;
	[Range(0.0f, 1.0f)]
	public float frontWeight = 0.3f;
	//[Range(0.0f, 1.0f)]
	//public float allyWeight = 0.3f;
	[Range(0.0f, 1.0f)]
	public float passWeight = 0.4f;

	// 更新処理を行う時間
	[Min(0.0f)]
	public float updateTime = 0.1f;

	// 更新処理をする時間を数える変数
	private float timer = 0.0f;


	// ピッチ上キャラクター
	private List<PitchCharacter> characters;

	// キャラ側のピッチスコア
	private List<CharacterPitchScore> pitchScores;

	// スコアの計算用インターフェイス
	public List<ScoreEvaluatorBase> scoreEvaluators;

	// プレイヤーチームのグリッド
	float[,] playersGrid;

	// 敵チームのグリッド
	float[,] enemiesGrid;

	// 値を渡す時のための配列
	float[,] cashedGrids;

	Vector2 totalSize = Vector2.zero;

	// デバッグ用の描画コンポーネント
	//private PitchHeatmapVisualizer DebugVisualizer;


	// グリッドの行と列の取得
	public int GetGridCols => gridCols;
	public int GetGridRows => gridRows;

	private void Awake()
	{
		// メモリを確保
		characters = new List<PitchCharacter>();
		pitchScores = new List<CharacterPitchScore>();
		//scoreEvaluators = new List<ScoreEvaluatorBase>();
		// 平面からグリッドを作るための情報取得
		InitializeGridFromPlane();
	}
	void Start()
	{
		playersGrid = new float[gridCols, gridRows];

		enemiesGrid = new float[gridCols, gridRows];
		cashedGrids = new float[gridCols, gridRows];

		// 初期更新は早めにする
		timer = updateTime;

		//scoreEvaluators.Add(Crea ScoreDistToGoal());
		//scoreEvaluators.Add(new ScoreOpponentPressure());

		Debug.Log("ScoreEvaluators count:" + scoreEvaluators.Count);
	}

	// 初期化処理（マッチマネージャで呼び出す）
	public void RegisterCharacter(List<CharacterBase> characterBases)
	{
		// プレイヤーチームの追加
		foreach (CharacterBase p in characterBases)
		{
			CharacterTeam team = (p.teamNo == (int)CharacterTeam.Player) ? CharacterTeam.Player : CharacterTeam.Enemy;
			PitchCharacter pitchCharacter = new PitchCharacter(team, p.transform);
			this.characters.Add(pitchCharacter);
		}
	}

	public void RegisterPitchScore(CharacterPitchScore score)
	{
		// キャラ側のピッチコンポーネントを追加
		pitchScores.Add(score);

		// グリッドの構成情報を渡す
		score.SetPitchInfo(gridCols, gridRows, gridSize, gridOrigin);
	}

	void Update()
	{
		// タイマーが指定時間を超えたら更新処理を行うようにする
		if (!UpdateTimer())
			return;

		EvaluatePitch();
	}

	private bool UpdateTimer()
	{
		// 指定時間を越えなければ、falseを返す
		if (timer < updateTime)
		{
			timer += Time.deltaTime;
			return false;
		}
		else
		{
			// タイマーリセット
			timer = 0.0f;
		}
		return true;
	}

	private void InitializeGridFromPlane()
	{
		if (pitchPlaneMeshFilter == null)
		{
			Debug.LogError("Pitch plane mesh filter is not assigned!");
			return;
		}

		// 1. メッシュのローカルサイズと、オブジェクトのスケールを掛け合わせて実際のワールドサイズを算出する
		Bounds localBounds = pitchPlaneMeshFilter.sharedMesh.bounds;
		Vector3 worldScale = pitchPlaneMeshFilter.transform.lossyScale;

		// サッカーピッチの広さを XZ平面（横×奥行き）として計算
		totalSize.x = localBounds.size.x * worldScale.x;
		totalSize.y = localBounds.size.z * worldScale.z;

		// 2. 総サイズとセルサイズから、マスの総数を計算する
		gridCols = Mathf.RoundToInt(totalSize.x / gridSize.x);
		gridRows = Mathf.RoundToInt(totalSize.y / gridSize.y);

		// 3. Planeの中心位置から、グリッドの「左下（起点）」のワールド座標を計算する
		// ※Unityの標準Planeは中心が(0,0,0)なので、幅の半分を引くことで左下を割り出す
		Vector3 planeCenter = pitchPlaneMeshFilter.transform.position;
		gridOrigin = planeCenter - (pitchPlaneMeshFilter.transform.right * (totalSize.x / 2f))
								 - (pitchPlaneMeshFilter.transform.forward * (totalSize.y / 2f));

		Debug.Log($"グリッドの初期化結果: Cols={gridCols}, Rows={gridRows}, Origin={gridOrigin}");
	}

	// 全セルのスコアを計算・更新
	void EvaluatePitch()
	{
#if true
		EvaluationContext context = new EvaluationContext(gridCols, gridRows, gridSize, totalSize, gridOrigin);
		foreach (CharacterPitchScore s in pitchScores)
		{
			for (int x = 0; x < gridCols; x++)
			{
				for (int z = 0; z < gridRows; z++)
				{
					float score = 0.0f;
					foreach (ScoreEvaluatorBase evaluator in scoreEvaluators)
					{
						// セルの中央ワールド座標を計算
						Vector3 cellPos = GetCellWorldPosition(x, z);

						score += evaluator.CalculateScore(cellPos, s.OwnerTeam, s.OwnerRole, characters, context);
					}
					s.RefreshEvaluation(score, x, z);
					//Debug.Log($"x:{x},y:{z},score{score}");
				}
			} 
		}

#else
		for (int x = 0; x < gridCols; x++)
		{
			for (int z = 0; z < gridRows; z++)
			{
				// セルの中央ワールド座標を計算
				Vector3 cellPos = GetCellWorldPosition(x, z);

				// プレイヤーチームのスコア計算
				playersGrid[x, z] = CalculateCellScore(cellPos, CharacterTeam.Player);

				// 敵チームのスコア計算
				enemiesGrid[x, z] = CalculateCellScore(cellPos, CharacterTeam.Enemy);
			}
		}
		// 計算結果をキャラ側に渡す
		foreach (CharacterPitchScore s in pitchScores)
		{
			for (int x = 0; x < gridCols; x++)
			{
				for (int z = 0; z < gridRows; z++)
				{
					float score = (s.OwnerTeam == CharacterTeam.Player) ? playersGrid[x, z] : enemiesGrid[x, z];
					score = ScoreByRole(s.OwnerRole, score);
					s.RefreshEvaluation(score, x, z);
				}
			}
		}
#endif
	}

	float CalculateCellScore(Vector3 cellPos, CharacterTeam teamType)
	{
		// ゴールまでの距離によるスコア
		float goalScore = ScoreDistToGoal(cellPos, teamType, goalWeight);

		// 敵との距離によるスコア調整
		float pressureScore = CalculateOpponentPressure(cellPos, teamType, pressureWeight);

		// 味方との距離による加算
		//float allyScore = CalculateAllyDistance(cellPos, teamType, allyWeight);

		// ボールまでの距離によるスコア
		float ballScore = ScoreDistToBall(cellPos, teamType,ballWeight);
		// グリッドがボールを持っているキャラの正面にあったらスコアを加算する
		float frontScore = ScoreFrontToBaller(cellPos, frontWeight);

		// パス可能かのスコア
		float enablePassScore = ScoreEnablePass(cellPos, teamType, passWeight);

		// 合算
		return goalScore + pressureScore + ballScore + frontScore + enablePassScore;
	}

	private float ScoreDistToGoal(Vector3 cellPos, CharacterTeam team, float weight)
	{
		CharacterTeam opponentTeam = (team == CharacterTeam.Player) ? CharacterTeam.Enemy : CharacterTeam.Player;
		// ゴールまでの距離
		float distToGoal = Vector3.Distance(cellPos, MatchManager.Instance.GetGoalPosition(opponentTeam));
		// 逆数でゴールに近いほどスコアが高くなるようにする
		float goalScore = Mathf.Clamp01(1.0f - (distToGoal / totalSize.y));

		// ウェイト調整
		return goalScore * weight;
	}

	private float ScoreDistToBall(Vector3 cellPos, CharacterTeam team, float weight)
	{
		float score = 0.0f;
		float closerLength = gridSize.x;
		Vector3 ballPosition = MatchManager.Instance.BallTransform.position;
		// ボールまでの距離
		float distToBall = Vector3.Distance(cellPos, ballPosition);

		float minDistance = float.MaxValue;

		// ボールに最も近い敵のキャラクターの距離を取得
		foreach (var character in characters)
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
			score = Mathf.Clamp01(1.0f - (distToBall / totalSize.y));

		// ボールの近くに敵がいる場合は、スコアが高い場所がより高くなるようにする
		if (minDistance < gridSize.y)
			score = Easing.InOutQuad(score);

		// ウェイト調整
		return score * weight;
	}

	private float ScoreFrontToBaller(Vector3 cellPos, float weight)
	{
		// ボールを持っているキャラがいなければ、終了
		if (MatchManager.Instance.CurrentBallHolder == null)
			return 0.0f;

		Vector3 forward = MatchManager.Instance.CurrentBallHolder.transform.forward;
		Vector3 direction = cellPos - MatchManager.Instance.CurrentBallHolder.transform.position;

		float dot = Vector3.Dot(forward, direction.normalized);

		//// 正面方向にあったらウェイト分加算
		//if (dot > Mathf.PI / 5.0f)
		//	return weight * dot;
		return Mathf.Clamp01(dot * weight);
	}

	private float ScoreEnablePass(Vector3 cellPos, CharacterTeam team, float weight)
	{
		if (MatchManager.Instance.CurrentBallHolder == null)
			return 0.0f;
		// 所属チームで比較
		if (MatchManager.Instance.CurrentBallHolder.teamNo != (int)team)
			return 0.0f;

		Vector3 ballPos = MatchManager.Instance.CurrentBallHolder.transform.position;
		Vector3 direction = (cellPos - ballPos);
		//Vector3 direction = (ballPos - cellPos);

		// 同じチームのキャラを除外する
		LayerMask layer = ~(1 << MatchManager.Instance.CurrentBallHolder.gameObject.layer);

		float radius = gridSize.x / 2.0f;
		Ray ray = new Ray(ballPos, direction.normalized);
		if (Physics.SphereCast(ray, radius, out RaycastHit hitInfo, direction.magnitude, layer))
		{
			if (hitInfo.transform.tag == "Character")
				return 0.0f;
		}
		
		return weight;
	}

	private float ScoreDistToOpponent(Vector3 cellPos, CharacterTeam team, float weight, float maxDist)
	{
		// 周囲の敵が離れているほどハイスコア
		float nearestEnemyDist = float.MaxValue;
		foreach (var opponent in characters)
		{
			// 所属チームが同じならスキップ
			if (opponent.team == team)
				continue;
			// 距離計算
			float d = Vector3.Distance(cellPos, opponent.transform.position);

			// 最も近い場所を更新
			if (d < nearestEnemyDist)
				nearestEnemyDist = d;
		}


		// 最大距離以上離れていれば満点
		float spaceScore = Mathf.Clamp01(nearestEnemyDist / maxDist);

		// ウェイト調整
		return spaceScore * weight;
	}
	private float CalculateOpponentPressure(Vector3 cellPos, CharacterTeam team, float weight)
	{
		float pressure = 1.0f;
		float influenceRadius = gridSize.x + gridSize.y; // 影響を及ぼす半径

		// 登録されている全キャラクターから、
		// 相手チームのキャラを探して距離に応じたプレッシャーを加算
		foreach (var character in characters)
		{
			// 所属チームが同じならスキップ
			if (character.team == team)
				continue;

			// 距離測定
			float dist = Vector3.Distance(cellPos, character.transform.position);
			
			// 距離が影響範囲外ならスキップ
			if (dist > influenceRadius)
				continue;

			// 近ければ近いほど強いプレッシャー（逆数や減衰関数）
			pressure -= Mathf.Clamp01(1.0f - (dist / influenceRadius));
		}
		pressure = Mathf.Clamp01(pressure);
		return pressure * weight;
	}

	private float CalculateAllyDistance(Vector3 cellPos, CharacterTeam team, float weight)
	{
		float score = 0.0f;
		// 近すぎる場合は除外する範囲
		float closerRadius = gridSize.x + gridSize.y;
		// 影響を及ぼす半径
		float outerRadius = closerRadius * 2.0f;

		float minDistance = float.MaxValue;

		// 登録されている全キャラクターから、
		// 相手チームのキャラを探して距離に応じたプレッシャーを加算
		foreach (var character in characters)
		{
			// 所属チームが違うならスキップ
			if (character.team != team)
				continue;

			// 距離測定
			float dist = Vector3.Distance(cellPos, character.transform.position);

			// 距離が影響範囲外ならスキップ
			if (dist > outerRadius)
				continue;

			// 最も距離が近いキャラのものだけ使う
			if (minDistance < dist)
				continue;

			// 近すぎる場合には、逆にスコアが下がるようにする
			if(dist < closerRadius)
				score = Mathf.Clamp01(dist / closerRadius);
			
			else
				// 距離が適切であれば、スコアが高くなる
				score = Mathf.Clamp01(1.0f - (dist / outerRadius));

			minDistance = dist;
		}
		score = Mathf.Clamp01(score);
		return score * weight;
	}

	// ポジション毎にウェイト調整用
	private float ScoreByRole(CharacterRole role, float score)
	{
		score = Mathf.Clamp01(score);
		switch(role)
		{
			case CharacterRole.Forward:
				return Easing.InOutCubic(score);

			case CharacterRole.MidFielder:
				return Easing.InSine(score);

			case CharacterRole.Defender:
				return Easing.InSine(1.0f-score);
		}
		return score;
	}

	// 最もスコアが高いマスのワールド座標を取得する
	public Vector3 GetBestPosition(CharacterTeam teamType)
	{
		return Vector3.zero;
	}

	public Vector3 CuluBestPosition(CharacterTeam teamType, Vector3 Position)
	{
		return Vector3.zero;
	}

	Vector3 GetCellWorldPosition(int x, int z)
	{
		float worldX = gridOrigin.x + (x * gridSize.x) + (gridSize.x / 2.0f);
		float worldZ = gridOrigin.z + (z * gridSize.y) + (gridSize.y / 2.0f);
		//float worldX = (x * w) - (gridSize.x / 2f) + (w / 2f);
		//float worldZ = (z * h) - (gridSize.y / 2f) + (h / 2f);
		return new Vector3(worldX, 0, worldZ);
	}
	void GetCellIndexByWorldPosition(Vector3 worldPosition, out int x, out int z)
	{
		float cellWidth = gridSize.x / gridCols;
		float cellHeight = gridSize.y / gridRows;
		x = Mathf.Clamp(Mathf.FloorToInt((worldPosition.x + (gridSize.x / 2f)) / cellWidth), 0, gridCols - 1);
		z = Mathf.Clamp(Mathf.FloorToInt((worldPosition.z + (gridSize.y / 2f)) / cellHeight), 0, gridRows - 1);
	}

	void OnDrawGizmos()
	{
		if (!isDrawGizmo)
			return;

		Vector3 offset = new Vector3(gridSize.x / 2.0f, 0.0f, gridSize.y / 2.0f);
		Vector3 size = new Vector3(gridSize.x, 1.0f, gridSize.y);
		UnityEngine.Color color = UnityEngine.Color.white;
		color.a = colorAlpha;
		for (int z = 0; z < gridRows; z++)
		{
			//color.b = (float)z / (float)gridRows;
			for (int x = 0; x < gridCols; x++)
			{
				float score = ScoreByRole(debugRole, (debugTeam == CharacterTeam.Player)?playersGrid[x,z] : enemiesGrid[x, z]);
				//float score = ScoreByRole(debugRole, playersGrid[x, z]);
				color.r = score;
				color.g = score;
				color.b = score;
				Gizmos.color = color;
				
				Vector3 GridVec = new Vector3(x * gridSize.x, 0.0f, z * gridSize.y);
				Gizmos.DrawCube(GetCellWorldPosition(x,z), size);
				//Gizmos.DrawCube(gridOrigin + GridVec + offset, size);
				Gizmos.DrawSphere(GetCellWorldPosition(x, z), 1.0f);
				//Gizmos.DrawSphere(gridOrigin + GridVec + offset, 1.0f);
			}
		}

		Gizmos.DrawSphere(gridOrigin, 1.0f);

		if (characters==null)
			return;
		foreach(PitchCharacter p in characters)
		{
			Gizmos.color = (p.team == CharacterTeam.Player) ? UnityEngine.Color.blue : UnityEngine.Color.red; 
			Gizmos.DrawSphere(p.transform.position, 1.0f);
		}
	}
}
