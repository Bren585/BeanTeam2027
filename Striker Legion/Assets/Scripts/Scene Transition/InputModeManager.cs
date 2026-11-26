using Unity.Behavior;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputModeManager : MonoBehaviour
{
    // 使用中の入力デバイス
    public enum UsingDevice
    {
        None,
        Keyboard, // キーボード
        Mouse, // マウス
        Pad // パッド
    }
    public UsingDevice currentDevice = UsingDevice.None;

    public static InputModeManager instance { get; private set; }

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

    // 現在のゲームパッドの情報を取得し、フラグの切り替えとカーソルの更新を行う
    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            SetKeyboardMode();
        }
        else if (Mouse.current != null && Mouse.current.delta.ReadValue() != Vector2.zero)
        {
            SetMouseMode();
        }
        else if (Gamepad.current != null)
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
        }
    }

    // ゲームパッド操作
    private void SetGamePadMode()
    {
        if (currentDevice == UsingDevice.Pad) return;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        currentDevice = UsingDevice.Pad;
    }

    // マウス操作
    private void SetMouseMode()
    {
        if (currentDevice == UsingDevice.Mouse) return;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        currentDevice = UsingDevice.Mouse;
    }

    private void SetKeyboardMode()
    {
        if (currentDevice == UsingDevice.Keyboard) return;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        currentDevice = UsingDevice.Keyboard;
    }

    // パッド使用中フラグの取得
    public bool CurrentUsingGamePad()
    {
        switch(currentDevice)
        {
            case UsingDevice.Pad:
                return true;

            default:
                return false;
        }
    }

    // キーボード使用中フラグの取得
    public bool CurrentUsingKeyboard()
    {
        switch(currentDevice)
        {
            case UsingDevice.Keyboard:
                return true;

            default:
                return false;
        }
    }

    // マウス使用中フラグの取得
    public bool CurrentUsingMouse()
    {
        switch(currentDevice)
        {
            case UsingDevice.Mouse:
                return true;

            default:
                return false;
        }
    }

    // カーソルポジションを強制的に変える
    public void WarpCursorPosition()
    {
        Mouse.current.WarpCursorPosition(new Vector2(2500, 2500));
    }

    // 最後に選択したオブジェクトの位置にカーソルを復旧する
    public void RestoreCursorPosition()
    {
        Vector3 currentSelectPos = SelectObjectArchive.instance.GetObjectArchive().GetComponent<RectTransform>().position;
        Vector2 newCursorPos =  new Vector2(currentSelectPos.x, currentSelectPos.y);
        Mouse.current.WarpCursorPosition(newCursorPos);
    }
}
