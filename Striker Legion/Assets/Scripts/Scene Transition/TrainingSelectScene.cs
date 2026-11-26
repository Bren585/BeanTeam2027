using NUnit.Framework;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TrainingSelectScene : MonoBehaviour, IPointerEnterHandler
{
    // ボタンオブジェクト配列
    [SerializeField]
    private List<GameObject> Buttons;

    // 選択中を示すアウトライン
    [SerializeField]
    private Image outline;

    // モード名を表示するテキスト
    [SerializeField]
    private Text explanation1;

    // モード説明テキスト
    [SerializeField]
    private Text explanation2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SelectObjectArchive.instance.SetSubstituteObject(Buttons[0]);
    }

    // Update is called once per frame
    void Update()
    {
        GameObject selected = SelectObjectArchive.instance.GetObjectArchive();
        for (int i = 0; i < Buttons.Count; ++i )
        {
            // 選択中のボタンを取得
            if (selected == Buttons[i])
            {
                // アウトラインの位置を設定
                RectTransform selectedButtonTransform = selected.GetComponent<RectTransform>();
                float3 pos = selectedButtonTransform.position;
                outline.GetComponent<RectTransform>().position = pos;
                float3 scale = selectedButtonTransform.localScale;
                outline.GetComponent<RectTransform>().localScale = scale;

                // 説明の更新
                if (selected.TryGetComponent<ModeButton>(out var modeButton))
                {
                    SetExplanation(modeButton.GetExplanation());
                }
            }
        }
    }

    // 説明テキストを設定
    private void SetExplanation(ModeExplanationScriptableObject explanationObject)
    {
        explanation1.text = explanationObject.modeName;
        explanation2.text = explanationObject.explanation;
    }

    // パス練習シーンへの遷移
    public void PassTrainingMode()
    {
        SceneTransitionManager.instance.LoadScene((int)SceneID.PassTraining);
    }

    // カーソルで選択中ゲームオブジェクトを切り替える処理
    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
