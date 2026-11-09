using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Press", story: "Move closer to the character holding the ball within [PressDistance] Cancel [CancelDistance]", category: "Action", id: "7de5736b617fe3def2461c27d6181229")]
public partial class PressAction : ActionBase
{
	// どこまで近づくか
	[SerializeReference] public BlackboardVariable<float> PressDistance;
	[SerializeReference] public BlackboardVariable<float> CancelDistance;
    //private float PressDistance = 4.0f;

    private CharacterBase TargetCharacter;

	protected override Status OnStart()
    {
        base.OnStart();

        // 移動開始
        characterAIAgent.SteeringController.StartMove(SteeringMode.Seek);

        //Debug.Log(GameObject.name + " : Start Press");

        // ターゲットを設定
        // Todo:MatchManagerコンポーネントを修正
        TargetCharacter = MatchManager.Instance.CurrentBallHolder;

        if (TargetCharacter == null)
        {
			//Debug.Log(GameObject.name + "プレス失敗：ターゲットがNull");
			return Status.Failure;
        }
        // キャラのタグが同一なら、同じ陣営のキャラを追いかけようとしているからFalse
        if (TargetCharacter.team == characterAIAgent.Team)
            return Status.Failure;

		// 行動をデバッグで表示
		characterAIAgent.DebugText.SetText("プレスをかける");

		return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // ターゲットがいなかったら失敗
        if (TargetCharacter == null)
        {
            //Debug.Log(GameObject.name + "プレス失敗：ターゲットを見失った");
            return Status.Failure;
        }
        // ターゲットがボールを手放したら失敗
        if (!TargetCharacter.hasBall)
        {
			//Debug.Log(GameObject.name + "プレス失敗：ターゲットがボールを手放した");
			return Status.Failure;
        }
        
        // 追跡する座標をターゲットの位置に更新
		characterAIAgent.SetTargetPosition(TargetPosition());
		//characterAIAgent.SetTargetPosition(TargetCharacter.transform.position);

        Vector3 Distance = TargetCharacter.transform.position - characterAIAgent.transform.position;

        // ターゲットまでの距離を取得
        if (Distance.magnitude <= PressDistance || !characterAIAgent.SteeringController.IsMove)
        {
            //Debug.Log(GameObject.name + ": プレス成功");

            // 一定距離まで近づいたら成功を返す
            return Status.Success;
        }
        else if (Distance.magnitude > CancelDistance)
        {
			//Debug.Log(GameObject.name + "プレス失敗：ターゲットが遠すぎる");
			return Status.Failure;
        }
		// 近くまで行ってない場合、実行し続ける
		return Status.Running;
    }

    protected override void OnEnd()
    {
    }

    Vector3 TargetPosition()
    {
        // 対象のキャラの座標
        Vector3 targetCharacterPosition = TargetCharacter.transform.position;

        // 対象のキャラの前方方向ベクトル
        Vector3 targetForward = TargetCharacter.transform.forward;

        // 前方方向を移動予想としてベクトルの補正を入れる
        Vector3 result = targetCharacterPosition + targetForward * 5.0f;

		return result;
	}
}

