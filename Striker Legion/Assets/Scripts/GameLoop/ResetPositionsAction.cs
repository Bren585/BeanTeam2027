using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Unity.VisualScripting;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Reset Positions", story: "Respawn all SpawnLinkages via [Field]", category: "Action", id: "c6c330c920d13ba98de4aea04963cd8f")]
public partial class ResetPositionsAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Field;

    protected override Status OnStart()
    {
        foreach (SpawnerLinkage spawner in Field.ConvertTo<GameObject>().GetComponentsInChildren<SpawnerLinkage>())
        {
            spawner.respawn();
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

