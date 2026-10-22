using UnityEngine;
using UnityEngine.InputSystem;

public class AimMarkerController : BallHolder
{
    [SerializeField] Renderer aimMarkerRenderer;
    [SerializeField] Vector2 goalSpace = new Vector2(10f,5f); //ゴールの範囲だが、仮置きの長方形しか用意していないので、目測で値を用意している
    [SerializeField] GameObject playerHandler;
    PlayerController playerController;
    [SerializeField] InputActionReference inputRef;
    [SerializeField] Vector2 movingSensitivity = new Vector2(5.0f, 5.0f);
    Vector3 originPosition = Vector3.zero;
    [SerializeField] Unity.Behavior.EventChannel onGoalEvent;

    // override ************************
    public override Vector3 GetHoldPosition()
    {
        return transform.position;
    }

    private void Start()
    {
        playerController = playerHandler.GetComponent<PlayerController>();
        originPosition = transform.position;
        aimMarkerRenderer.enabled = false; // AimMarkerを非表示にする
    }

    void Update()
    {
        if (playerController == null || playerController.playerCharacter == null) { return; }
        if(playerController.playerCharacter.isShooting)
        {
            // AimMarker を表示する
            aimMarkerRenderer.enabled = true;

            // --AimMarkerの移動処理--
            Vector2 input = inputRef.action.ReadValue<Vector2>();
            // 入力に応じて AimMarker を移動させる
            transform.position += new Vector3(input.x * movingSensitivity.x * Time.deltaTime, input.y * movingSensitivity.y * Time.deltaTime, 0.0f);

            // AimMarker の位置をゴールの範囲内に制限する
            Vector3 nowPosition = transform.position;
            
            transform.position = new Vector3(Mathf.Clamp(nowPosition.x, originPosition.x - goalSpace.x, originPosition.x + goalSpace.x), Mathf.Clamp(nowPosition.y, originPosition.y - goalSpace.y, originPosition.y + goalSpace.y), nowPosition.z);
        }
        else
        {
            // AimMarker を非表示にする
            aimMarkerRenderer.enabled = false;
            // AimMarker を元の位置に戻す
            transform.position = originPosition;
        }
    }

    protected override void OnGetBall()
    {
        onGoalEvent.SendEventMessage();
        MatchManager.Instance.GoalEvent(team);
    }
}
