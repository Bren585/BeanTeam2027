using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Turn To Goal", story: "Turn To Opponent Goal", category: "Action", id: "eacaae34fafdf7d174d40ef7d92e3083")]
public partial class TurnToGoalAction : ActionBase
{
    Vector3 targetGoalPos;

    CharacterTeam opponentTeam;

    protected override Status OnStart()
    {
        base.OnStart();

        // 対象のゴールの座標
        targetGoalPos = MatchManager.Instance.GetGoal(characterAIAgent.OpponentTeam).position;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
		GameObject.transform.rotation = Quaternion.LookRotation((GameObject.transform.position - targetGoalPos).normalized, Vector3.up);
        
		//return Status.Running;

		return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

