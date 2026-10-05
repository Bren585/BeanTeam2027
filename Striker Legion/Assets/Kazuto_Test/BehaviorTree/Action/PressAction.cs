using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Press", story: "Move closer to the character holding the ball", category: "Action", id: "7de5736b617fe3def2461c27d6181229")]
public partial class PressAction : ActionBase
{
    // ボールまでの距離(Blackboradの値を参照)
    [SerializeReference] public BlackboardVariable<float> distanceFromBall;

    // どこまで近づくか
    private const float PressDistance = 1.0f;

    private SampleCharacter TargetCharacter;

	protected override Status OnStart()
    {
        base.OnStart();

        characterComponent.SteeringController.StartMove();

        Debug.Log(GameObject.name + " : Start Press");

        // ターゲットを設定
        TargetCharacter = characterComponent.CharacterSensor.GetEnemyHavingBall(GameObject.tag);


		return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // 対象を設定する
        //characterComponent.SetTargetIsBall();
        characterComponent.SteeringController.TargetPosition = TargetCharacter.transform.position;

		if (distanceFromBall.Value <= PressDistance)
		{
            Debug.Log(GameObject.name + ": Success Press. Distance : " + distanceFromBall.Value);

			// 一定距離まで近づいたら成功を返す
			return Status.Success;
		}
        // 近くまで行ってない場合、実行し続ける
		return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

