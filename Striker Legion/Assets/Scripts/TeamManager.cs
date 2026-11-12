using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Behavior;
using Unity.VisualScripting;
using UnityEngine;

[BlackboardEnum]
public enum PositioningName
{
	Offense,
	OffensiveMiddle,
	DefensiveMiddle,
	Defense,
}

public class TeamManager : MonoBehaviour
{
	// ポジショニングの基準となるTransformを格納する
	[SerializeField] public Dictionary<PositioningName, Transform> positioningBaseTransforms = new Dictionary<PositioningName, Transform>();
	//[SerializeField] public Dictionary<CharacterRole, Transform> positioningBaseTransforms = new Dictionary<CharacterRole, Transform>();

	[SerializeField] private CharacterTeam teamType;
	public CharacterTeam TeamType => teamType;

	// 同じチームのオブジェクトを格納する
	public List<CharacterBase> teamObjects { get; private set; }

	private void Awake()
	{
		//// マッチマネージャに登録する
		//MatchManager.Instance.RegisterTeam(this);

		//Debug.Log(teamType + "：チーム数=" + teamObjects.Count);
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
#if false
		if (teamObjects.Count <= 0)
		{
			Debug.Log("チームの取得に失敗");
			return;
		}
		int count = 0;

		Dictionary<PositioningName, Vector3> positioningBasePositions = new Dictionary<PositioningName, Vector3>();
		foreach (PositioningName n in positioningBaseTransforms.Keys)
		{
			positioningBasePositions.Add(n, positioningBaseTransforms[n].position);
		}

		foreach (CharacterBase child in teamObjects)
		{
			// AIコンポーネントの取得
			if (!child.TryGetComponent<CharacterAIAgent>(out CharacterAIAgent agent))
			{
				count++;
				continue;
			}
			// キャラクターに設定するロール
			CharacterRole role = Enum.IsDefined(typeof(CharacterRole), count) ? (CharacterRole)count : CharacterRole.Forward;

			// ポジショニングの基準座標
			agent.Initialize(positioningBasePositions, role);
			//agent.Initialize(positioningBaseTransforms[role].position, role);

			count++;
			Debug.Log(gameObject.name + " Child Object: " + child.name + "ロール:" + role);
		}

		UpdateAllBehaviorGraph();
#endif
		
	}

	public void Init()
	{
		// LINQを使って取得
		teamObjects = transform.Cast<Transform>().Select(t => t.gameObject.GetComponent<CharacterBase>()).ToList();

		if (teamObjects.Count <= 0)
		{
			Debug.Log("チームの取得に失敗");
			return;
		}
		int count = 0;

		Dictionary<PositioningName, Vector3> positioningBasePositions = new Dictionary<PositioningName, Vector3>();
		foreach (PositioningName n in positioningBaseTransforms.Keys)
		{
			positioningBasePositions.Add(n, positioningBaseTransforms[n].position);
		}

		foreach (CharacterBase child in teamObjects)
		{
			// AIコンポーネントの取得
			if (!child.TryGetComponent<CharacterAIAgent>(out CharacterAIAgent agent))
			{
				count++;
				continue;
			}
			// キャラクターに設定するロール
			CharacterRole role = Enum.IsDefined(typeof(CharacterRole), count) ? (CharacterRole)count : CharacterRole.Forward;

			// ポジショニングの基準座標
			agent.Initialize(positioningBasePositions, role);
			//agent.Initialize(positioningBaseTransforms[role].position, role);

			count++;
			Debug.Log(gameObject.name + " Child Object: " + child.name + "ロール:" + role + "座標" + child.transform.position);
		}

		UpdateAllBehaviorGraph();
	}

	// Update is called once per frame
	void Update()
    {
		UpdateAllBehaviorGraph();
	}

	private void UpdateAllBehaviorGraph()
	{
#if false
		bool isAnyHoldingBall = teamObjects.Any(obj => obj.hasBall);
		// チーム内の全てのオブジェクトに対して、BehaviorGraphAgentのチームがボールを保持しているかの変数を更新
		foreach (CharacterBase obj in teamObjects)
		{
			obj.BehaviorGraphAgent.SetVariableValue("TeamHavingBall", isAnyHoldingBall);
		}
#endif
	}

	public void ResetInit()
	{
		//Debug.Log("リセット！！！！！！！！！！！１");
		SpawnerLinkage[] respawners = GameObject.FindGameObjectWithTag("Respawn").GetComponentsInChildren<SpawnerLinkage>();
		foreach (SpawnerLinkage respawner in respawners) {
			respawner.respawn(); 
			//Debug.Log(respawner.character.name+ ":"+respawner.character.transform.position);
		}

		// 座標の変更を同期
		Physics.SyncTransforms();

#if false
		foreach (SampleCharacter child in teamObjects)
		{
			child.ResetInit();
		}
#endif
	}

	// チームからボールを奪う命令
	public void OrderLoseBall(GameBall ball)
	{
#if false
		foreach (CharacterBase child in teamObjects)
		{
			child.hasBall = false;
		}
#endif
		
	}

	// チームにボールを与える命令
	public void OrderGetBall(GameBall ball)
	{
		ball.SetOwner(teamObjects[0]);
		teamObjects[0].GetBall();
	}

	// スタン状態じゃないキャラを取得する（読み取り専用）
	public IReadOnlyList<CharacterBase> GetActiveCharacters()
	{
		List<CharacterBase> activeCharacters = new List<CharacterBase>();
		foreach(CharacterBase character in teamObjects)
		{
			if (!character.isDown)
				activeCharacters.Add(character);
		}

		return activeCharacters;
	}

	// GUIのデバッグ表示(MatchManagerで呼び出し)
	public void DebugGUI()
	{
		//foreach (CharacterBase child in teamObjects)
		//{
		//	child.DebugGUI();
		//}

	}
}
