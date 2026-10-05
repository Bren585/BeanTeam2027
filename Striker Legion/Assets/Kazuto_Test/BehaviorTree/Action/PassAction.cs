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
		base.OnStart();
		Debug.Log(GameObject.name + ": PassAction started.");
		return Status.Running;
	}

	protected override Status OnUpdate()
	{
		if (GameObject == null)
		{
			//Debug.Log("Self is null");
			return Status.Failure;
		}
		if (!characterComponent.isHoldingBall)
		{
			//Debug.Log("Self is not holding the ball");
			return Status.Failure;
		}

		Transform transform = GameObject.transform;
		float searchRadius = 50f; // 検索範囲の半径
		//GameObject objectToPassTo = null;
		SampleCharacter objectToPassTo = null;
		Vector3 searchVec = new Vector3(searchRadius, searchRadius, searchRadius);

		float minDistanceSqr = Mathf.Infinity;
#if false
		GameObject[] objects = GameObject.FindGameObjectsWithTag(GameObject.tag);
		for (int i = 0; i < objects.Length; i++)
		{
			if (objects[i] == GameObject) continue;
			float distance = Vector3.Distance(transform.position, objects[i].transform.position);
			if (distance <= searchRadius && distance < minDistanceSqr)
			{
				objectToPassTo = objects[i];
				minDistanceSqr = distance;
			}
		}
#endif   
		// 最も近いキャラクターを取得
		characterComponent.CharacterSensor.GetClosestCharacter(LayerMask.LayerToName(GameObject.layer), minDistanceSqr, out objectToPassTo);

		if (objectToPassTo == null)
		{
			//Debug.Log("No target to pass to");
			return Status.Failure;
		}
		else
		{
			//Debug.Log("Nearest target: " + objectToPassTo.name);
			objectToPassTo.isHoldingBall = true;
			//objectToPassTo.GetComponentInChildren<SampleCharacter>().isHoldingBall = true;
			characterComponent.isHoldingBall = false;
			//Debug.Log("Passing to: " + objectToPassTo.name);
		}

		// 何回もパスしないよう、パスをしたらスタンする
		characterComponent.StartStan();
		return Status.Success;
	}

	protected override void OnEnd()
    {
	}
}

