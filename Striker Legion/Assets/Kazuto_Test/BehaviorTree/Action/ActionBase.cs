using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable]
public partial class ActionBase : Action
{
	// 割り込み可能フラグ
	protected bool isInterrupted = false;
	//protected BlackboardVariable<bool> isInterrupted = new BlackboardVariable<bool>(false);
	//protected GameObject self;
	protected SampleCharacter characterComponent;
	//[SerializeReference] public BlackboardVariable<SampleCharacter> characterComponent;

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

		//// Behaviorエージェントを取得
		//if (!GameObject.TryGetComponent<BehaviorGraphAgent>(out var agent))
		//{
		//	Debug.LogError(GameObject.name + ": Action開始時にBehaviorエージェントの取得に失敗");
		//	return Status.Failure;
		//}

		// SampleCharacterコンポーネントを取得
		if (GameObject.TryGetComponent<SampleCharacter>(out var character))
		//if (agent.GetVariable<SampleCharacter>("CharacterComponent", out var character))
		{
			characterComponent = character;
		}
		else
		{
			Debug.LogError(GameObject.name + ": Action開始時にcharacterComponentの取得に失敗");
			return Status.Failure;
		}

		return Status.Success;
	}
}
