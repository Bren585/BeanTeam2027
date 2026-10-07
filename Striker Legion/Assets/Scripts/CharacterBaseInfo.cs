using UnityEngine;

[CreateAssetMenu(fileName = "CharacterBaseInfo", menuName = "Scriptable Objects/CharacterBaseInfo")]
public class CharacterBaseInfo : ScriptableObject
{
    [Header("CharacterBaseInfo")]
    public float minTurnSpeed = 90;
    public float maxTurnSpeed = 360;
    public float tackleHitboxDuration = 0.25f;
    public float tackleCooldown = 1.0f;
    public float maxDownTime = 2.0f;
    public float maxPassDistance = 10.0f;
    public float gravity = 5.0f;
    public float jumpStrength = 2.0f;
}
