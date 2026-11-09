using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Scorer
{
    // 得点者名
    public string scorerName;
    // 得点を入れた際の時間
    public int time;

    public Scorer(string scorerName, int time)
    {
        this.scorerName = scorerName;
        this.time = time;
    }
}

public class GameResultManager : MonoBehaviour
{
    public static GameResultManager instance { get; private set; }

    // プレイヤー側総合得点
    private int playerSideTotalScore = 0;
    // プレイヤー側得点者リスト
    private List<Scorer> playerSideScorerList = new();

    // 敵側総合得点
    private int enemySideTotalScore = 0;
    // 敵側の得点者リスト
    private List<Scorer> enemySideScorerList = new();
    // 経過時間
    private float timer = 0;
    // タイマーのフラグ
    private bool isStart = false;

    // 得点側判別用
    public enum ScoreSide
    {
        Player,
        Enemy,
    }

    // 勝者
    public enum Winner
    {
        None,
        Draw, // 引き分け
        Player, // プレイヤー
        Enemy // エネミー
    }
    private Winner winner = Winner.None;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isStart)
            timer += Time.deltaTime;
    }

    // ゲーム開始時に呼び出す
    public void GameStart()
    {
        isStart = true;
    }

    // ゲーム終了時に呼び出す
    public void GameSet()
    {
        isStart = false;
        timer = 0;
    }

    // 得点の加算 : ScoreSide(加算される側), scorerName(得点者名)
    public void AddScore(ScoreSide side, string scorerName)
    {
        int matchTime = Mathf.FloorToInt(timer);
        switch (side)
        {
            case ScoreSide.Player:
                playerSideTotalScore++;
                playerSideScorerList.Add(new Scorer(scorerName, matchTime));
                break;

            case ScoreSide.Enemy:
                enemySideTotalScore++;
                enemySideScorerList.Add(new Scorer(scorerName, matchTime));
                break;
        }
    }

    // ゲーム終了かどうかを取得する処理
    public bool IsGameSet()
    {
        if (playerSideTotalScore >= 5)
        {
            winner = Winner.Player;
            return true;
        }
        else if(enemySideTotalScore >= 5)
        {
            winner = Winner.Enemy;
            return true;
        }
        else
            return false;
    }

    // スコア情報の初期化
    public void Remove()
    {
        playerSideTotalScore = 0;
        playerSideScorerList.Clear();
        enemySideTotalScore = 0;
        enemySideScorerList.Clear();
    }

    // プレイヤースコアの取得
    public int GetPlayerTotalScore()
    {
        return playerSideTotalScore;
    }

    // エネミースコア取得
    public int GetEnemyTotalScore()
    {
        return enemySideTotalScore;
    }

    // プレイヤー側得点者リスト取得
    public List<Scorer> GetPlayerSideScorerList()
    {
        return playerSideScorerList;
    }

    // エネミー側得点者リスト取得
    public List<Scorer> GetEnemySideScorerList()
    {
        return enemySideScorerList;
    }

    // 勝者情報の取得
    public int GetWinner()
    {
        return (int)winner;
    }
}
