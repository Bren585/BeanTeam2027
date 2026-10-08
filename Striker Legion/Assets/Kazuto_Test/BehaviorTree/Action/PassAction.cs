using System;
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
	protected override Status OnStart()
	{
		// 基底クラスの初期化
		base.OnStart();
		Debug.Log(GameObject.name + ": PassAction started.");
		return Status.Running;
	}

	protected override Status OnUpdate()
	{
		// キャラがボールをロストしていたら、失敗で終了
		if (!characterComponent.isHoldingBall)
		{
			return Status.Failure;
		}

		Transform transform = GameObject.transform;

		// パス対象を保存する
		SampleCharacter objectToPassTo = null;
		
		float minDistanceSqr = Mathf.Infinity;

		// 最も近いキャラクターを取得
		characterComponent.CharacterSensor.GetClosestCharacter(LayerMask.LayerToName(GameObject.layer), minDistanceSqr, out objectToPassTo);

		if (objectToPassTo == null)
		{
			//Debug.Log("No target to pass to");
			return Status.Failure;
		}
		else
		{
			objectToPassTo.isHoldingBall = true;
			characterComponent.isHoldingBall = false;
		}

		// 何回もパスしないよう、パスをしたらスタンする
		//characterComponent.StartStan();
		return Status.Success;
	}

	protected override void OnEnd()
    {
	}
}

