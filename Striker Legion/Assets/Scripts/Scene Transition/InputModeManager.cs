using UnityEngine;
using UnityEngine.InputSystem;

public class InputModeManager : MonoBehaviour
{
    public static InputModeManager instance { get; private set; }

    // パッド操作フラグ
    private bool usedGamePad = false;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(Gamepad.current != null &&
            Gamepad.current.wasUpdatedThisFrame)
        {
            SetGamePadMode();
        }

        if(Mouse.current != null &&
            Mouse.current.delta.ReadValue() != Vector2.zero)
        {
            SetMouseMode();
        }
    }

    private void SetGamePadMode()
    {
        if (usedGamePad) return;
        Cursor.visible = false;
        usedGamePad = true;
    }

    private void SetMouseMode()
    {
        if (!usedGamePad) return;
        Cursor.visible = true;
        usedGamePad = false;
    }
}
