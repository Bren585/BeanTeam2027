using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class CharacterSelectButtonScript : MonoBehaviour
{
    public enum DisplayType
    {
        SelectedIndex, // 選択順表示
        CharacterName, // キャラクター名表示
    }

    // 選択時に代入されるキャラクター情報オブジェクト
    [SerializeField]
    public CharacterScriptableObject characterData;
    // キャラクターアイコン画像
    private Image iconImage;
    // 情報の表示タイプ
    [SerializeField]
    private DisplayType displayType;
    // 更新可能かのフラグ
    private bool can_update = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // キャラクターアイコンのイメージ画像設定
    private void OnEnable()
    {
        iconImage = GetComponent<Image>();

        if(characterData)
            iconImage.sprite = characterData.icon;
        
        // 画像設定できてなければ更新しない
        if (iconImage.sprite != null) can_update = true;
    }

    // 選出済みキャラクターをポジション選択の際に表示するために使う
    public void SetDataFromSelected(int index)
    {
        // 範囲チェック
        int length = CharacterSelectManager.instance.GetCharacterDataLength() - 1;
        if (index < 0 || index > length) return;
        // データチェック
        CharacterScriptableObject characterObject = CharacterSelectManager.instance.GetSelectedCharacterData(index);
        if (characterObject != null)
        {
            characterData = characterObject;
            iconImage.sprite = characterData.icon;
        }
        // 画像設定できてなければ更新しない
        if (iconImage.sprite != null) can_update = true;
    }

    private void Update()
    {
        if (!can_update) return;
        // 表示の切り替え
        switch (displayType)
        {
            case DisplayType.SelectedIndex:
                DisplaySelectedIndex();
                break;

            case DisplayType.CharacterName:
                DisplayCharacterName();
                break;
        }
    }

    // 選択順の表示
    private void DisplaySelectedIndex()
    {
        Text selectIndex = iconImage.GetComponentInChildren<Text>();
        if (selectIndex == null)
        {
            return;
        }

        // ボタンの上に選択順を表示
        int index = CharacterSelectManager.instance.GetSelectedIndex(characterData);
        if (index <= 0)
        {
            selectIndex.text = "";
        }
        else
        {
            selectIndex.text = index.ToString();
        }
    }

    // キャラクター名の表示
    private void DisplayCharacterName()
    {
        Text characterName = iconImage.GetComponentInChildren<Text>();
        if (characterName == null)
        {
            return;
        }
        characterName.text = characterData.characterName;
    }
}
