using UnityEngine;
using UnityEngine.WSA;

[CreateAssetMenu(fileName = "SkillEffectBase", menuName = "Scriptable Objects/SkillEffectBase")]
public class SkillEffectBase : ScriptableObject
{
    public virtual void Activate(CharacterBase user) { }
}
