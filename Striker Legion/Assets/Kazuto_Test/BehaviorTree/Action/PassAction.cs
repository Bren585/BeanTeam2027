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
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
		Transform transform = self.Value.transform;
		float searchRadius = 50f; // 検索範囲の半径
		Vector3 searchVec = new Vector3(searchRadius, searchRadius, searchRadius);

		// 指定したレイヤーのコライダーのみを取得
		Collider[] hitColliders = Physics.OverlapBox(transform.position, searchVec);
		//Collider[] hitColliders = Physics.OverlapSphere(transform.position, searchRadius);

		Transform nearestEnemy = null;
		float minDistanceSqr = Mathf.Infinity;
		GameObject objectToPassTo = null;
		Vector3 currentPosition = transform.position;

		foreach (Collider hitCollider in hitColliders)
		{
			// 自分自身は除外
			if (hitCollider.gameObject == self.Value) continue;

			// 距離の二乗で計算（Mathf.Sqrtを避けて高速化）
			Vector3 directionToTarget = hitCollider.transform.position - currentPosition;
			float dSqrToTarget = directionToTarget.sqrMagnitude;

			if (dSqrToTarget < minDistanceSqr)
			{
				minDistanceSqr = dSqrToTarget;
				nearestEnemy = hitCollider.transform;
				objectToPassTo = hitCollider.gameObject;
			}
		}

		if(objectToPassTo == null)
		{
			Debug.Log("No target to pass to");
			//return Status.Failure;
		}
		else
		{
			Debug.Log("Nearest target: " + objectToPassTo.name);
			//objectToPassTo.GetComponentInChildren<SampleCharacter>().isHoldingBall = true;
			self.Value.GetComponent<SampleCharacter>().isHoldingBall = false;
			Debug.Log("Passing to: " + objectToPassTo.name);
		}

		return Status.Success;
    }

    protected override void OnEnd()
    {
	}
}

