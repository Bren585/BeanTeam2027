using UnityEngine;

public class BallHolder : MonoBehaviour
{  //BallHolder is a base class for objects that can hold a ball


    [field: SerializeField] public CharacterTeam team { get; private set; }
    
    public bool hasBall => (Ball().GetOwner() == this);

    virtual public Vector3 GetHoldPosition() { return Vector3.zero; }
    virtual public Vector3 GetVelocity() { return Vector3.zero; }

    protected GameBall gameBall;

    //public int GetTeamNo() { return team; }

    private GameBall Ball() { if (gameBall != null) return gameBall; else return gameBall = FindAnyObjectByType<GameBall>(); }

    private void Start()
    {
        gameBall = FindAnyObjectByType<GameBall>();
    }

    public void GetBall() { 
        //hasBall = true; 
        OnGetBall(); 
    }
    public void LoseBall() { 
        //hasBall = false;
    }

    public void SetTeam(CharacterTeam team) { this.team = team; }

    virtual protected void OnGetBall() { }
}
