using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SharedUIScript : MonoBehaviour
{
    [SerializeField]
    private InputSystemUIInputModule uiInputModule;
    private bool updatedMauseEnable = false;
    private bool updatedMauseDisable = false;
    // Update is called once per frame
    void Update()
    {
        UpdateMouseHover();
    }

    private void UpdateMouseHover()
    {
        if (InputModeManager.instance.CurrentUsingMouse())
        {
            // マウス操作時
            if(!updatedMauseEnable)
            {
                updatedMauseDisable = false;
                updatedMauseEnable = true;
                InputModeManager.instance.RestoreCursorPosition();
                uiInputModule.point.action.Enable();
            }
        }
        else
        {
            if(!updatedMauseDisable)
            {
                updatedMauseDisable = true;
                updatedMauseEnable = false;
                InputModeManager.instance.WarpCursorPosition();
                //EventSystem.current.SetSelectedGameObject(null);

                //Canvas.ForceUpdateCanvases();
                uiInputModule.point.action.Disable();
            }
        }
    }

    // 前のシーンに戻る
    public void SceneBackEvent()
    {
        SceneTransitionManager.instance.LoadPreviousScene();
    }
}
