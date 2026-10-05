using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;


public class PlayerController : MonoBehaviour
{
    [SerializeField] int                teamNo;
    [SerializeField] GameBall           gameBall; 

    CharacterBase                       playerCharacter;
    [SerializeField] CinemachineCamera  playerCamera;

    [SerializeField] float              cameraMaxSpeed;
    Transform                           cameraTarget;
    
    Vector2 moveInput;

    CharacterBase GetClosestPossessableCharacter()
    {
        Vector3 target;
        {
            CharacterBase ballOwner = gameBall.GetOwner();
            if (ballOwner == null) { target = gameBall.transform.position; }
            else if (ballOwner.teamNo == teamNo) { return ballOwner; }
            else { target = ballOwner.transform.position; }
        }

        CharacterBase closestCharacter = null;
        float closest = float.MaxValue;

        CharacterBase[] allCharacters = FindObjectsByType<CharacterBase>();

        foreach (CharacterBase character in allCharacters)
        {
            if (character.teamNo != teamNo) { continue; }
            float distance = (character.transform.position - target).magnitude;
            if (distance < closest)
            {
                closest = distance;
                closestCharacter = character;
            }
        }

        return closestCharacter;
    }

    void Start()
    {
        gameBall = FindAnyObjectByType<GameBall>();
        if (gameBall)
        {
            PossessCharacter(GetClosestPossessableCharacter());
        }
        transform.position = cameraTarget.position;
    }

    void Update()
    {
        // Player Move Input, rotated to Camera
        if (playerCharacter != null)
        {

            Quaternion cameraYaw = Quaternion.Euler(0, playerCamera.transform.rotation.eulerAngles.y, 0);
            Vector3 rotatedInput = cameraYaw * new Vector3(moveInput.x, 0, moveInput.y);
            playerCharacter.Move(rotatedInput);
        }
        else
        {
            // No player? Follow the ball as a fallback.
            cameraTarget = gameBall.transform;
        }

        // Move the Camera.
        Vector3 toTarget = cameraTarget.position - transform.position;
        float distance = toTarget.magnitude;
        float speedTime = Time.deltaTime * cameraMaxSpeed;
        if (distance > speedTime) {
            toTarget *= (speedTime / distance); 
        }
        toTarget.y = 0;
        transform.position += toTarget;
    }

    public void PossessCharacter(CharacterBase character) 
    { 
        playerCharacter = character;
        if (playerCharacter == null)
        {
            cameraTarget = gameBall.transform;
        }
        else
        {
            cameraTarget = playerCharacter.transform;
        }
    }

    public void OnMove(InputValue input)
    {
        moveInput = input.Get<Vector2>();
    }

    public void OnJump()
    {
        if (playerCharacter == null) { return; }
        playerCharacter.Jump();
    }

    public void OnSkill()
    {
        if (playerCharacter == null) { return; }
        playerCharacter.Skill();
    }

    public void OnShoot()
    {
        if (playerCharacter == null) { PossessCharacter(GetClosestPossessableCharacter()); }
        if (playerCharacter.hasBall)
        {
            playerCharacter.Shoot();

            CharacterBase ballOwner = gameBall.GetOwner();
            if (ballOwner != playerCharacter && ballOwner != null)
            {
                if (ballOwner.teamNo == teamNo)
                {
                    PossessCharacter(ballOwner);
                }
            }
        }
        else
        {
            if (gameBall.GetOwner() == playerCharacter) { return; }
            CharacterBase possesionTarget = playerCharacter.PassTarget();
            if (possesionTarget == null)
            {
                possesionTarget = GetClosestPossessableCharacter();
            }
            PossessCharacter(possesionTarget);
        }
    }

    public void OnTackle()
    {
        if (playerCharacter == null) { PossessCharacter(GetClosestPossessableCharacter()); }
        playerCharacter.Tackle();
    }

    public void OnStrafe(InputValue input)
    {
        if (playerCharacter == null) { return; }
        playerCharacter.SetStrafe(input.isPressed);
    }
}
