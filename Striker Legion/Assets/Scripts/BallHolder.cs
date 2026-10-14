using UnityEngine;

public class BallHolder : MonoBehaviour
{  //BallHolder is a base class for objects that can hold a ball

    [field: SerializeField] public int teamNo { get; private set; }
    [field: SerializeField] public bool hasBall { get; private set; } = false;

    virtual public Vector3 GetHoldPosition() { return Vector3.zero; }
    virtual public Vector3 GetVerocity() { return Vector3.zero; }
    
    public int GetTeamNo() { return teamNo; }

    public void GetBall() { 
        hasBall = true; 
        OnGetBall(); 
    }
    public void LoseBall() { 
        hasBall = false;
    }
    virtual protected void OnGetBall() { }
}
