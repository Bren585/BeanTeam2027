using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Unity.Mathematics;

public class TitleUIScript : MonoBehaviour
{
    [SerializeField]
    GameObject firstSelectButton;

    [SerializeField]
    private RectTransform cursor;

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
