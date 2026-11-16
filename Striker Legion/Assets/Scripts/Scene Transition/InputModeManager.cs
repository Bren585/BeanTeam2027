using UnityEngine;
using UnityEngine.InputSystem;

public class InputModeManager : MonoBehaviour
{
    public static InputModeManager instance { get; private set; }

    // パッド操作フラグ
    public bool usedGamePad = false;

    private Gamepad pad;

    // Padモード変更時のマウスカーソル最終位置
    private Vector2 lastCursorPosition;
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

    // 現在のゲームパッドの情報を取得し、フラグの切り替えとカーソルの更新を行う
    // Update is called once per frame
    void Update()
    {
        if (Gamepad.current == null) return;
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

        if((Mouse.current != null && Mouse.current.delta.ReadValue() != Vector2.zero) ||
            (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame))
        {
            SetMouseMode();
        }
    }

    // ゲームパッド操作
    private void SetGamePadMode()
    {
        if (usedGamePad) return;
        lastCursorPosition = Mouse.current.position.ReadValue();
        Cursor.visible = false;
        Mouse.current.WarpCursorPosition(new Vector2(1000, -1000));
        usedGamePad = true;
    }

    // マウス操作
    private void SetMouseMode()
    {
        if (!usedGamePad) return;
        Cursor.visible = true;
        Mouse.current.WarpCursorPosition(lastCursorPosition);
        usedGamePad = false;
    }

    // フラグの取得
    public bool GetUsedGamePadFlag()
    {
        return usedGamePad;
    }
}
