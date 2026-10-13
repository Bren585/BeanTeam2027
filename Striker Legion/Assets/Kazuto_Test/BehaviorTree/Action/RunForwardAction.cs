using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RunForward", story: "Run To Enemy Goal", category: "Action", id: "cc37bb13f82b2ebb2615a50cb044bac5")]
public partial class RunForwardAction : ActionBase
{
	PitchEvaluator pitchEvaluator;

    TeamType teamType;

    float timer = 0.0f;

    protected override Status OnStart()
    {
        // 基底クラスの初期化
        base.OnStart();

        // PitchEvaluatorの取得
        //if(GameObject.FindWithTag("PitchManager").GetComponent<PitchEvaluator>())
            pitchEvaluator = GameObject.FindWithTag("PitchManager").GetComponent<PitchEvaluator>();

        if (pitchEvaluator == null)
        {
            Debug.LogError("PitchEvaluator not found.");
            return Status.Failure;
        }
        else
            Debug.Log("RunForward Start");

        // 移動処理を開始
        characterComponent.SteeringController.StartMove();

        teamType = LayerMask.LayerToName(GameObject.layer) == "Player" ? TeamType.Player : TeamType.Enemy;

        timer = 0.0f;

		characterComponent.DebugText.SetText("前に走る");

		return Status.Running;
    }

    protected override Status OnUpdate()
    {
        characterComponent.SteeringController.TargetPosition = GameObject.transform.position + GameObject.transform.forward * 1.5f;
		//characterComponent.SteeringController.TargetPosition = pitchEvaluator.GetBestPosition(teamType); 
		//characterComponent.SteeringController.TargetPosition = new Vector3(0, 0, 0);

		timer += Time.deltaTime;
        if(timer >= 0.5f)
		{
			Debug.Log(GameObject.name + ": Success RunForwardAction.");
			return Status.Success;
		}
        //if (!characterComponent.isHoldingBall)
        //    return Status.Failure;

		return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

