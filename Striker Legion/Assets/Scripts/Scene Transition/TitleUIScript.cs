using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Unity.Mathematics;

public class TitleUIScript : MonoBehaviour
{
    // タイトル画面で最初に選択されるボタン」
    [SerializeField]
    GameObject firstSelectButton;

    // 選択中を示すカーソル
    [SerializeField]
    private RectTransform cursor;

    // カーソルのオフセット位置
    [SerializeField]
    private float cursorOffset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventSystem.current.SetSelectedGameObject(firstSelectButton);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCursorPosition();
    }

    // モード選択カーソルの移動
    public void UpdateCursorPosition()
    {
        GameObject selectedObject = EventSystem.current.currentSelectedGameObject;

        if (selectedObject == null) return;
        RectTransform selectedTransform = selectedObject.GetComponent<RectTransform>();
        float3 pos = selectedTransform.position;
        pos.x += cursorOffset;
        cursor.position = pos; 
    }

    // 1人用モードへの遷移
    public void SoloMode()
    {
        SceneTransitionManager.instance.LoadScene((int)SceneID.CharacterSelect);
    }

    // トレーニングモードへの遷移
    public void TrainingMode()
    {
        SceneTransitionManager.instance.LoadScene((int)SceneID.TrainingSelect);
    }

    // オプションモードへの遷移
    public void OptionMode()
    {
        SceneTransitionManager.instance.LoadScene((int)SceneID.Option);
    }
}
