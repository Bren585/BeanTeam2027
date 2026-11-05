using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseMenuScript : MonoBehaviour
{
    [SerializeField]
    GameObject pausePanel;

    [SerializeField]
    GameObject firstSelectButton;

    [SerializeField]
    PlayerInput playerInput;

    private bool isPause = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DeActivation();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TogglePause()
    {
        if (!isPause)
        {
            Activation();
            Debug.Log("Pause");
        }
        else
        {
            DeActivation();
        }
    }

    public void Resume()
    {
        if(isPause)
        {
            DeActivation();
        }
    }

    private void Activation()
    {
        isPause = true;
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstSelectButton);
    }

    private void DeActivation()
    {
        isPause = false;
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
    }

    public void ChangeTitleScene()
    {
        DeActivation();
        GameResultManager.instance.Remove();
        SceneTransitionManager.instance.LoadScene((int)SceneID.Title);
    }

    public void ChangeOptionScene()
    {
        DeActivation();
        SceneTransitionManager.instance.LoadScene((int)SceneID.Option);
    }
}
