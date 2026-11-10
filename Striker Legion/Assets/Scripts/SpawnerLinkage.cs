using UnityEngine;

public class SpawnerLinkage : MonoBehaviour
{
    [SerializeField] private CharacterTeam team;
    [SerializeField] private CharacterRole role;

    public CharacterTeam Team => team;
    public CharacterRole Role => role;

    [SerializeField] public CharacterBase character;
    Vector3 position;
    Quaternion rotation;
   
    void Awake()
    {
        position = transform.position; 
        rotation = transform.rotation;
    }

    public void respawn()
    {
        character.transform.position = position;
        character.transform.rotation = rotation;
    }
}
