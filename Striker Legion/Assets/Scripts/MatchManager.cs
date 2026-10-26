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

	//　ゲームバール
	[SerializeField] GameBall gameBall;

	// ゲームクロック
    float gametimer;

	//　スコアボード
    private Dictionary<CharacterTeam, int> scores;

	public Dictionary<CharacterTeam, int> Score => scores;

    // ボールを持っているキャラをマネージャーで集約して保持すると超軽量化できる
    //private CharacterBase currentBallHolder;
    public CharacterBase CurrentBallHolder => gameBall.GetOwner() as CharacterBase;

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

	private void Start()
	{
        //UpdateBallHolder();
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

		SkillBar.instance = new SkillBar();

        SkillBar.instance.RegisterTeam(CharacterTeam.Player);
        SkillBar.instance.RegisterTeam(CharacterTeam.Enemy);

		scores = new Dictionary<CharacterTeam, int>();
		scores[CharacterTeam.Player] = 0;
		scores[CharacterTeam.Enemy]	= 0;
    }

	void Update()
	{
		// ボールが誰が持っているか検索する
		// UpdateBallHolder();
	}

	// ゲームボールのGetOwnerに変更
	//
	//// ボールを持っているキャラクターを検索する
	//void UpdateBallHolder()
	//{
	//	if (PlayerTeam == null || EnemyTeam == null)
	//		return;

	//	if (PlayerTeam.teamObjects == null || EnemyTeam.teamObjects == null)
	//		return;

	//	//if(currentBallHolder)
	//	//Debug.Log("今ボールを持っているキャラのチーム番号:"+currentBallHolder.team);

	//	foreach (CharacterBase c in PlayerTeam.teamObjects)
	//	{
	//		if (!c.hasBall)
	//			continue;

	//		if (currentBallHolder == c)
	//			return;

	//		Debug.Log("Playerチームがボールを持った");

	//		currentBallHolder = c;
	//		return;
	//	}
	//	foreach (CharacterBase c in EnemyTeam.teamObjects)
	//	{
	//		if (!c.hasBall)
	//			continue;

	//		if (currentBallHolder == c)
	//			return;
	//		Debug.Log("Enemyチームがボールを持った");
	//		currentBallHolder = c;
	//		return;
	//	}
	//	//Debug.Log("だれもボールを持っていない");
	//}

	// ゴールした際の処理
	public void GoalEvent(CharacterTeam team)
	{
		Debug.Log(team + "側のゴールにボールが入りました。");

		scores[team] += 1;

		// エネルギーを与える
		SkillBar.instance.GainForShoot(team);

		// キャラクターの座標などをリセットする
		ResetForKickoff(team);
	}

	// キックオフのために状況をリセットする
	private void ResetForKickoff(CharacterTeam team)
	{
		PlayerTeam.ResetInit();
		EnemyTeam.ResetInit();

		// プレイヤー側のゴールに入った場合
		if(team == CharacterTeam.Player)
		{
			EnemyTeam.OrderLoseBall();	// 点をいれた側からボールを没収する
			PlayerTeam.OrderGetBall();	// 点をいれられた側にボールを渡す。
		}
		else
		{
			PlayerTeam.OrderLoseBall();
			EnemyTeam.OrderGetBall();
		}
	}
	public CharacterBase GetAllyByRole(int teamNumber, CharacterRole role)
	{
		return GetAllyByRole((CharacterTeam)teamNumber, role);
	}

	public CharacterBase GetAllyByRole(CharacterTeam team, CharacterRole role)
	{
#if false
		// 味方のオブジェクトを取得する
		foreach (CharacterBase ally in GetTeamManager(team).teamObjects)
		{
			// キャラクターのコンポーネントをヌルチェック
			if (ally == null)
				continue;

			// ロールが指定したものかチェックする
			if (ally.CharacterRole == role)
				return ally;

		}
#endif
		return null;
	}

	public IReadOnlyList<CharacterBase> GetActiveCharacters(CharacterTeam team)
	{
		return GetTeamManager(team).GetActiveCharacters();
	}

	public Vector3 GetGoalPosition(int teamNumber)
	{
		return GetGoalPosition((CharacterTeam)teamNumber);
	}
	public Vector3 GetGoalPosition(CharacterTeam team)
	{
		return (team == CharacterTeam.Player) ? PlayerGoal.position : EnemyGoal.position;
	}

	private TeamManager GetTeamManager(CharacterTeam team)
	{
		if (team == PlayerTeam.GetTeamType())
			return PlayerTeam;
		else
			return EnemyTeam;
	}

	// GUIのデバッグ表示
	public void DebugGUI()
	{
		PlayerTeam.DebugGUI();
		EnemyTeam.DebugGUI();
	}
}
