using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterBase playerCharacter;
    [SerializeField] private Camera playerCamera;

    [SerializeField] private Vector3 cameraOffset;
    [SerializeField] private Quaternion cameraAngle;

    Vector2 moveInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerCharacter == null) { return; }

        Quaternion cameraYaw = Quaternion.Euler(0, cameraAngle.eulerAngles.y, 0);
        Vector3 rotatedInput = cameraYaw * new Vector3(moveInput.x, 0, moveInput.y);
        playerCharacter.Move(rotatedInput);

        if (playerCamera == null) { return; }
        playerCamera.transform.position = playerCharacter.transform.position + cameraOffset;

    }

    public void PossessCharacter(CharacterBase character) 
    { 
        playerCharacter = character;
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
        if (playerCharacter == null) { return; }
        playerCharacter.Shoot();
    }

    public void OnTackle()
    {
        if (playerCharacter == null) { return; }
        playerCharacter.Tackle();
    }

    public void OnStrafe(InputValue input)
    {
        if (playerCharacter == null) { return; }
        playerCharacter.SetStrafe(input.isPressed);
    }
}
