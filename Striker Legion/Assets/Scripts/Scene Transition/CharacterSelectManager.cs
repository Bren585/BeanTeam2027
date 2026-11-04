using System;
using System.Linq;
using UnityEngine;

// 自軍キャラの担当ポジション
public enum Character_Position
{
    None = -1,
    Forward = 0,
    MidFielder = 1,
    Defender = 2,
    Keeper = 3,
    Other = 4,
}

public enum ExecuteState
{
    None,
    CharacterSelect,
    PositionSelect,
    Complete
}


public class CharacterSelectManager : MonoBehaviour
{ 
    public static CharacterSelectManager instance {  get; private set; }
    public int nextSceneId;
    public CharacterScriptableObject[] selectedCharacters;
    // キャラクターに割り振られたポジションの番号
    // public int[] selectedPositionIndexs;
    // ポジションから見たキャラクターの割り当て
    public int[] positionToCharacter;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        selectedCharacters = new CharacterScriptableObject[1];
        ResetCharacterData();
        ResetPositionData();
    }

    // 選択可能キャラ数の変更
    public void Resize(int size)
    {
        Array.Resize(ref selectedCharacters, size);
        ResetCharacterData();
        //Array.Resize(ref selectedPositionIndexs, size);
        Array.Resize(ref positionToCharacter, (int)Character_Position.Other);
        ResetPositionData();
    }

    // 選択したキャラクターのデータを配列に追加
    public void SetCharacter(CharacterScriptableObject set)
    {
        if(selectedCharacters.Contains(set))
        {
            Debug.Log("selected");
            return;
        }

        for (int i = 0; i < selectedCharacters.Length; i++)
        {
            if (selectedCharacters[i] == null)
            {
                selectedCharacters[i] = set;
                return;
            }
        }
    }

    public void SetPositionData(int characterIndex, Character_Position pos_index)
    {
        //if (characterIndex >= selectedPositionIndexs.Length) return;
        //selectedPositionIndexs[characterIndex] = (int)pos_index;

        if (characterIndex >= selectedCharacters.Length) return;
        selectedCharacters[characterIndex].position = pos_index;
    }

    public void SetPositionToCharacterData(int characterIndex, Character_Position pos_index)
    {
        if (pos_index != Character_Position.None)
        {
            int index = (int)pos_index;
            positionToCharacter[index] = characterIndex;
        }
    }

    // キャラクター選択情報の取得
    public CharacterScriptableObject GetSelectedCharacterData(int index)
    {
        if (index >= selectedCharacters.Length) return null;
        else return selectedCharacters[index];
    }

    // キャラクター選択情報のデータ長取得
    public int GetCharacterDataLength()
    {
        return selectedCharacters.Length;
    }

    // ポジション選択情報の取得
    public int GetSelectedPositionData(int index)
    {
        //if (index >= selectedPositionIndexs.Length) return (int)Character_Position.None;
        //else return selectedPositionIndexs[index];
        if (index >= selectedCharacters.Length) return (int)Character_Position.None;
        else return (int)selectedCharacters[index].position;
    }

    public int GetPositionToCharacterData(int index)
    {
        if (index > (int)Character_Position.Keeper) return (int)Character_Position.None;
        else return positionToCharacter[index];
    }

    // ポジション選択情報のデータ長取得
    public int GetPositionDataLength()
    {
        //return selectedPositionIndexs.Length;
        
        return selectedCharacters.Length;
    }

    // 選択順の番号取得
    public int GetSelectedIndex(CharacterScriptableObject obj)
    {
        for(int i = 0; i < selectedCharacters.Length; ++i)
        {
            if (selectedCharacters[i] != obj)
            {
                continue;
            }
            else
            {
                return (i + 1);
            }
        }
        return 0;
    }

    // 実行中のステート
    public ExecuteState CurrentExecuteState()
    {
        for (int i = selectedCharacters.Length - 1; i >= 0; --i)
        {
            if (selectedCharacters[i] == null)
            {
                return ExecuteState.CharacterSelect;
            }
        }

        //for(int i = selectedPositionIndexs.Length - 1; i >= 0; --i)
        //{
        //    if (selectedPositionIndexs[i] == (int)Character_Position.None)
        //    {
        //        return ExecuteState.PositionSelect;
        //    }
        //    int index = selectedPositionIndexs[i];
        //}

        for (int i = selectedCharacters.Length - 1; i >= 0; --i)
        {
            if (selectedCharacters[i].position == Character_Position.None)
            {
                return ExecuteState.PositionSelect;
            }
        }

        return ExecuteState.Complete;
    }

    // 一番後ろのキャラクター情報を null
    public bool DeleteLastCharacterData()
    {
        for(int i = selectedCharacters.Length - 1; i >= 0; --i)
        {
            if (selectedCharacters[i] != null)
            {
                selectedCharacters[i] = null;
                return true;
            }
        }
        return false;
    }

    // キャラクター選択情報の初期化
    public void ResetCharacterData()
    {
        for (int i = 0; i < selectedCharacters.Length; ++i)
        {
            selectedCharacters[i] = null;
        }
    }

    // 一番後ろのポジション情報を null
    public bool DeleteLastPositionData()
    {
        //for (int i = selectedPositionIndexs.Length - 1; i >= 0; --i)
        //{
        //    if (selectedPositionIndexs[i] != (int)Character_Position.None)
        //    {
        //        int currentPosIndex = selectedPositionIndexs[i];
        //        selectedPositionIndexs[i] = (int)Character_Position.None;
        //        positionToCharacter[currentPosIndex] = -1;
        //        return true;
        //    }
        //}

        for (int i = selectedCharacters.Length - 1; i >= 0; --i)
        {
            if (selectedCharacters[i].position != Character_Position.None)
            {
                int currentPosIndex = (int)selectedCharacters[i].position;
                selectedCharacters[i].position = Character_Position.None;
                positionToCharacter[currentPosIndex] = -1;
                return true;
            }
        }

        for (int j = 0; j < (int)Character_Position.Other; ++j)
        {
            positionToCharacter[j] = -1;
        }
        return false;
    }

    // ポジション選択情報の初期化
    public void ResetPositionData()
    {
        //for (int i = 0; i < selectedPositionIndexs.Length; ++i)
        //{
        //    selectedPositionIndexs[i] = (int)Character_Position.None;
        //}

        if (selectedCharacters[0] == null) return;

        for (int i = 0; i < selectedCharacters.Length; ++i)
        {
            selectedCharacters[i].position = Character_Position.None;
        }

        for (int j = 0; j < (int)positionToCharacter.Length; ++j)
        {
            positionToCharacter[j] = -1;
        }
    }
}
