using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

public class SkillBar : MonoBehaviour
{
    Dictionary<int, float> energyBars = new Dictionary<int, float>();

    [SerializeField] private SkillBarInfo barInfo;

    Slider test_slider;

    float secondTimer = 1.0f;

    public void RegisterTeam(int teamNo) { energyBars[teamNo] = barInfo.startingEnergy; }

    public float GetEnergy(int teamNo) { return energyBars[teamNo]; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        test_slider = FindAnyObjectByType<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        secondTimer -= Time.deltaTime;
        if (secondTimer < 0.0f)
        {
            secondTimer += 1;
            
            Dictionary<int, float>.KeyCollection keys = energyBars.Keys;
            int[] teams = new int[keys.Count];
            int i = 0;

            foreach (int teamNo in keys)
            {
              teams[i++] = teamNo;
            }

            foreach (int teamNo in teams)
            {
                Charge(barInfo.passiveGain, teamNo);
            }
        }

        energyBars.TryGetValue(0, out float test_value);
        test_slider.value = test_value / barInfo.maxEnergy;
    }

    public bool Ready(int teamNo) { return energyBars[teamNo] >= barInfo.reqEnergy; }

    void Charge(float amount, int teamNo) 
    {
        energyBars[teamNo] = Mathf.Clamp(energyBars[teamNo] + amount, 0, barInfo.maxEnergy);
    }
    
    public bool PayForSkill(int teamNo) 
    {
        if (!Ready(teamNo)) return false;
        Charge(-barInfo.maxEnergy, teamNo);
        return true;
    }

    public void GainForTackle(int teamNo) { Charge(barInfo.tackleGain, teamNo); }
    public void GainForShoot(int teamNo) { Charge(barInfo.shootGain, teamNo); }

}
