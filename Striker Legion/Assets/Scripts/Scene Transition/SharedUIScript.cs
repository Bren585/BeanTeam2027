using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SharedUIScript : MonoBehaviour
{
    // マウス操作時の入力ガイド
    [SerializeField]
    private GameObject[] mousemodeInputGuides;

    // パッド操作時の入力ガイド
    [SerializeField]
    private GameObject[] padmodeInputGuides;

    // 現在の入力媒体がPadであるかのチェックフラグ
    private bool isInputPad = false;

    // Update is called once per frame
    void Update()
    {
        ChangeGuideUI();
    }

    // 前のシーンに戻る
    public void SceneBackEvent()
    {
        SceneTransitionManager.instance.LoadPreviousScene();
    }

    // 操作方法表示の切り替え
    public void ChangeGuideUI()
    {
        if (InputModeManager.instance.GetUsedGamePadFlag())
        {
            if(!isInputPad)
            {
                for(int mIndex = 0; mIndex < mousemodeInputGuides.Length; ++mIndex)
                {
                    mousemodeInputGuides[mIndex].SetActive(false);
                }

                for(int pIndex = 0; pIndex < padmodeInputGuides.Length; ++pIndex)
                {
                    padmodeInputGuides[pIndex].SetActive(true);
                }

                isInputPad = true;
            }
        }
        else
        {
            if(isInputPad)
            {
                for (int mIndex = 0; mIndex < mousemodeInputGuides.Length; ++mIndex)
                {
                    mousemodeInputGuides[mIndex].SetActive(true);
                }

                for (int pIndex = 0; pIndex < padmodeInputGuides.Length; ++pIndex)
                {
                    padmodeInputGuides[pIndex].SetActive(false);
                }

                isInputPad = false;
            }
        }
    }
}
