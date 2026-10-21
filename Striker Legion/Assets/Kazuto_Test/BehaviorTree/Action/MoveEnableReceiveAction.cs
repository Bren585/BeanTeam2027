using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Mono.Cecil.Cil;
using System.Collections.Generic;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move Enable Receive", story: "Move to spot enable receive a pass", category: "Action", id: "23efebd20e129c8393d2a7ba151864dd")]
public partial class MoveEnableReceiveAction : ActionBase
{
    // ボールを持っているキャラクター
    CharacterBase ballHolder;

    float timer = 0.0f;

    const float RetryTime = 1.0f;

    protected override Status OnStart()
    {
        // 基底クラスの初期化
        base.OnStart();

        // 現在ボールを持っているキャラを移動の基準として保存
        ballHolder = MatchManager.Instance.CurrentBallHolder;

        if (ballHolder == null)
        {
            Debug.Log(GameObject.name + ":ボールを持っているキャラがいない");
            return Status.Failure;
        }
        else
            Debug.Log("ボールを持っている対象:" + ballHolder.name);

        // 目標地点を設定
        characterAIAgent.SetTargetPosition(CalculateEnableReceivePos());

        // 移動開始
        characterAIAgent.SteeringController.StartMove(SteeringMode.Arrive);

        timer = 0.0f;


		characterAIAgent.DebugText.SetText("パスが受けられる場所に移動");
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // ボールを持っていたキャラクターがボールを奪われてたら失敗
        if (!ballHolder.hasBall)
        {
			characterAIAgent.SteeringController.CancelMove();

			return Status.Failure;
        }
        // ボールを持っているキャラがパスできるようになれば成功
   //     if (ballHolder.EnablePass())
   //     {
   //         Debug.Log("パスが受けられる");
			//characterAIAgent.SteeringController.CancelMove();
			//return Status.Success;
   //     }
        // 移動終了で成功
        if (!characterAIAgent.SteeringController.IsMove)
            return Status.Success;

        timer += Time.deltaTime;
		// 時間経過で目標座標を再計算
		// Todo:時間で更新させているけど、大きく動きすぎる。制限をかけないといけない

		if (RetryTime < timer)
        {
            timer = 0.0f;
			characterAIAgent.SetTargetPosition(CalculateEnableReceivePos());
		}

        return Status.Running;
    }

    protected override void OnEnd()
    {

	}

    Vector3 CalculateEnableReceivePos()
    {
        // ボールを持っている対象の座標
        Vector3 targetPosition = ballHolder.transform.position;
        // 対象の正面ベクトル
        Vector3 targetForward = ballHolder.transform.forward;

        // ボールを持っているキャラまでのベクトル
        Vector3 direction = (targetPosition - GameObject.transform.position);

#if true
        // 内積の結果で使う垂直ベクトルの向きを変える
        Vector3 up = -Vector3.up;

        // 対象までのベクトルと垂直のベクトル
        Vector3 cross = Vector3.Cross(direction.normalized, up);

        Vector3 vec = cross + direction.normalized;
        // 垂直ベクトルと対象の前方ベクトルで内積
        float dot = Vector3.Dot(targetForward, vec.normalized);
        //float dot = Vector3.Dot(targetForward.normalized, cross.normalized);

        // 内積結果が負なら再計算
        if (dot <= 0.0f)
        {
            up = Vector3.up;

            // 対象までのベクトルと垂直のベクトル
            cross = Vector3.Cross(direction.normalized, up);
            vec = cross + direction.normalized;
        }
#else
		float dot = Vector3.Dot(targetForward, direction.normalized);
        Vector3 up = (dot > 2.0f) ? Vector3.up : -Vector3.up;

		// 対象までのベクトルと垂直のベクトル
		Vector3 cross = Vector3.Cross(direction.normalized, up);

		Vector3 vec = cross.normalized + direction.normalized;
#endif
		// ベクトルを合成し、迂回するベクトルを取る
		//Vector3 vec = cross.normalized + direction.normalized;

        // 旋回する半径
		float radius = 20.0f;

		return GameObject.transform.position + vec.normalized * radius;

    }
}

