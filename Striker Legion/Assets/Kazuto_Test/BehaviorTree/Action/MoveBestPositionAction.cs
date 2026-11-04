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

    bool isUpdateScore = false;

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

		characterAIAgent.SteeringController.StartMove(SteeringMode.Avoid);

        isUpdateScore = false;


		return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // 移動終了で成功を返す
        if (!characterAIAgent.SteeringController.IsMove)
            return Status.Success;

        // 目標座標の更新
        if(characterAIAgent.IsUpdateScore && !isUpdateScore)
        {
			bestPosition = characterAIAgent.GetClosestPointOnBestGrid(GameObject.transform.position);

            // 更新は一回だけ行う
            isUpdateScore = true;

			// ステアリングの対象指定
			characterAIAgent.SetTargetPosition(bestPosition);
            Debug.Log("ベストな地点を更新");
		}

		// このアクションでボールを持っている場合、近くに敵がいる場合は失敗を返す
		if (characterAIAgent.HasBall)
        {
			if (characterAIAgent.CharacterSensor.IsNearbyOpponent(10.0f))
			{
				Debug.Log(GameObject.name + "近くに敵がいる場合、終了");
				return Status.Failure;
			}
		}

        return Status.Running;
    }

    protected override void OnEnd()
    {
        isUpdateScore = false;
    }
}

