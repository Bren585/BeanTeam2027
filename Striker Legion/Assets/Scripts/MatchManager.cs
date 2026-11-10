using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class MatchManager : MonoBehaviour
{
	public static MatchManager Instance { get; private set; }

	// プレイヤー陣営のチーム
	private TeamManager PlayerTeam;
	// 敵陣営のチーム
	private TeamManager EnemyTeam;

	[SerializeField] private TeamHandoff teamHandoff;
	[SerializeField] private GameObject characterPrefab;

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

	private PitchEvaluator pitchEvaluator;

	public Transform BallTransform => gameBall.transform;

	private void Awake()
	{
		// シングルトンのチェック
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;

		InitializeTeams();
        // 初期化処理
        Initialize();
		ResetForKickoff(CharacterTeam.Player);
	}

	private void Start()
	{
		//UpdateBallHolder();
		SkillBar.instance.RegisterTeam(CharacterTeam.Player);
		SkillBar.instance.RegisterTeam(CharacterTeam.Enemy);
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
			if (goalObject.name.StartsWith("Player"))
			//if (LayerMask.LayerToName(goalObject.layer) == "Player")
				PlayerGoal = goalObject.transform;
			else
				EnemyGoal = goalObject.transform;
		}

		//BallTransform = GameObject.FindGameObjectWithTag("Ball").transform;

		// このオブジェクトについているコンポーネントを保存
		pitchEvaluator = gameObject.GetComponentInChildren<PitchEvaluator>();

		//SkillBar.instance = new SkillBar();

        

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

	void InitializeTeams()
	{
		if (PlayerTeam == null)
		{
			Debug.Log("プレイヤーチームが未登録");
			return;
		}
		if(EnemyTeam == null)
		{
			Debug.Log("エネミーチームが未登録");
			return;
		}
		if (PlayerTeam.teamObjects == null || EnemyTeam.teamObjects == null)
			return;


        SpawnerLinkage[] player_respawners = GameObject.FindGameObjectWithTag("Respawn").transform.Find("Player").GetComponentsInChildren<SpawnerLinkage>();
        SpawnerLinkage[] enemy_respawners = GameObject.FindGameObjectWithTag("Respawn").transform.Find("Enemy").GetComponentsInChildren<SpawnerLinkage>();
        foreach (Character_Position role in Enum.GetValues(typeof(Character_Position)))
		{
			// Player Team
			if (teamHandoff.playerTeam.TryGetValue(role, out CharacterInfo playerInfo))
			{
				GameObject newCharacter = Instantiate(characterPrefab);
				newCharacter.transform.SetParent(PlayerTeam.transform);
				newCharacter.GetComponent<CharacterBase>().setInfo(playerInfo);
				foreach (SpawnerLinkage spawner in player_respawners)
				{
					if ((int)(spawner.Role) == (int)(role))
					{
						spawner.character = newCharacter.GetComponent<CharacterBase>();
						break;
					}
				}
			}
			else
			{
				switch (role)
				{
					case Character_Position.None:
					case Character_Position.Other:
						break;
					default:
						Debug.Log("Missing Team Member!!!");
						throw new Exception("Team Creation Failed");
				}
			}

            // Enemy Team
            
            if (teamHandoff.enemyTeam.TryGetValue(role, out CharacterInfo enemyInfo))
            {
                GameObject newCharacter = Instantiate(characterPrefab);
                newCharacter.transform.SetParent(EnemyTeam.transform);
                newCharacter.GetComponent<CharacterBase>().setInfo(enemyInfo);
                foreach (SpawnerLinkage spawner in enemy_respawners)
                {
                    if ((int)(spawner.Role) == (int)(role))
                    {
                        spawner.character = newCharacter.GetComponent<CharacterBase>();
                        break;
                    }
                }
            }
            else
            {
                switch (role)
                {
                    case Character_Position.None:
                    case Character_Position.Other:
                        break;
                    default:
                        Debug.Log("Missing Team Member!!!");
                        throw new Exception("Team Creation Failed");
                }
            }
        }
	}

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
		if (team == PlayerTeam.TeamType)
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

	public void RegisterTeam(TeamManager team)
	{
		if(team.TeamType == CharacterTeam.Player)
		{
			// チームが登録済みなら終了
			if (PlayerTeam != null)
			{
				Debug.Log("プレイヤーチームは登録済み");
				return;
			}
			PlayerTeam = team;
			PitchEvaluatorRegister(team.teamObjects);
		}
		else
		{
			// チームが登録済みなら終了
			if (EnemyTeam != null)
			{
				Debug.Log("エネミーチームは登録済み");
				return;
			}
			EnemyTeam = team;
			PitchEvaluatorRegister(team.teamObjects);
		}
	}

	// ------------------------ ピッチグリッド計算クラスの呼び出し関数 --------------------------
	void PitchEvaluatorRegister(List<CharacterBase> characters)
	{
		if (pitchEvaluator != null)
		{
			pitchEvaluator.RegisterCharacter(characters);
		}
		else
			Debug.Log("評価用コンポーネントがNull");
	}

	public void RegisterEvaluatorScore(CharacterPitchScore score)
	{
		if (pitchEvaluator != null)
		{
			pitchEvaluator.RegisterPitchScore(score);
		}
	}
}
