using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Mono.Cecil.Cil;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move Enable Receive", story: "Move to spot enable receive a pass", category: "Action", id: "23efebd20e129c8393d2a7ba151864dd")]
public partial class MoveEnableReceiveAction : ActionBase
{
    // ボールを持っているキャラクター
    CharacterBase ballHolder;

    protected override Status OnStart()
    {
        // 基底クラスの初期化
        base.OnStart();

        // 現在ボールを持っているキャラを移動の基準として保存
        ballHolder = MatchManager.Instance.CurrentBallHolder;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
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

        

        return Vector3.zero;
    }
}

