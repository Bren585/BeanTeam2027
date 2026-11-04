using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadAllCharacterIcons : MonoBehaviour
{
    public GameObject iconPrefab;
    public List<CharacterScriptableObject> AllCharacters;

    public int start_x;
    public int start_y;

    public int col_count;
    public int col_offset;
    public int row_offset;

    public void Awake()
    {
        CharacterSelectUIScript selectScript = FindAnyObjectByType<CharacterSelectUIScript>();
        int count = 0;
        foreach (CharacterScriptableObject chara in AllCharacters)
        {
            GameObject newIcon = Instantiate(iconPrefab);
            newIcon.transform.SetParent(gameObject.transform, true);

            Vector3 pos = new Vector3(start_x, start_y, 0);
            pos.x += col_offset * (count % col_count);
            pos.y -= row_offset * Mathf.Floor(count / col_count);
            newIcon.transform.localPosition = pos; 

            newIcon.name = chara.name;
            newIcon.GetComponent<CharacterSelectButtonScript>().characterData = chara;
            newIcon.GetComponent<Image>().sprite = chara.icon;
            newIcon.GetComponent<Button>().onClick.AddListener(selectScript.SelectCharacter);
            
            newIcon.SetActive(true);

            count++;
        }
    }
}
