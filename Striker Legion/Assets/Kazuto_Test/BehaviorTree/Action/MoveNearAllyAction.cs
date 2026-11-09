using System;
using TMPro;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move Near Ally", story: "Move to near ally within [range]", category: "Action", id: "a1f80026d243500c9f7bfeed54d33e6d")]
public partial class MoveNearAllyAction : ActionBase
{
    [SerializeReference] public BlackboardVariable<float> Range;

	Vector3 targetPosition = Vector3.zero;

	Transform targetTransform;

	protected override Status OnStart()
    {
		// 基底クラスの初期化
		base.OnStart();

		float minDistance = float.MaxValue;
		bool isSuccess = false;
		foreach (CharacterBase c in MatchManager.Instance.GetActiveCharacters(characterAIAgent.Team))
		{
			float dist = (GameObject.transform.position - c.transform.position).magnitude;

			if (c.gameObject == characterAIAgent.gameObject)
				continue;

			// ダウン中は除外
			if (c.isDown)
				continue;

			// 最も近く、尚且つ一定距離より近くにいる
			if (dist < minDistance)
			{
				minDistance = dist;
				targetPosition = c.transform.position;

				targetTransform = c.transform;
				isSuccess = true;
			}
		}
		// 近くに対象がいなければ、失敗
		if (!isSuccess)
			return Status.Failure;

		characterAIAgent.SteeringController.StartMove(SteeringMode.Arrive);
		characterAIAgent.SetTargetPosition(targetTransform.position);
		characterAIAgent.DebugText.SetText("最も近い味方に移動");


		return Status.Running;
    }

	protected override Status OnUpdate()
	{
		characterAIAgent.SetTargetPosition(targetTransform.position);

		// 一定距離まで移動で成功
		if ((GameObject.transform.position - targetTransform.position).magnitude < Range.Value)
		{
			return Status.Success;
		}

		return Status.Running;
	}

    protected override void OnEnd()
    {
    }
}

