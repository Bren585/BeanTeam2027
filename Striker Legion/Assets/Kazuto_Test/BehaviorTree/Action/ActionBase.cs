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

	protected override void ResetStatus()
	{
		base.ResetStatus();
		//Debug.Log("ActionBaseのリセットステータス");
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
		{
			characterAIAgent = character;
		}
		else
		{
			Debug.LogError(GameObject.name + ": Action開始時にcharacterComponentの取得に失敗");

			Initialized = false;
			return Status.Failure;
		}

		return Status.Success;
	}

	protected override void OnSetup()
	{
		base.OnSetup();
	}

	protected override void OnTeardown()
	{
		base.OnTeardown();
	}
	protected override void OnSerialize()
	{
		base.OnSerialize();
	}

	protected override void OnDeserialize()
	{
		base.OnDeserialize();
	}
}
