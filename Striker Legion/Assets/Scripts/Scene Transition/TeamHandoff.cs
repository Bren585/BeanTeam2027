using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TeamHandoff", menuName = "Scriptable Objects/Team")]
public class TeamHandoff : ScriptableObject
{
    [SerializeField] public Dictionary<Character_Position, CharacterInfo> playerTeam = new();
    [SerializeField] public Dictionary<Character_Position, CharacterInfo> enemyTeam = new();
}
