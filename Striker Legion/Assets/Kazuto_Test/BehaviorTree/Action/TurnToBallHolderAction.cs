using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Turn to BallHolder", story: "Takes [seconds] to Turn to BallHolder", category: "Action", id: "4aa4ea2b771c8f830ab33ca132ccd21e")]
public partial class TurnToBallHolderAction : ActionBase
{
	[SerializeReference] public BlackboardVariable<float> Seconds;
	Vector3 targetPosition = Vector3.zero;

	Transform targetTransform;

	Quaternion startRotation;

	float timer = 0.0f;

	protected override Status OnStart()
	{
		// 基底クラスの初期化
		base.OnStart();

		if (MatchManager.Instance.BallTransform == null)
			return Status.Failure;

		// ボールの姿勢情報を対象とする
		targetTransform = MatchManager.Instance.BallTransform;

		characterAIAgent.SteeringController.CancelMove();

		characterAIAgent.DebugText.SetText("ボールを持っているキャラに方向転換");

		timer = 0.0f;

		startRotation = GameObject.transform.rotation;

		//characterAIAgent.SteeringController.StartMove(SteeringMode.Simple);
		//characterAIAgent.SetTargetPosition(TargetPosition());


		return Status.Running;
	}

	protected override Status OnUpdate()
	{
#if true
		//characterAIAgent.DebugText.SetText("最も近い味方に方向転換,係数:" + (timer / Seconds));

		if (Seconds > 0.0f)
			GameObject.transform.rotation = Quaternion.Lerp(startRotation, Quaternion.LookRotation((targetTransform.position - GameObject.transform.position), GameObject.transform.up), timer / Seconds);
		else
			GameObject.transform.rotation = Quaternion.LookRotation((GameObject.transform.position - targetTransform.position).normalized, Vector3.up);
#else
        characterAIAgent.SetTargetPosition(TargetPosition());
		//GameObject.transform.rotation = Quaternion.Lerp(GameObject.transform.rotation, Quaternion.LookRotation((targetTransform.position - GameObject.transform.position), GameObject.transform.up), timer / Seconds);
        //if(Quaternion.Dot(GameObject.transform.rotation, Quaternion.LookRotation((GameObject.transform.position - targetTransform.position).normalized, GameObject.transform.up)) > 0.7f)
        if(Quaternion.Dot(GameObject.transform.rotation, Quaternion.LookRotation((targetTransform.position - GameObject.transform.position), GameObject.transform.up)) > 0.7f)
		{
			return Status.Success;
		}
#endif

		timer += Time.deltaTime;
		if (timer >= Seconds)
		{
			timer = 0.0f;
			return Status.Success;
		}

		return Status.Running;
	}

	protected override void OnEnd()
    {
    }

	Vector3 TargetPosition()
	{
		Vector3 direction = targetTransform.position - GameObject.transform.position;


		return GameObject.transform.position + direction.normalized * 2.0f;
	}
}

