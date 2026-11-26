using UnityEngine;
using UnityEngine.EventSystems;

public class TrainingScene : MonoBehaviour
{
    [SerializeField]
    PauseMenuScript pause;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(!pause.GetIsPause())
        {
            SelectObjectArchive.instance.SetSubstituteObject(null);
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
