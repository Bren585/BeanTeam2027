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
        if (character == null) { return; }

        character.transform.position = transform.position;
        character.transform.rotation = transform.rotation;

        // CharacterBaseのリスポーン処理
        character.Respawn();
    }
}
