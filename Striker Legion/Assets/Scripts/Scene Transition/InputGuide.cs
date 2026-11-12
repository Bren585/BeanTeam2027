using Microsoft.Extensions.Logging.Abstractions;
using UnityEngine;
using UnityEngine.UI;

public class InputGuide : MonoBehaviour
{
    [SerializeField]
    PauseMenuScript pause;

    // パッド操作時に表示するかどうか(offならマウス操作時に表示)
    [SerializeField]
    bool showPadMode;

    // ポーズ中に表示するかどうか
    [SerializeField]
    bool showPause;

    private Text guideText;

    private void OnEnable()
    {
        guideText = gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        bool currentUsedPad = InputModeManager.instance.GetUsedGamePadFlag();

        if (pause != null)
        {
            bool currentPause = pause.GetIsPause();

            if (currentPause && showPause)
            {
                if ((showPadMode && currentUsedPad) ||
                    (!showPadMode && !currentUsedPad))
                {
                    guideText.enabled = true;
                    return;
                }
            }
            else if ((!showPause && currentPause) || (showPause && !currentPause))
            {
                guideText.enabled = false;
                return;
            }
        }

        if ((showPadMode && currentUsedPad) || (!showPadMode && !currentUsedPad))
        {
            guideText.enabled = true;
        }
        else
        {
            guideText.enabled = false;
        }
    }
}
