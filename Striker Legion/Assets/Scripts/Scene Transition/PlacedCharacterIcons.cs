using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlacedCharacterIcons : MonoBehaviour
{
    [SerializeField]
    private GameObject imagePrefab;
    public Vector2 centerPosition;
    public int colCount;
    public int colOffset;
    public int rowOffset;
    public bool placedEnemySide;

    private void Awake()
    {
        PlacedPlayerSideCharacters();

        //if(placedEnemySide)
        //{
        //    PlacedEnemySideCharacters();
        //}
    }

    // プレイヤー側キャラキターアイコンを追加
    private void PlacedPlayerSideCharacters()
    {
        int count = 0;

        foreach (CharacterScriptableObject character
            in CharacterSelectManager.instance.selectedCharacters)
        {
            GameObject newImage = Instantiate(imagePrefab);
            
            newImage.transform.SetParent(transform, false);

            int col = count % colCount;
            int row = count / colCount;

            float rowShift = (row % 2 == 1) ? colOffset * 0.5f : 0f;

            float x = centerPosition.x +
                      (col - (colCount - 1) * 0.5f) * colOffset +
                      rowShift;

            float y = centerPosition.y -
                      row * rowOffset;

            // 配置
            newImage.GetComponent<RectTransform>().anchoredPosition =
                new Vector2(x, y);

            string positionText;
            switch(character.position)
            {
                case Character_Position.Forward:
                    positionText = "Forward";
                    break;

                case Character_Position.MidFielder:
                    positionText = "MidFieldes";
                    break;

                case Character_Position.Defender:
                    positionText = "Defender";
                    break;

                case Character_Position.Keeper:
                    positionText = "Keeper";
                    break;

                default:
                    positionText = "None";
                    break;
            }
            
            newImage.name = character.name;
            newImage.GetComponent<Image>().sprite = character.icon;
            newImage.GetComponentInChildren<Text>().text = positionText;
            newImage.SetActive(true);

            count++;
        }
    }

    private void PlacedEnemySideCharacters()
    {

    }
}
