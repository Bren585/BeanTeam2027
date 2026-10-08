using System;
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
		Instance = this;
	}

	void Start()
	{
		// ゴールオブジェクトを検索する
		GameObject[] goalObjects = GameObject.FindGameObjectsWithTag("Goal");
		
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
}
