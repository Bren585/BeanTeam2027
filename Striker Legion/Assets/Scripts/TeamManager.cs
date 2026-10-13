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
	public List<SampleCharacter> teamObjects { get; private set; }

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		// LINQを使って GameObject の配列として1行で取得
		teamObjects = transform.Cast<Transform>().Select(t => t.gameObject.GetComponent<SampleCharacter>()).ToList();

		int count = 0;
		// 子のオブジェクト名をログ出力
		foreach(SampleCharacter child in teamObjects)
		{
			// キャラクターに設定するロール
			CharacterRole role = Enum.IsDefined(typeof(CharacterRole), count) ? (CharacterRole)count : CharacterRole.Forward;

			// キャラクターのデータを初期化
			child.InitializeData(role, positioningBaseTransforms[role]);

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
		bool isAnyHoldingBall = teamObjects.Any(obj => obj.isHoldingBall);
		// チーム内の全てのオブジェクトに対して、BehaviorGraphAgentのチームがボールを保持しているかの変数を更新
		foreach (SampleCharacter obj in teamObjects)
		{
			obj.BehaviorGraphAgent.SetVariableValue("TeamHavingBall", isAnyHoldingBall);
		}
	}

	public void ResetInit()
	{
		foreach (SampleCharacter child in teamObjects)
		{
			child.ResetInit();
		}

	}

	// チームからボールを奪う命令
	public void OrderLoseBall()
	{
		foreach (SampleCharacter child in teamObjects)
		{
			child.isHoldingBall = false;
		}
		
	}

	// チームにボールを与える命令
	public void OrderGetBall()
	{
		teamObjects[0].isHoldingBall = true;
	}

	public CharacterTeam GetTeamType()
	{
		return TeamType;
	}

	// スタン状態じゃないキャラを取得する（読み取り専用）
	public IReadOnlyList<SampleCharacter> GetActiveCharacters()
	{
		List<SampleCharacter> activeCharacters = new List<SampleCharacter>();
		foreach(SampleCharacter character in teamObjects)
		{
			if (!character.IsStan())
				activeCharacters.Add(character);
		}

		return activeCharacters;
	}

	// GUIのデバッグ表示(MatchManagerで呼び出し)
	public void DebugGUI()
	{
		foreach (SampleCharacter child in teamObjects)
		{
			child.DebugGUI();
		}

	}
}
