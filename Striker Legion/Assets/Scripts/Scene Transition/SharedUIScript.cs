using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SharedUIScript : MonoBehaviour
{
    // キーボードの戻るボタンガイド画像
    //[SerializeField]
    //private GameObject keyboardBackButtonGuide;
    // パッドの戻るボタンガイド画像
    //[SerializeField]
    //private GameObject gamepadBackButtonGuide;

    private void Start()
    {
        //bool connectedGamepad = SceneTransitionManager.instance.GetFinallyDeviceInfo();

        //keyboardBackButtonGuide.SetActive(!connectedGamepad);
        //gamepadBackButtonGuide.SetActive(connectedGamepad);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 前のシーンに戻る
    public void SceneBackEvent()
    {
        SceneTransitionManager.instance.LoadPreviousScene();
    }

    // 操作方法表示の切り替え
    public void ChangeGuideUI(PlayerInput playerInput)
    {
        //if (playerInput == null) return;
        //bool connectedGamepad = (playerInput.currentControlScheme == "Gamepad");

        //keyboardBackButtonGuide.SetActive(!connectedGamepad);
        //gamepadBackButtonGuide.SetActive(connectedGamepad);
    }
}
