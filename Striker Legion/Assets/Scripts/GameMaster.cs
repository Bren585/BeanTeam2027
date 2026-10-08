using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    float gametimer;
    Dictionary<int, int> scores;

    enum GameState { 
        CountDown,
        Gameplay,
        Finish
    };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SkillBar skillBar = FindAnyObjectByType<SkillBar>();
        skillBar.RegisterTeam(0);
        skillBar.RegisterTeam(1);
    }

    // Update is called once per frame
    void Update()
    {
        gametimer += Time.deltaTime;
    }
}
