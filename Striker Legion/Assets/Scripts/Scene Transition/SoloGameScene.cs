using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SoloGameScene : MonoBehaviour
{
    [SerializeField]
    PauseMenuScript pause;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		GameResultManager.instance.GameStart();
	}

    private void OnEnable()
    {
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

    // リザルト画面への遷移
    private void ChangeResultScene()
    {
        SceneTransitionManager.instance.LoadScene((int)SceneID.Result);
    }

    // プレイヤー側のスコア加算
    public void AddPlayerScore()
    {
        int randomRange = Random.Range(0, (CharacterSelectManager.instance.selectedCharacters.Length - 1));
        GameResultManager.instance.AddScore(GameResultManager.ScoreSide.Player, CharacterSelectManager.instance.selectedCharacters[randomRange].characterName);
        Debug.Log("Add PlayerScore");
    }

    // エネミー側のスコア加算
    public void AddEnemyScore()
    {
        GameResultManager.instance.AddScore(GameResultManager.ScoreSide.Enemy, "Bear");
        Debug.Log("Add EnemyScore");
    }
}
