using UnityEngine;
using UnityEngine.InputSystem;

public class InputModeManager : MonoBehaviour
{
    public static InputModeManager instance { get; private set; }

    // パッド操作フラグ
    public bool usedGamePad = false;

    private Gamepad pad;
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
        pad = Gamepad.current;
        bool padInput = false;
        if (pad != null)
        {
            padInput =
                pad.buttonSouth.wasPressedThisFrame ||
                pad.buttonNorth.wasPressedThisFrame ||
                pad.buttonEast.wasPressedThisFrame ||
                pad.buttonWest.wasPressedThisFrame ||
                pad.startButton.wasPressedThisFrame ||
                pad.selectButton.wasPressedThisFrame ||
                pad.leftShoulder.wasPressedThisFrame ||
                pad.rightShoulder.wasPressedThisFrame ||
                pad.dpad.up.wasPressedThisFrame ||
                pad.dpad.down.wasPressedThisFrame ||
                pad.dpad.left.wasPressedThisFrame ||
                pad.dpad.right.wasPressedThisFrame ||
                pad.leftStick.ReadValue().sqrMagnitude > 0.01f ||
                pad.rightStick.ReadValue().sqrMagnitude > 0.01f;
        }

        if (pad != null && padInput)
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
