using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Unity.VisualScripting;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Steal", 
    story: "Steal Ball From Enemy Character", 
    category: "Action", id: "b68009142807c675678dae36c9dc9b2c")]
public partial class StealAction : ActionBase
{
    protected override Status OnStart()
    {
        Debug.Log(self.Value.name + ": StealAction started.");
		return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // ボールを持っているキャラクターを取得
        characterComponent.Value.CharacterSensor.GetCharacterHavingBall("", 1.0f, out SampleCharacter characterWithBall);

        if(characterWithBall == null)
		{
			Debug.Log("No character is holding the ball.");
			return Status.Failure;
		}

		// ボールを奪われるキャラクターにスタンをかける
		characterWithBall.StartStan();

		// ボールを奪う処理
		characterWithBall.isHoldingBall = false;
        characterComponent.Value.isHoldingBall = true;

		return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

