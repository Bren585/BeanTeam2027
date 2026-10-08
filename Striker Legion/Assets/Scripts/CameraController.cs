using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;



public class CameraController : MonoBehaviour
{
    [Header("カメラ操作")]
    [Tooltip("シネマシン"), SerializeField]
    private CinemachineCamera   cinemachineCamera;
    [Tooltip("入力"), SerializeField]
    private InputActionReference cameraRef;
    [Tooltip("感度"), SerializeField]
    private Vector2 lookSensitivity = new Vector2(7.5f, 5.0f);

    [SerializeField]
    GameObject mainCamera; 

    [Tooltip("PlayerHandler"), SerializeField]
    private GameObject playerHandler;
    PlayerController       playerController;
    Transform               cameraTarget;
    [Tooltip("ゴールとその範囲"), SerializeField]
    private GameObject goal;

    private float targetYaw;    // ヨー角 
    private float targetPitch;  // ピッチ角 

    void Start()
    {
        playerController = playerHandler.GetComponent<PlayerController>();
    }

    // Updateの後に呼ばれる更新処理 
    void LateUpdate()
    {
        cameraTarget = playerHandler.transform.GetChild(0);

        if (playerController.playerCharacter.isShooting)
            ShootModeUpdate();
        else
            NormalModeUpdate();
    }

    void NormalModeUpdate()
    {
        // Inspector で設定した Action の値を取得 
        Vector2 input = cameraRef.action.ReadValue<Vector2>();

        // 回転する 
        // 縦軸で回転 
        targetYaw += input.x * lookSensitivity.x * Time.deltaTime;
        // 横軸で回転
        targetPitch += input.y * lookSensitivity.y * Time.deltaTime;
        // pitch と yaw を使って回転する
        cameraTarget.transform.rotation = Quaternion.Euler(targetPitch, targetYaw, 0.0f);
    }

    void ShootModeUpdate()
    {
        //カメラの位置をカメラターゲットとは別の位置（ShootCameraPosition）にする
        Vector3 ShootCameraPosition = playerHandler.transform.GetChild(1).position;
        cameraTarget.position = ShootCameraPosition;
        // ゴールの子オブジェクトにLookatする
        cinemachineCamera.LookAt = goal.transform.GetChild(0).transform;
    }
}