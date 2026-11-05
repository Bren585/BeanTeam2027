using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoloGameUIScript : MonoBehaviour
{
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnEnable()
    {
        GameResultManager.instance.GameStart();
    }

    private void OnDisable()
    {
        GameResultManager.instance.GameSet();
        CharacterSelectManager.instance.ResetCharacterData();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameResultManager.instance.IsGameSet())
        {
            ChangeResultScene();
        }
    }

    private void ChangeResultScene()
    {
        SceneTransitionManager.instance.LoadScene((int)SceneID.Result);
    }

    public void AddPlayerScore()
    {
        int randomRange = Random.Range(0, (CharacterSelectManager.instance.selectedCharacters.Length - 1));
        GameResultManager.instance.AddScore(GameResultManager.ScoreSide.Player, CharacterSelectManager.instance.selectedCharacters[randomRange].characterName);
        Debug.Log("Add PlayerScore");
    }

    public void AddEnemyScore()
    {
        GameResultManager.instance.AddScore(GameResultManager.ScoreSide.Enemy, "Bear");
        Debug.Log("Add EnemyScore");
    }
}
