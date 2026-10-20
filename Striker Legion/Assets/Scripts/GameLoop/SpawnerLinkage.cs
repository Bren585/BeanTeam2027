using UnityEngine;

public class SpawnerLinkage : MonoBehaviour
{
    [SerializeField] int teamNo;
    [SerializeField] CharacterRole role;

    [SerializeField] public CharacterBase character;
    Vector3 position;
    Quaternion rotation;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        position = transform.position; 
        rotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void respawn()
    {
        character.transform.position = position;
        character.transform.rotation = rotation;
    }
}
