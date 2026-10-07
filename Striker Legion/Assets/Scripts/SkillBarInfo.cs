using UnityEngine;

[CreateAssetMenu(fileName = "SkillBarInfo", menuName = "Scriptable Objects/SkillBarInfo")]
public class SkillBarInfo : ScriptableObject
{
    [Header("SkillBarInfo")]
    public float maxEnergy = 200;
    public float reqEnergy = 100;
    public float passiveGain = 2;
    public float tackleGain = 5;
    public float shootGain = 50;
}
