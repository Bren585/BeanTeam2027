using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Press", story: "Move closer to the character holding the ball within [PressDistance]", category: "Action", id: "7de5736b617fe3def2461c27d6181229")]
public partial class PressAction : ActionBase
{
    // ボールまでの距離(Blackboradの値を参照)
    [SerializeReference] public BlackboardVariable<float> distanceFromBall;

	// どこまで近づくか
	[SerializeReference] public BlackboardVariable<float> PressDistance;
    //private float PressDistance = 4.0f;

    private CharacterBase TargetCharacter;

	protected override Status OnStart()
    {
        base.OnStart();

        // ターゲットの設定
#if true

        // 移動開始
        characterAIAgent.SteeringController.StartMove(SteeringMode.Arrive);

        //Debug.Log(GameObject.name + " : Start Press");

        // ターゲットを設定
        // Todo:MatchManagerコンポーネントを修正
        TargetCharacter = MatchManager.Instance.CurrentBallHolder;

        if (TargetCharacter == null)
            return Status.Failure;

        // キャラのタグが同一なら、同じ陣営のキャラを追いかけようとしているからFalse
        if (TargetCharacter.teamNo == characterAIAgent.TeamNumber)
            return Status.Failure;

		// 行動をデバッグで表示
		characterAIAgent.DebugText.SetText("プレスをかける");
#endif

		return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // ターゲットがいなかったら失敗
        if (TargetCharacter == null)
            return Status.Failure;

		// 追跡する座標をターゲットの位置に更新
		characterAIAgent.SteeringController.TargetPosition = TargetCharacter.transform.position;

        Vector3 Distance = TargetCharacter.transform.position - characterAIAgent.transform.position;

        // ターゲットまでの距離を取得
        if (Distance.magnitude <= PressDistance)
        {
            //Debug.Log(GameObject.name + ": Success Press. Distance : " + distanceFromBall.Value);

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

