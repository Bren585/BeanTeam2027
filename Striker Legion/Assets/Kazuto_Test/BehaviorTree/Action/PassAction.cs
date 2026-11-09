using System;
using System.Collections.Generic;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Pass", 
    story: "Pass To Other Character", 
    category: "Action", 
    id: "072f5c0b970ac8587b9f39a11da9c895")]
public partial class PassAction : ActionBase
{
	//IReadOnlyList<SampleCharacter> ActiveCharacters;

	protected override Status OnStart()
	{
		base.OnStart();

		characterAIAgent.DebugText.SetText("パスを実行");

		// パスの回数が時間内に一定回数を超えていたら失敗
		if (!characterAIAgent.PermissiblePassCount)
		{
			Debug.Log(GameObject.name + "パスカウントがオーバー");
			return Status.Failure;
		}

		return Status.Running;
	}

	protected override Status OnUpdate()
	{
		if (!characterAIAgent.HasBall)
		{

			Debug.Log(GameObject.name + "パス前にボールロスト");
			return Status.Failure;
		}
		characterAIAgent.Pass();

		// パスカウント
		characterAIAgent.IncrementPassCount();



		return Status.Success;
	}

	protected override void OnEnd()
    {
	}
}

