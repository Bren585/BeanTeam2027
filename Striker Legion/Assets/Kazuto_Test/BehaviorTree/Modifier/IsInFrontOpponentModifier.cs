using System;
using Unity.Behavior;
using UnityEngine;
using Modifier = Unity.Behavior.Modifier;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "IsInFrontOpponent", story: "Is There Opponent In Front Of [characterComponent]", category: "Flow", id: "be5c026402a3f12bc2067e6a40a9c889")]
public partial class IsInFrontOpponentModifier : Modifier
{
    [SerializeReference] public BlackboardVariable<SampleCharacter> CharacterComponent;


    protected override Status OnStart()
    {
		Debug.Log("Is In Front OpponentModifier Start");
		return Status.Running;
    }

    protected override Status OnUpdate()
    {
		// 正面に敵がいないかどうかチェックする
		//bool isHit = CharacterComponent.Value.CharacterSensor.IsEnemyInRange(LayerMask.LayerToName(CharacterComponent.Value.gameObject.layer), 1.0f);
		if (Child != null)
		{
			//return Child.Execute();
		}

		// 子ノードが接続されていない場合は Failure
		return Status.Failure;

		Debug.Log("Is In Front OpponentModifier Update");

        return Status.Success;
        //return status;
		//return (isHit) ? Status.Failure : Status.Success;
	}

    protected override void OnEnd()
    {
        Debug.Log("Is In Front OpponentModifier End");
    }
}

