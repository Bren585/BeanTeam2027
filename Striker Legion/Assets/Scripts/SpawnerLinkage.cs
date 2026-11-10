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

        //Debug.Log(name + ":" + position);
    }

    public void respawn()
    {
        Debug.Log(name + ":Respawn" + transform.position);
        character.transform.position = transform.position;
        //character.transform.position = position;
        character.transform.rotation = transform.rotation;
        //character.transform.rotation = rotation;
    }
}
