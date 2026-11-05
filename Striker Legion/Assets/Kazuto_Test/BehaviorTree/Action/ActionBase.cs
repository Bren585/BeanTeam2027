using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable]
public partial class ActionBase : Action
{	
	protected CharacterAIAgent characterAIAgent;

	bool Initialized = false;

	
	protected override Status OnStart()
	{
		return Initialize();
	}

	protected override Status OnUpdate()
	{
		return Status.Success;
	}

	protected override void OnEnd()
	{
	}

	private Status Initialize()
	{
		if(Initialized)
		{
			return Status.Success;
		}
		else
		{
			Debug.Log(GameObject.name + ": Action初期化");

			// 初期化ロジック
			Initialized = true;
		}

		// SampleCharacterコンポーネントを取得
		if (GameObject.TryGetComponent<CharacterAIAgent>(out var character))
		//if (agent.GetVariable<SampleCharacter>("CharacterComponent", out var character))
		{
			characterAIAgent = character;
		}
		else
		{
			Debug.LogError(GameObject.name + ": Action開始時にcharacterComponentの取得に失敗");
			return Status.Failure;
		}

		return Status.Success;
	}
}
