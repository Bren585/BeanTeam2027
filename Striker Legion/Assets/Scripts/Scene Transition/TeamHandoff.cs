using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TeamHandoff", menuName = "Scriptable Objects/Team")]
public class TeamHandoff : ScriptableObject
{
    public Dictionary<Character_Position, CharacterInfo> playerTeam;
    public Dictionary<Character_Position, CharacterInfo> enemyTeam;
}
