using UnityEngine;
using UnityEngine.UI;

public class SkillBar : MonoBehaviour
{
    [field: SerializeField] public int teamNo { get; private set; }

    [field: SerializeField] public float energy { get; private set; }

    [SerializeField] private SkillBarInfo barInfo;

    Slider slider;

    float secondTimer = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        energy = 0.0f;
        slider = GetComponentInParent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        secondTimer -= Time.deltaTime;
        if (secondTimer < 0.0f) 
        {
            secondTimer += 1;
            Charge(barInfo.passiveGain);
        }
    }

    public bool Ready() { return energy >= barInfo.reqEnergy; }

    void Charge(float amount) 
    { 
        energy = Mathf.Clamp(energy + amount, 0, barInfo.maxEnergy);
        slider.value = energy / barInfo.maxEnergy;
    }
    
    public bool PayForSkill() 
    {
        if (!Ready()) return false;
        Charge(-barInfo.maxEnergy);
        return true;
    }

    public void GainForTackle() { Charge(barInfo.tackleGain); }
    public void GainForShoot() { Charge(barInfo.shootGain); }

    static public SkillBar GetSkillBar(int teamNo)
    {
        SkillBar[] bars = FindObjectsByType<SkillBar>();
        foreach (SkillBar bar in bars) { 
            if (bar.teamNo == teamNo) { return bar; }
        }
        return null;
    }
}
