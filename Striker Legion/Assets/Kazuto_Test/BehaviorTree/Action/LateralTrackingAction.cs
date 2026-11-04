using System;
using Unity.Behavior;
using Unity.Properties;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UIElements;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "LateralTracking", story: "Lateral Tracking for Defense from [base] limit [xlimit]", category: "Action", id: "721bbfcceaace68933e314f2138c56a3")]
public partial class LateralTrackingAction : ActionBase
{
    [SerializeReference] public BlackboardVariable<PositioningName> Base;
    [SerializeReference] public BlackboardVariable<float> XLimit;
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
        characterAIAgent.SteeringController.StartMove(SteeringMode.Seek);

        characterAIAgent.DebugText.SetText("スペースを空けないように防衛");


		return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // ターゲットがボールを失うか手放したら失敗として終了
        if (!targetCharacter.hasBall)
            return Status.Failure;

        // ターゲットを更新
        characterAIAgent.SetTargetPosition(CalculateContainTargetPositionAndRotation());
        //characterAIAgent.SetTargetPosition(CalculateContainTargetPosition());

        
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
        Vector3 result = basePosition + (offsetVector);

        result.z = characterAIAgent.BasePosition(Base.Value).z;
        //result.z = characterAIAgent.BasePosition.z;

		// 位置から計算した射影ベクトルを足した座標を返す
		return result;
	}

	public Vector3 CalculateContainTargetPositionAndRotation()
    {
		// このアクションを実行しているキャラの座標
		Vector3 basePosition = GameObject.transform.position;

		// 対象の座標
		Vector3 targetPosition = targetCharacter.transform.position;
		float targetZ = targetPosition.z;

		// 位置から敵へのベクトル
		Vector3 toEnemy = basePosition - targetPosition;
		//Vector3 toEnemy = targetPosition - basePosition;

		// 対象の正面ベクトル
		Vector3 targetForward = targetCharacter.transform.forward;
        //Debug.Log("targetForward:" + targetForward);

		// 対象の正面ベクトルと、対象までのベクトルの内積を計算
		float dot = Vector3.Dot(targetForward, toEnemy);
		//float dot = Vector3.Dot(toEnemy, targetForward);
        //Debug.Log("dot:" + dot );

        // 内積の結果を使って射影ベクトルを作る
        Vector3 vec = targetForward * dot * 0.5f;
        //Debug.Log("vec:" + vec);

        Vector3 result = targetPosition + vec;

		result.z = characterAIAgent.BasePosition(Base.Value).z;
        result.x = Mathf.Clamp(result.x, -XLimit.Value, XLimit.Value);
		//result.z = characterAIAgent.BasePosition.z;

		// 位置から計算した射影ベクトルを足した座標を返す
		return result;
	}
}

