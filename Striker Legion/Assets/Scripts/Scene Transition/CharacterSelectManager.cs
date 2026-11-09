using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    // public int nextSceneId;
    // 選択されたキャラクターの配列
    public CharacterScriptableObject[] selectedCharacters;
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

    // キャラクターにポジションを割り当てる
    public void SetPositionData(int characterIndex, Character_Position pos_index)
    {
        if (characterIndex >= selectedCharacters.Length) return;
        selectedCharacters[characterIndex].position = pos_index;
    }

    // ポジション配列から見たキャラクターインデックスを保管
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
        if (index >= selectedCharacters.Length || index < 0) return null;
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
        //if(SceneTransitionManager.instance)

        // キャラクター選択
        for (int i = selectedCharacters.Length - 1; i >= 0; --i)
        {
            if (!selectedCharacters[i])
            {
                return ExecuteState.CharacterSelect;
            }
        }
        // ポジション選択
        for (int i = selectedCharacters.Length - 1; i >= 0; --i)
        {
            if (selectedCharacters[i].position == Character_Position.None)
            {
                return ExecuteState.PositionSelect;
            }
        }
        // 次へ
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
