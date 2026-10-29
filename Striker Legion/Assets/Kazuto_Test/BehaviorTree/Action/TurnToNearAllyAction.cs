using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.UIElements;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Turn to Near Ally", story: "Takes [seconds] to Turn to Near Ally", category: "Action", id: "97ab3efd7825bcfeef447e99c65b4640")]
public partial class TurnToNearAllyAction : ActionBase
{
    [SerializeReference] public BlackboardVariable<float> Seconds;
    Vector3 targetPosition = Vector3.zero;

    Transform targetTransform;

    float timer = 0.0f;

    protected override Status OnStart()
    {
        // 基底クラスの初期化
        base.OnStart();

        float minDistance = float.MaxValue;
        float maxDistance = 20.0f;
        bool isSuccess = false;
        foreach(CharacterBase c in MatchManager.Instance.GetActiveCharacters(characterAIAgent.Team))
        {
            float dist = (GameObject.transform.position - c.transform.position).magnitude;

            if(c.gameObject == characterAIAgent.gameObject)
                continue;

            // 最も近く、尚且つ一定距離より近くにいる
            if(dist < minDistance && dist < maxDistance)
            {
                minDistance = dist;
                targetPosition = c.transform.position;

                targetTransform = c.transform;
				isSuccess = true;
			}
        }
        // 近くに対象がいなければ、失敗
        if (!isSuccess)
            return Status.Failure;

        characterAIAgent.DebugText.SetText("最も近い味方に方向転換,距離:"+minDistance);

        timer = 0.0f;

        
        //characterAIAgent.SetTargetPosition(Vector3.zero);
        //characterAIAgent.SteeringController.StartMove(SteeringMode.Arrive);
        //characterAIAgent.SteeringController.SetParentVelocity(Vector3.zero);
        //characterAIAgent.SetTargetPosition(GameObject.transform.position + (GameObject.transform.position - targetPosition).normalized);
		return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if(Seconds > 0.0f)
		    GameObject.transform.rotation = Quaternion.Lerp(GameObject.transform.rotation, Quaternion.LookRotation((targetTransform.position - GameObject.transform.position), Vector3.up), timer / Seconds);
		else
            GameObject.transform.rotation = Quaternion.LookRotation((GameObject.transform.position - targetTransform.position).normalized, Vector3.up);

		timer += Time.deltaTime;
        if(timer >= Seconds)
        {
            timer = 0.0f;
        return Status.Success;

        }    

        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

