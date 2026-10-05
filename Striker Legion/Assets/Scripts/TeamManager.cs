using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
	// 同じチームのオブジェクトを格納する
	private List<SampleCharacter> teamObjects;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		// 子に配置されているオブジェクトをチームとして取得
		List<Transform> childrenTransforms = new List<Transform>();
		foreach (Transform child in transform)
		{
			childrenTransforms.Add(child);
		}

		// LINQを使って GameObject の配列として1行で取得
		teamObjects = transform.Cast<Transform>().Select(t => t.gameObject.GetComponent<SampleCharacter>()).ToList();

		// 子のオブジェクト名をログ出力
		foreach(SampleCharacter child in teamObjects)
		{
			Debug.Log(gameObject.name + " Child Object: " + child.name);
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
}
