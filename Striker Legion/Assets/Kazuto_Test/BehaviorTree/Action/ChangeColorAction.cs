using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Change Color", story: "Change [Color] If is true add color: [IsAdd]", category: "Action", id: "c4687ae26c8a53b8719fe6ea1d3b7f76")]
public partial class ChangeColorAction : ActionBase
{
    [SerializeReference] public BlackboardVariable<Color> Color;
    [SerializeReference] public BlackboardVariable<bool> IsAdd;
    protected override Status OnStart()
    {
		// オブジェクトの Renderer と Material を取得
		Renderer targetRenderer = GameObject.GetComponent<Renderer>();

		// 使用しているマテリアルを取得
        Material targetMaterial = targetRenderer.material;

        if (IsAdd)
        {
            Color BaseColor = targetMaterial.GetColor("_BaseColor");
            BaseColor += Color;
            targetMaterial.SetColor("_BaseColor", BaseColor);
        }
        else
        {
            // 色を変更 
            targetMaterial.SetColor("_BaseColor", Color);
        }
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

