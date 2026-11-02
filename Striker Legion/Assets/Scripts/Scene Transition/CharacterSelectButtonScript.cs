using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class CharacterSelectButtonScript : MonoBehaviour
{
    public enum DisplayType
    {
        SelectedIndex,
        CharacterName,
    }

    [SerializeField]
    public CharacterScriptableObject characterData;

    [SerializeField]
    int characterDataIndex = -1;

    [SerializeField]
    private Image iconImage;

    [SerializeField]
    private DisplayType displayType;

    private bool can_update = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    private void OnEnable()
    {
        int length = CharacterSelectManager.instance.GetCharacterDataLength() - 1;
        if (characterDataIndex < 0 || characterDataIndex > length)
        {
            iconImage.sprite = characterData.icon;
        }
        else
        {
            if (CharacterSelectManager.instance.GetSelectedCharacterData(characterDataIndex) == null)
            {
                iconImage.sprite = characterData.icon;
                Debug.Log("select manager is null");
            }
            else
            {
                characterData = CharacterSelectManager.instance.GetSelectedCharacterData(characterDataIndex);
                iconImage.sprite = characterData.icon;
            }
        }

        if (iconImage.sprite != null) can_update = true;
    }

    private void Update()
    {
        if (!can_update) return;
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
