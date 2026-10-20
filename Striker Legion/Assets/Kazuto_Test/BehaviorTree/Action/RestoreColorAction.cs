using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Restore Color", story: "Restore Color From Changed Color", category: "Action", id: "0859ac6bd3e19283f368da8c2a9977dd")]
public partial class RestoreColorAction : ActionBase
{

    protected override Status OnStart()
    {
        base.OnStart();

		// オブジェクトの Renderer と Material を取得
		Renderer targetRenderer = GameObject.GetComponent<Renderer>();

        if (targetRenderer == null)
            return Status.Failure;
        
		// 使用しているマテリアルを取得
		Material targetMaterial = targetRenderer.material;

        if (targetMaterial == null)
            return Status.Failure;

		// 色を変更
		targetMaterial.SetColor("_BaseColor", characterAIAgent.BaseColor);

        Debug.Log("色を元に戻す");

		return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

