using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
	// ポジショニングの基準となるTransformを格納する
	[SerializeField] public Dictionary<CharacterRole, Transform> positioningBaseTransforms = new Dictionary<CharacterRole, Transform>();

	[SerializeField] private CharacterTeam TeamType;

	// 同じチームのオブジェクトを格納する
	public List<CharacterBase> teamObjects { get; private set; }

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		// LINQを使って GameObject の配列として1行で取得
		teamObjects = transform.Cast<Transform>().Select(t => t.gameObject.GetComponent<CharacterBase>()).ToList();

		if (teamObjects.Count <= 0)
		{
			Debug.Log("チームの取得に失敗");
			return;
		}
		int count = 0;
		foreach(CharacterBase child in teamObjects)
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
			agent.Initialize(positioningBaseTransforms[role].position, role);

			count++;
			Debug.Log(gameObject.name + " Child Object: " + child.name + "ロール:" + role);
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
#if false
		foreach (SampleCharacter child in teamObjects)
		{
			child.ResetInit();
		}
#endif
	}

	// チームからボールを奪う命令
	public void OrderLoseBall()
	{
#if false
		foreach (SampleCharacter child in teamObjects)
		{
			child.isHoldingBall = false;
		}
#endif
	}

	// チームにボールを与える命令
	public void OrderGetBall()
	{
		//teamObjects[0].isHoldingBall = true;
	}

	public CharacterTeam GetTeamType()
	{
		return TeamType;
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
