using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PositionSelect
{
    // ポジション選択の表示物
    [SerializeField]
    private GameObject positionSelectDisplayObjects;

    // ポジション選択画面でボタンUIを制御する用キャンバスグループ
    [SerializeField]
    private CanvasGroup positionButtonCanvasGroup;
    [SerializeField]
    private CanvasGroup characterButtonCanvasGroup;

    // 片方が選択されると設定されるボタン
    [SerializeField]
    private GameObject firstSelectPositionButton;
    [SerializeField]
    private GameObject firstSelectCharacterButton;

    // 選択したボタンを分かりやすくする用のアウトライン
    [SerializeField]
    private GameObject selectedOutline;
    private bool isButtonSelected = false;

    // ポジション選択ボタンのイメージコンポーネント
    [SerializeField]
    private Image[] positionButtonImages;

    [SerializeField]
    private Button[] characterButtons;

    // デフォルトのボタン用スプライト画像
    [SerializeField]
    private Sprite defaultButtonSprite;

    // 選択したボタンのイメージコンポーネント
    private Image selectedButtonImage;

    // キャラクター番号(ポジション選択時)
    private int characterIndex = -1;

    // キャラクターのポジション(ポジション選択時)
    private Character_Position posIndex = Character_Position.None;

    // 有効かどうかのチェックフラグ
    bool is_active = false;

    public void Initialize()
    {
        is_active = true;
        positionSelectDisplayObjects.SetActive(true);
        SetCanvasGroup(positionButtonCanvasGroup, true);
        SetCanvasGroup(characterButtonCanvasGroup, true);
        EventSystem.current.SetSelectedGameObject(firstSelectPositionButton);

        for (int i = 0; i < characterButtons.Length; ++i)
        {
            CharacterSelectButtonScript characterScript = characterButtons[i].GetComponent<CharacterSelectButtonScript>();
            if (characterScript)
                characterScript.SetDataFromSelected(i);
        }
    }

    public void UnInitialize()
    {
        is_active = false;
    }

    public void Update()
    {
        // ポジションにキャラクターが未割当の際、ポジションボタンの画像を初期化
        for (int i = 0; i < (int)Character_Position.Other; ++i)
        {
            if (CharacterSelectManager.instance.GetPositionToCharacterData(i) == -1)
            {
                positionButtonImages[i].sprite = defaultButtonSprite;
            }
        }

        // 未選択の状態でないかの確認
        if (posIndex == Character_Position.None || characterIndex < 0) return;
        else
        {
            // ポジション重複チェック
            int oldPosition = 0;
            for (int i = 0; i < CharacterSelectManager.instance.GetPositionDataLength(); ++i)
            {
                oldPosition = CharacterSelectManager.instance.GetSelectedPositionData(i);
                if (oldPosition == (int)posIndex)
                {
                    CharacterSelectManager.instance.SetPositionData(i, Character_Position.None);
                    CharacterSelectManager.instance.SetPositionToCharacterData(-1, posIndex);
                    break;
                }
            }

            // データチェック
            oldPosition = CharacterSelectManager.instance.GetSelectedPositionData(characterIndex);
            if (oldPosition != -1)
            {
                CharacterSelectManager.instance.SetPositionData(characterIndex, Character_Position.None);
                CharacterSelectManager.instance.SetPositionToCharacterData(-1, posIndex);
                positionButtonImages[oldPosition].sprite = defaultButtonSprite;
            }
            // 対応する番号にポジション情報を登録
            CharacterSelectManager.instance.SetPositionData(characterIndex, posIndex);
            CharacterSelectManager.instance.SetPositionToCharacterData(characterIndex, posIndex);
            CharacterScriptableObject currentCharacter = CharacterSelectManager.instance.GetSelectedCharacterData(characterIndex);
            selectedButtonImage.sprite = currentCharacter.icon;
            // 値を初期化
            characterIndex = -1;
            posIndex = Character_Position.None;
            selectedButtonImage = null;
            isButtonSelected = false;
            SetCanvasGroup(characterButtonCanvasGroup, true);
            SetCanvasGroup(positionButtonCanvasGroup, true);
        }
    }

    private void SetCanvasGroup(CanvasGroup group, bool enable, float maxAlpha = 1f, float minAlpha = 0.5f)
    {
        group.interactable = enable;
        group.blocksRaycasts = enable;
        group.alpha = enable ? maxAlpha : minAlpha;
    }

    public void BackSelectItem()
    {
        if (!CharacterSelectManager.instance.DeleteLastPositionData())
        {
            CharacterSelectManager.instance.DeleteLastCharacterData();
        }
    }

    public bool GetIsActive()
    {
        return is_active;
    }
}
