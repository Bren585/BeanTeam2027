using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable]
public partial class ActionBase : Action
{
	// 割り込み可能フラグ
	[SerializeReference] public BlackboardVariable<bool> isInterrupted = new BlackboardVariable<bool>(false);
	[SerializeReference] public BlackboardVariable<GameObject> self;
	[SerializeReference] public BlackboardVariable<SampleCharacter> characterComponent;

	
	protected override Status OnStart()
	{
		return Status.Running;
	}

	protected override Status OnUpdate()
	{
		return Status.Success;
	}

	protected override void OnEnd()
	{
	}
}
