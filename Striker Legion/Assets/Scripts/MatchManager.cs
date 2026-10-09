using System;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
	public static MatchManager Instance { get; private set; }

	// プレイヤー陣営のチーム
	[SerializeField] private TeamManager PlayerTeam;
	// 敵陣営のチーム
	[SerializeField] private TeamManager EnemyTeam;

	// プレイヤー側のゴール
	public Transform PlayerGoal { get; private set; }

	// 敵側のゴール
	public Transform EnemyGoal { get; private set; }

	public Transform BallTransform { get; private set; }

	// ボールを持っているキャラをマネージャーで集約して保持すると超軽量化できる
	public SampleCharacter CurrentBallHolder { get; private set; }

	private void Awake()
	{
		// シングルトンのチェック
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;

		// 初期化処理
		Initialize();
	}

	void Initialize()
	{
		Debug.Log("マッチマネージャーの初期化");
		// ゴールオブジェクトを検索する
		GameObject[] goalObjects = GameObject.FindGameObjectsWithTag("Goal");

		if (goalObjects.Length <= 0)
			Debug.Log("ゴールオブジェクトの取得に失敗");

		// ゴールの姿勢情報を保存する
		foreach(GameObject goalObject in goalObjects)
		{
			if (LayerMask.LayerToName(goalObject.layer) == "Player")
				PlayerGoal = goalObject.transform;
			else
				EnemyGoal = goalObject.transform;
		}

		BallTransform = GameObject.FindGameObjectWithTag("Ball").transform;
	}

	void Update()
	{
		// ボールが誰が持っているか検索する
		UpdateBallHolder();
	}

	void UpdateBallHolder()
	{
		foreach (SampleCharacter c in PlayerTeam.teamObjects)
		{
			if (!c.isHoldingBall)
				continue;

			CurrentBallHolder = c;
			return;
		}
		foreach (SampleCharacter c in EnemyTeam.teamObjects)
		{
			if (!c.isHoldingBall)
				continue;

			CurrentBallHolder = c;
			return;
		}
	}

	public SampleCharacter GetAllyByRole(CharacterTeam team, CharacterRole role)
	{
		// 味方のオブジェクトを取得する
		foreach (SampleCharacter ally in GetTeamManager(team).teamObjects)
		{
			// キャラクターのコンポーネントをヌルチェック
			if (ally == null)
				continue;

			// ロールが指定したものかチェックする
			if (ally.CharacterRole == role)
				return ally;

		}
		return null;
	}
	public IReadOnlyList<SampleCharacter> GetActiveCharacters(CharacterTeam team)
	{
		return GetTeamManager(team).GetActiveCharacters();
	}

	private TeamManager GetTeamManager(CharacterTeam team)
	{
		if (team == PlayerTeam.GetTeamType())
			return PlayerTeam;
		else
			return EnemyTeam;
	}

}
