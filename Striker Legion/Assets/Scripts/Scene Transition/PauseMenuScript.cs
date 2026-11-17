using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseMenuScript : MonoBehaviour
{
    // ポーズ時に表示されるパネルオブジェクト
    [SerializeField]
    GameObject pausePanel;

    // ポーズ移行後に初めに選択されるボタン
    [SerializeField]
    GameObject firstSelectButton;

    // ポーズ中フラグ
    public bool isPause = false;

    // アクションマップを持つPlayerInput
    [SerializeField]
    PlayerInput playerInput;

    // メニューの入力アクション
    private InputAction menuAction;

    private void Awake()
    {
        menuAction = playerInput.actions["Menu"];
    }

    private void OnEnable()
    {
        menuAction.performed += TogglePause;
    }

    private void OnDisable()
    {
        menuAction.performed -= TogglePause;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DeActivation();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // ポーズ切り替え処理
    public void TogglePause(InputAction.CallbackContext context)
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

    // ゲーム再開
    public void Resume()
    {
        if(isPause)
        {
            DeActivation();
        }
    }

    //ポーズ有効化
    private void Activation()
    {
        playerInput.SwitchCurrentActionMap("UI");
        isPause = true;
        Time.timeScale = 0f; // ゲーム内タイマーストップ
        pausePanel.SetActive(true); // ポーズパネルを有効化
        EventSystem.current.SetSelectedGameObject(firstSelectButton); // ボタンを選択オブジェクトに設定
    }

    // ポーズ非有効化
    private void DeActivation()
    {
        playerInput.SwitchCurrentActionMap("Player");
        isPause = false;
        Time.timeScale = 1f; // タイマー再開
        pausePanel.SetActive(false); // ポーズパネルを非有効化
    }

    // タイトル画面遷移処理
    public void ChangeTitleScene()
    {
        DeActivation();
        GameResultManager.instance.Remove();
        SceneTransitionManager.instance.LoadScene((int)SceneID.Title);
    }

    // オプション画面遷移処理
    public void ChangeOptionScene()
    {
        DeActivation();
        SceneTransitionManager.instance.LoadScene((int)SceneID.Option);
    }

    public bool GetIsPause()
    {
        return isPause;
    }
}
