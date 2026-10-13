using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "BackDefense", story: "Back To Defense", category: "Action", id: "f1faf8c2153754cad20e59a5c72a9ff3")]
public partial class BackDefenseAction : ActionBase
{
    float timer = 0.0f;

    protected override Status OnStart()
    {
        // 基底クラスの初期化
        base.OnStart();

		// 移動処理を開始
		characterComponent.SteeringController.StartMove();

        // タイマーの初期化
        timer = 0.0f;

        // テキスト表示
        characterComponent.DebugText.SetText("守備に戻る");

		return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // 目標地点をキャラクターの後ろ方向に設定
		characterComponent.SteeringController.TargetPosition = GameObject.transform.position + -GameObject.transform.forward * 1.5f;

		timer += Time.deltaTime;
		if (timer >= 0.5f)
		{
			Debug.Log(GameObject.name + ": Success RunForwardAction.");
			return Status.Success;
		}

		return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

