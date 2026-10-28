using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move Best Position", story: "Move Best Position Selected by Grid Utility", category: "Action", id: "2c6268344c45d9b6378e14efeed3b6c9")]
public partial class MoveBestPositionAction : ActionBase
{
    Vector3 bestPosition;

    protected override Status OnStart()
    {
        // 基底クラスの初期化
        base.OnStart();

        // 目標地点として、最も評価が高い場所を取得
        bestPosition = characterAIAgent.GetClosestPointOnBestGrid(GameObject.transform.position);
        //bestPosition = characterAIAgent.BestPosition;

        // ステアリングの対象指定
        characterAIAgent.SetTargetPosition(bestPosition);

		// 行動をデバッグで表示
		characterAIAgent.DebugText.SetText("スコアが最大の位置に移動");

		characterAIAgent.SteeringController.StartMove(SteeringMode.Arrive);

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // 移動終了で成功を返す
        if (!characterAIAgent.SteeringController.IsMove)
            return Status.Success;

        // 目標座標の更新
        if(characterAIAgent.IsUpdateScore)
        {
			bestPosition = characterAIAgent.GetClosestPointOnBestGrid(GameObject.transform.position);

			// ステアリングの対象指定
			characterAIAgent.SetTargetPosition(bestPosition);
		}    

        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

