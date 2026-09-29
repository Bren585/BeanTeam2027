using UnityEngine;
using static UnityEngine.Mathf;

[RequireComponent(typeof(CharacterController))]
public class CharacterBase : MonoBehaviour
{
    // コンポネント ******************************************************************************************

    protected CharacterController body;
    protected GameObject ball;
    protected GameObject tackleHitbox;

    // キャラ定義 *******************************************************************************************

    [Header("ベース設定")]
    [SerializeField] float minTurnSpeed = 90;
    [SerializeField] float maxTurnSpeed = 360;

    [SerializeField] float tackleHitboxDuration = 0.25f;
    [SerializeField] float tackleCooldown = 1.0f;

    // キャラパラメータ **************************************************************************************

    [Header("継承パラメータ")]
    /// <summary>
    ///                                             走るマックススピード
    /// </summary>
    [SerializeField] protected float maxRunSpeed;

    /// <summary>
    ///                                             ジャンプ力
    /// </summary>
    [SerializeField] protected float jumpStrength;

    /// <summary>
    ///                                             復活力
    /// </summary>
    [SerializeField] protected float recovery;

    /// <summary>
    ///                                             ボールの扱いやすさ
    ///                                             <br/>
    ///                                             値は0.0（攻撃されたら絶対落とす）から１．０（ボール落とさない）までとします。
    /// </summary>
    [SerializeField, Range(0f, 1f)] protected float evasion;

    /// <summary>
    ///                                             走るときの小回り力、横移動スピード、アクセル                      
    ///                                             <br/>            
    ///                                             値は０．０（最低に遅い）から１．０（最高に早い）までとします。
    /// </summary>
    [SerializeField, Range(0f, 1f)] protected float groundMobility;

    /// <summary>
    ///                                             ショート力
    /// </summary>
    [SerializeField] protected float power;

    [Header("鳥系")]
    /// <summary>
    ///                                             飛べるかどうか
    /// </summary>
    [SerializeField] protected bool canFly = false;

    /// <summary>
    ///                                             飛ぶマックススピード
    /// </summary>
    [SerializeField] protected float maxFlySpeed = 0;

    /// <summary>
    ///                                             飛ぶときの小回り力、横移動スピード、アクセル
    /// </summary>
    [SerializeField] protected float airMobility = 0;

    // キャラ状態 *******************************************************************************************

    [Header("状態")]

    /// <summary>
    ///                                             マックス体力
    /// </summary>
    [SerializeField] protected float maxStamina;

    /// <summary>
    ///                                             体力
    /// </summary>
    protected float stamina;

    Vector3 velocity;

    /// <summary>
    /// trueなら動いている向きに回る、falseなら向かない
    /// </summary>
    bool moving = false;
    bool strafing = false;
    bool flying = false;

    private float maxSpeed 
    { 
        get 
        {
            if      (flying)    return maxFlySpeed;
            else if (strafing)  return maxRunSpeed * (0.1f + 0.9f * groundMobility);
            else                return maxRunSpeed;
        } 
    }

    [SerializeField] public bool hasBall;
    //public bool hasBall { get; private set; } = false;

    float tackleTimer;
    

    // 関数 *******************************************************************************************

    protected virtual void Awake()
    {
        body = GetComponent<CharacterController>();
        ball = transform.Find("Ball").gameObject;
        tackleHitbox = transform.Find("TackleHitbox").gameObject;
        tackleHitbox.SetActive(false);
    }

    void Start()
    {
        stamina = maxStamina;
        tackleTimer = tackleCooldown;
    }

    void Update()
    {
        ball.SetActive(hasBall);

        if (tackleTimer < tackleCooldown)
        {
            tackleTimer += Time.deltaTime;
            if (tackleHitbox.activeSelf)
            {
                if (tackleTimer > tackleHitboxDuration) { tackleHitbox.SetActive(false); }
            }
        }

        if (flying) // 飛んでる
        {
        }
        else // 走ってる
        {
            // 速度チェック
            Vector3 groundVelocity = new Vector3(velocity.x, 0, velocity.z);
            float speed = groundVelocity.magnitude; 
            if (speed > maxSpeed)
            {
                float m = (maxSpeed / speed);
                velocity.Scale(new Vector3(m, 1, m));
                groundVelocity.Scale(new Vector3(m, 1, m));
                speed = maxSpeed;
            }

            if (strafing) // 横移動
            {
                body.Move(groundVelocity * Time.deltaTime);
            }
            else // 普通に走る
            {
                if (moving)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(groundVelocity.normalized);

                    body.transform.rotation = Quaternion.RotateTowards(
                        body.transform.rotation,
                        targetRotation,
                        (minTurnSpeed + Lerp(minTurnSpeed, maxTurnSpeed, groundMobility)) * Time.deltaTime
                    );

                }
                Vector3 forward = body.transform.forward.normalized;
                float dot = Vector3.Dot(groundVelocity.normalized, forward);
                body.Move(forward * (Clamp01(dot) * speed * Time.deltaTime));
            }
            // 重力
            // velocity += gravity
            // body.move(down)
        }
        if (!moving) { velocity = velocity * Clamp01(1.0f - Time.deltaTime / 0.25f); }
    }

    /// <summary>
    /// キャラに速度を加える
    /// </summary>
    /// <param name="input">速度</param>
    public void Move(Vector3 input)
    {
        velocity += input * maxSpeed * Time.deltaTime;
        moving = (input != Vector3.zero);
    }

    /// <summary>
    /// 移動先に向くべきか
    /// </summary>
    /// <param name="strafe">trueなら動いている向きに回る、falseなら向かない</param>
    public void SetStrafe(bool strafe)
    {
        if (flying) strafing = false;
        else        strafing = strafe;
    }

    public void Jump()
    {
        if (canFly) 
        { 
            // 飛び始めて
        }
        else 
        {
            return;
            //Vector3 jump;
            //jump.x = jump.z = 0;
            //jump.y = jumpStrength;
            //velocity += jump;
        }
    }

    public void Tackle()
    {
        if (hasBall) return;
        if (tackleTimer < tackleCooldown) return;
        tackleTimer = 0;
        tackleHitbox.SetActive(true);
    }

    /// <summary>
    /// 攻撃される
    /// </summary>
    /// <returns>ボール落としたらtrue</returns>
    public bool DropBall()
    {
        if (!hasBall) { return false; }
        bool dropped = Random.value > evasion;
        if (dropped) { hasBall = false; }
        return dropped;
    }

    public void GetBall()
    {
        hasBall = true;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "TackleHitbox")
        {
            if (DropBall())
            {
                other.GetComponentInParent<CharacterBase>().GetBall();
            }
        }
    }

    public void Shoot()
    {
        hasBall = false;
    }

    public virtual void Skill()
    {

    }
}
