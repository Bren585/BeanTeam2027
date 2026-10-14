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

    [Tooltip("PlayerHandler"), SerializeField] private GameObject playerHandler;
    PlayerController   playerController;
    Transform           cameraTarget;
    [Tooltip("ゴールとその範囲"), SerializeField] private GameObject goal;

    private float targetYaw;    // ヨー角 
    private float targetPitch;  // ピッチ角 
    private bool wasShooting = false; // 撃っていたかどうかのフラグ

    void Start()
    {
        playerController = playerHandler.GetComponent<PlayerController>();
        cameraTarget = playerHandler.transform.Find("CameraTarget").transform;
    }

    // Updateの後に呼ばれる更新処理 
    void LateUpdate()
    {
        if (playerController.playerCharacter.isShooting)
            ShootModeUpdate();
        else
            NormalModeUpdate();
    }

    void NormalModeUpdate()
    {
        if (wasShooting)
        {
            //Positionを戻し、CameraTargetにLookAtする
            cameraTarget.position = playerHandler.transform.Find("CameraPosition").position;

            cinemachineCamera.LookAt = cameraTarget;
            wasShooting = false;
        }

        // Inspector で設定した Action の値を取得 
        Vector2 input = cameraRef.action.ReadValue<Vector2>();

        // 回転する 
        // 縦軸で回転 
        targetYaw += input.x * lookSensitivity.x * Time.deltaTime;
        // 横軸で回転
        targetPitch += -input.y * lookSensitivity.y * Time.deltaTime;
        // pitch と yaw を使って回転する
        cameraTarget.transform.rotation = Quaternion.Euler(targetPitch, targetYaw, 0.0f);
    }

    void ShootModeUpdate()
    {
        if(!wasShooting)
        {
            //カメラの位置をカメラターゲットとは別の位置（ShootCameraPosition）にする
            cameraTarget.position = playerHandler.transform.Find("ShootCameraPosition").position;

            // ゴールの子オブジェクトにLookatする
            cinemachineCamera.LookAt = goal.transform.Find("ShootCamOrigin");
            wasShooting = true;
        }
    }
}