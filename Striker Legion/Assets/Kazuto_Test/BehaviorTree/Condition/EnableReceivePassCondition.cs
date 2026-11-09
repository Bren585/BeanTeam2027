using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Enable Receive Pass", story: "Enable Receive Pass From Having Ball Character", category: "Conditions", id: "fb0ae9f6ea3b5d494693016d9b1dd7ea")]
public partial class EnableReceivePassCondition : Condition
{

    public override bool IsTrue()
    {
        if (MatchManager.Instance.CurrentBallHolder == null)
            return false;

      
        return MatchManager.Instance.CurrentBallHolder.EnablePass();

        //return true;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
