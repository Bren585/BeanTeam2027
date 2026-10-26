using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SkillBar : MonoBehaviour
{
    Dictionary<CharacterTeam, float> energyBars = new Dictionary<CharacterTeam, float>();

    [SerializeField] private SkillBarInfo barInfo;

    Slider test_slider;

    float secondTimer = 1.0f;

    public void RegisterTeam(CharacterTeam team) { energyBars[team] = barInfo.startingEnergy; }

    public float GetEnergy(CharacterTeam team) { return energyBars[team]; }

    static public SkillBar instance = null;

    private void Awake()
    {
        if      (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject); return;
    }


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
            
            Dictionary<CharacterTeam, float>.KeyCollection keys = energyBars.Keys;
            CharacterTeam[] teams = new CharacterTeam[keys.Count];
            int i = 0;

            foreach (CharacterTeam team in keys)
            {
              teams[i++] = team;
            }

            foreach (CharacterTeam team in teams)
            {
                Charge(barInfo.passiveGain, team);
            }
        }

        energyBars.TryGetValue(0, out float test_value);
        test_slider.value = test_value / barInfo.maxEnergy;
    }

    public bool Ready(CharacterTeam team) { return energyBars[team] >= barInfo.reqEnergy; }

    void Charge(float amount, CharacterTeam team) 
    {
        energyBars[team] = Mathf.Clamp(energyBars[team] + amount, 0, barInfo.maxEnergy);
    }
    
    public bool PayForSkill(CharacterTeam team) 
    {
        if (!Ready(team)) return false;
        Charge(-barInfo.maxEnergy, team);
        return true;
    }

    public void GainForTackle(CharacterTeam team) { Charge(barInfo.tackleGain, team); }
    public void GainForShoot(CharacterTeam team) { Charge(barInfo.shootGain, team); }

}
