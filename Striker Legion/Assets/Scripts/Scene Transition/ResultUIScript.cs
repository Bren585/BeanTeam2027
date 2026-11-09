using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResultUIScript : MonoBehaviour
{
    // リザルト表示グループ
    [SerializeField]
    private CanvasGroup resultCanvasGroup;
    // 選択画面グループ
    [SerializeField]
    private CanvasGroup selectCanvasGroup;
    [SerializeField]
    private GameObject selectCanvasObject;

    // 最初に選択状態になるボタン
    [SerializeField]
    private GameObject firstSelectResultButton;
    // 選択画面で最初に選択されているボタン
    [SerializeField]
    private GameObject firstSelectNextButton;

    // プレイヤー関連テキスト
    [SerializeField]
    private Text playerTotalPointText;
    [SerializeField]
    private Text playerSideScorerText;

    // エネミー関連テキスト
    [SerializeField]
    private Text enemyTotalPointText;
    [SerializeField]
    private Text enemySideScorerText;

    private void OnEnable()
    {
        // リザルト有効化
        resultCanvasGroup.interactable = true;
        resultCanvasGroup.blocksRaycasts = true;
        // 次への選択を非有効化
        selectCanvasGroup.interactable = false;
        selectCanvasGroup.blocksRaycasts = false;
        selectCanvasObject.SetActive(false);
        // 初期選択ボタン設定x
        EventSystem.current.SetSelectedGameObject(firstSelectResultButton);

        // 総得点数を代入
        playerTotalPointText.text = GameResultManager.instance.GetPlayerTotalScore().ToString();
        enemyTotalPointText.text = GameResultManager.instance.GetEnemyTotalScore().ToString();

        // プレイヤーの得点者リスト作成
        StringBuilder playerSideSB = new StringBuilder();
        foreach(Scorer scorer in GameResultManager.instance.GetPlayerSideScorerList())
        {
            playerSideSB.AppendLine($"{scorer.scorerName} {scorer.time}sec");
        }
        playerSideScorerText.text = playerSideSB.ToString();

        // 敵の得点者リスト作成
        StringBuilder enemySideSB = new StringBuilder();
        foreach (Scorer scorer in GameResultManager.instance.GetEnemySideScorerList())
        {
            enemySideSB.AppendLine($"{scorer.scorerName} {scorer.time}sec");
        }
        enemySideScorerText.text = enemySideSB.ToString();
    }

    private void OnDisable()
    {

    }

    // Update is called once per frame
    void Update()
    {
       
    }

    // 選択画面に移る際に実行
    public void OnSelect()
    {
        resultCanvasGroup.interactable = false;
        resultCanvasGroup.blocksRaycasts = false;

        selectCanvasGroup.interactable = true;
        selectCanvasGroup.blocksRaycasts = true;
        selectCanvasObject.SetActive(true);

        EventSystem.current.SetSelectedGameObject(firstSelectNextButton);
    }

    // ゲームのリトライ
    public void Retry()
    {
        // リザルトの初期化
        GameResultManager.instance.Remove();
        // 遷移履歴をクリア
        SceneTransitionManager.instance.ClearHistory();
        // 新しく遷移
        SceneTransitionManager.instance.LoadNewScene((int)SceneID.CharacterSelect);
    }

    // タイトルに戻る
    public void ChangeTitleScene()
    {
        // リザルトの初期化
        GameResultManager.instance.Remove();
        // 遷移履歴をクリア
        SceneTransitionManager.instance.ClearHistory();
        // 新しく遷移
        SceneTransitionManager.instance.LoadNewScene((int)SceneID.Title);
    }
}
