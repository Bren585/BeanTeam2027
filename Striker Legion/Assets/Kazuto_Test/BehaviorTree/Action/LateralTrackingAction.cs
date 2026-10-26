using System;
using Unity.Behavior;
using Unity.Properties;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UIElements;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "LateralTracking", story: "Lateral Tracking for Defense", category: "Action", id: "721bbfcceaace68933e314f2138c56a3")]
public partial class LateralTrackingAction : ActionBase
{
    CharacterBase targetCharacter;

    float timer = 0.0f;

    protected override Status OnStart()
    {
        // 基底クラスの初期化
        base.OnStart();

		timer = 0.0f;

		// 今のボール保持者をターゲットとして保存
		targetCharacter = MatchManager.Instance.CurrentBallHolder;

        // 移動開始
        characterAIAgent.SteeringController.StartMove(SteeringMode.Arrive);

        characterAIAgent.DebugText.SetText("スペースを空けないように防衛");


		return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // ターゲットがボールを失うか手放したら失敗として終了
        if (targetCharacter == null) 
            return Status.Failure;
        if (!targetCharacter.hasBall)
            return Status.Failure;

        // ターゲットを更新
        characterAIAgent.SetTargetPosition(CalculateContainTargetPosition());
        //characterAIAgent.SteeringController.TargetPosition = CalculateContainTargetPosition();
        //characterAIAgent.SteeringController.TargetPosition = CalculateMovePosition();

        timer += Time.deltaTime;

        if (!characterAIAgent.SteeringController.IsMove)
            return Status.Success;

        if (timer > 5.0f)
            return Status.Success;

        return Status.Running;
    }

    protected override void OnEnd()
    {
        timer = 0.0f;
    }

	public Vector3 CalculateContainTargetPosition()
	{
		// このアクションを実行しているキャラの座標
		Vector3 basePosition = GameObject.transform.position;

		// 対象の座標
		Vector3 targetPosition = targetCharacter.transform.position;
        float targetZ = targetPosition.z;

		// 位置から敵へのベクトル
		Vector3 toEnemy = targetPosition - basePosition;

		// 横方向ベクトルに射影して、距離を出す
		float rawRightOffset = Vector3.Dot(toEnemy, Vector3.right);

        // 少し後ろ（自身のゴール）よりになるようにする
        Vector3 offsetVector = Vector3.right * rawRightOffset;
        //offsetVector.z = (toEnemy.z > 0.0f) ? -0.3f : 0.3f;
        //offsetVector.z = (toEnemy.z > 0.0f) ? 0.1f : -0.1f;
        Vector3 result = basePosition + (offsetVector);

        result.z = characterAIAgent.BasePosition.z;

		// 位置から計算した射影ベクトルを足した座標を返す
		return result;
		//return basePosition + (Vector3.right * rawRightOffset);
	}
}

