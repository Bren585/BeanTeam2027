using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class CharacterSelectUIScript : MonoBehaviour
{
    // 現在のシーンの進捗
    enum SelectSceneState
    {
        None,
        CharacterSelect, // キャラクター選択中
        PositionSelect, // ポジション選択中
    }

    // キャラクター選択UIグループ
    [SerializeField]
    private CanvasGroup characterSelectCanvasGroup;

    // キャラクター選択の表示物
    [SerializeField]
    private GameObject characterSelectDisplayObjects;

    // キャラクター選択画面で始めに選択状態にするボタン
    private GameObject firstCharacterSelectButton;

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

    // 選択完了時に表示されるUI
    [SerializeField]
    GameObject completePanel;

    // 選択完了画面で始めに選択状態にするボタン
    [SerializeField]
    GameObject firstCompletePanelButton;

    // プロフィール画像表示用
    [SerializeField]
    Image profileImage;

    // キャラクター説明用
    [SerializeField]
    Text descriptionText;

    // 選択中のキャラクター情報
    private CharacterScriptableObject current;

    // キャラクター番号(ポジション選択時)
    private int characterIndex = -1;

    // キャラクターのポジション(ポジション選択時)
    private Character_Position posIndex = Character_Position.None;

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

    [SerializeField] TeamHandoff teamHandoff;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventSystem.current.SetSelectedGameObject(firstCharacterSelectButton);
        CharacterSelectManager.instance.Resize(characterButtons.Length);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCompletePanel();
        UpdateSelectedImage();
        UpdatePositionSelect();
    }

    // キャラクター選択にて選択中ボタンに合うイメージ画像の更新
    void UpdateSelectedImage()
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

    // 現在実行中の選択処理のチェック
    private void UpdateCompletePanel()
    {
        switch(CharacterSelectManager.instance.CurrentExecuteState())
        {
            // キャラクター選択中
            case ExecuteState.CharacterSelect:
                if(!characterSelectDisplayObjects.activeSelf)
                {
                    Debug.Log("Active CharacterSelect");
                    characterSelectDisplayObjects.SetActive(true);
                    SetCanvasGroup(characterSelectCanvasGroup, true);
                    SetCanvasGroup(positionButtonCanvasGroup, false);
                    SetCanvasGroup(characterButtonCanvasGroup, false);
                    completePanel.SetActive(false);
                    positionSelectDisplayObjects.SetActive(false);
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(firstCharacterSelectButton);
                }
                break;

            // ポジション選択中
            case ExecuteState.PositionSelect:
                if (!positionSelectDisplayObjects.activeSelf)
                {
                    Debug.Log("Active PositionSelect");

                    CharacterSelectButtonScript[] select_buttons = characterButtonCanvasGroup.GetComponentsInChildren<CharacterSelectButtonScript>();
                    for (int i = 0; i < 4; i++)
                    {
                        select_buttons[i].characterData = CharacterSelectManager.instance.selectedCharacters[i];
                    }

                    positionSelectDisplayObjects.SetActive(true);
                    SetCanvasGroup(characterSelectCanvasGroup, false);
                    SetCanvasGroup(positionButtonCanvasGroup, true);
                    SetCanvasGroup(characterButtonCanvasGroup, true);
                    completePanel.SetActive(false);
                    characterSelectDisplayObjects.SetActive(false);
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(firstSelectPositionButton);

                    SetCharacterButtonsOfPositionSelect();
                }
                if(!isButtonSelected)
                {
                    selectedOutline.SetActive(false);
                }

                break;

            //すべて選択済み
            case ExecuteState.Complete:
                if (!completePanel.activeSelf)
                {
                    Debug.Log("Active Complete");
                    completePanel.SetActive(true);
                    SetCanvasGroup(characterSelectCanvasGroup, false);
                    SetCanvasGroup(positionButtonCanvasGroup, false);
                    SetCanvasGroup(characterButtonCanvasGroup, false);
                    characterSelectDisplayObjects.SetActive(false);
                    positionSelectDisplayObjects.SetActive(false);
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(firstCompletePanelButton);
                }
                break;
        }
    }

    // キャラクター選択画面の初めに選択されるオブジェクトの設定
    public void SetFirstSelectObjectToCharacterSelect(GameObject setObject)
    {
        firstCharacterSelectButton = setObject;
    }

    // キャラクター選択の登録処理
    public void SelectCharacter()
    {
        if (current == null) return;
        CharacterSelectManager.instance.SetCharacter(current);
    }

    // 選択時のボイス再生
    private void PlaySelectedVoice()
    {
        //int lastIndex = CharacterSelectManager.instance.GetCharacterDataLength() - 1;
        //CharacterScriptableObject characterObject = CharacterSelectManager.instance.GetSelectedCharacterData(lastIndex);
        //AudioSource audioSource = null;
        //audioSource.PlayOneShot(characterObject.selectVoices[0]);
    }

    // キャンバスグループの設定処理
    private void SetCanvasGroup(CanvasGroup group, bool enable, float maxAlpha = 1f, float minAlpha = 0.5f)
    {
        group.interactable = enable;
        group.blocksRaycasts = enable;
        group.alpha = enable ? maxAlpha : minAlpha;
    }

    private void SetCharacterButtonsOfPositionSelect()
    {
        Debug.Log(characterButtons.Length.ToString());
        for (int i = 0; i < characterButtons.Length; ++i)
        {
            CharacterSelectButtonScript characterScript = characterButtons[i].GetComponent<CharacterSelectButtonScript>();
            if (characterScript)
                characterScript.SetDataFromSelected(i);
        }
    }

    // ポジション選択時にバインドするキャラクター番号
    public void BindingCharacterIndex(int index)
    {
        if (index >= CharacterSelectManager.instance.GetCharacterDataLength())
        {
            return;
        }
        else if (index < 0)
        {
            characterIndex = 0;
        }
        else
        {
            characterIndex = index;
        }
        // ボタンを選択済みにしてアウトラインの位置を固定
        if (!isButtonSelected)
        {
            isButtonSelected = true;
            selectedOutline.SetActive(true);
            GameObject currentObject = EventSystem.current.currentSelectedGameObject.gameObject;
            selectedOutline.GetComponent<RectTransform>().position = currentObject.GetComponent<RectTransform>().position;
            selectedOutline.GetComponent<RectTransform>().localScale = currentObject.GetComponent<RectTransform>().localScale;
        }
        // ボタンをロック
        SetCanvasGroup(characterButtonCanvasGroup, false, 1f, 0.5f);
        SetCanvasGroup(positionButtonCanvasGroup, true, 1f, 0.5f);
        EventSystem.current.SetSelectedGameObject(firstSelectPositionButton);
    }

    // ポジション選択時にバインドする担当ポジションの番号
    public void BindingPositionIndex(int positionIndex)
    {
        if (positionIndex > (int)Character_Position.Keeper) return;
        else if (positionIndex < (int)Character_Position.None) return;
        else
        {
            posIndex = (Character_Position)positionIndex;
            selectedButtonImage = EventSystem.current.currentSelectedGameObject.GetComponent<Image>();
        }
        // ボタンを選択済みにしてアウトラインの位置を固定
        if (!isButtonSelected)
        {
            isButtonSelected = true;
            selectedOutline.SetActive(true);
            GameObject currentObject = EventSystem.current.currentSelectedGameObject.gameObject;
            selectedOutline.GetComponent<RectTransform>().position = currentObject.GetComponent<RectTransform>().position;
            selectedOutline.GetComponent<RectTransform>().localScale = currentObject.GetComponent<RectTransform>().localScale;
        }
        // ボタンをロック
        SetCanvasGroup(characterButtonCanvasGroup, true, 1f, 0.5f);
        SetCanvasGroup(positionButtonCanvasGroup, false, 1f, 0.5f);
        EventSystem.current.SetSelectedGameObject(firstSelectCharacterButton);
    }

    // ポジション選択の更新
    private void UpdatePositionSelect()
    {
        // ポジションにキャラクターが未割当の際、ポジションボタンの画像を初期化
        for(int i = 0; i < (int)Character_Position.Other; ++i)
        {
        　   if(CharacterSelectManager.instance.GetPositionToCharacterData(i) == -1)
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

    // 選択を一つ戻る
    public void BackSelectItem(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        BackSelectItem();
    }

    public void BackSelectItem()
    {
        // ステートに合わせて切り替え
        switch (CharacterSelectManager.instance.CurrentExecuteState())
        {
            case ExecuteState.None:
                break;

            // キャラクター選択はキャラクター情報を戻す。出来なければ前のシーンに戻る
            case ExecuteState.CharacterSelect:
                if (!CharacterSelectManager.instance.DeleteLastCharacterData())
                {
                    SceneTransitionManager.instance.LoadPreviousScene();
                }
                break;

            // ポジション選択はポジション割り当てを戻す。出来なければキャラクター情報の最後尾を削除
            case ExecuteState.PositionSelect:
                if (!CharacterSelectManager.instance.DeleteLastPositionData())
                {
                    CharacterSelectManager.instance.DeleteLastCharacterData();
                }
                break;
            
            // 次シーンへの選択はポジション割り当ての最後尾を削除
            case ExecuteState.Complete:
                CharacterSelectManager.instance.DeleteLastPositionData();
                break;
        }
    }

    // 次のシーンの呼び出し
    public void ChangeNextScene()
    {
        teamHandoff.playerTeam.Clear();
        for (int i = 0; i < CharacterSelectManager.instance.selectedCharacters.Length; i++)
        {
            CharacterSelectManager man = CharacterSelectManager.instance;
            teamHandoff.playerTeam.Add((Character_Position)man.GetSelectedPositionData(i), man.selectedCharacters[i].characterInfo);
        }

        foreach (Character_Position role in Enum.GetValues(typeof(Character_Position)))
        {
            if (role == Character_Position.None || role == Character_Position.Other) continue;
            //teamHandoff.enemyTeam.Add(role, )
        }

        SceneTransitionManager.instance.LoadScene((int)SceneID.SoloGame);
    }
}
