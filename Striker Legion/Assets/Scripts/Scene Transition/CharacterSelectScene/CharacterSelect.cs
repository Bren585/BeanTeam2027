using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterSelect
{
    // キャラクター選択UIグループ
    [SerializeField]
    private CanvasGroup characterSelectCanvasGroup;

    // キャラクター選択の表示物
    [SerializeField]
    private GameObject characterSelectDisplayObjects;

    // キャラクター選択画面で始めに選択状態にするボタン
    [SerializeField]
    GameObject firstSelectCharacterButton;

    // プロフィール画像表示用
    [SerializeField]
    Image profileImage;

    // キャラクター説明用
    [SerializeField]
    Text descriptionText;

    // 選択中のキャラクター情報
    private CharacterScriptableObject current;

    // 有効かどうかのチェックフラグ
    bool is_active = false;

    public void Initialize()
    {
        is_active = true;
        EventSystem.current.SetSelectedGameObject(firstSelectCharacterButton);
    }

    public void UnInitialize()
    {
        is_active = false;
        characterSelectDisplayObjects.SetActive(false);
    }

    public void Update()
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;
        if (selected != null && selected.TryGetComponent<CharacterSelectButtonScript>(out var button))
        {
            current = button.characterData;
        }
        // 現在選ばれているオブジェクトがキャラクターボタンなら画像を更新
        if (current == null) return;
        profileImage.sprite = current.profileImage;
        descriptionText.text = current.description;
    }

    public void BackSelectItem()
    {
        if (!CharacterSelectManager.instance.DeleteLastCharacterData())
        {
            SceneTransitionManager.instance.LoadPreviousScene();
        }
    }

    public bool GetIsActive()
    {
        return is_active;
    }
}
