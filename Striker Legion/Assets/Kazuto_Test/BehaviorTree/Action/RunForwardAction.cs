using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RunForward", story: "Run To Enemy Goal", category: "Action", id: "cc37bb13f82b2ebb2615a50cb044bac5")]
public partial class RunForwardAction : ActionBase
{
    float timer = 0.0f;

    Vector3 goalPosition;

    protected override Status OnStart()
    {
        // 基底クラスの初期化
        base.OnStart();

        // 移動処理を開始
        characterAIAgent.SteeringController.StartMove(SteeringMode.Avoid);

        timer = 0.0f;

		characterAIAgent.DebugText.SetText("前に走る");

        CharacterTeam team = CharacterTeam.Player == (CharacterTeam)characterAIAgent.TeamNumber ? CharacterTeam.Enemy : CharacterTeam.Player;

		goalPosition = MatchManager.Instance.GetGoalPosition(team);

		return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Vector3 Direction = goalPosition - GameObject.transform.position;

		characterAIAgent.SteeringController.TargetPosition = GameObject.transform.position + Direction.normalized * 1.5f;
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

