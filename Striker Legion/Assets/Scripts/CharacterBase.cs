using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using Unity.Collections;
using Unity.Collections.Tests.CoreCLR.TestJobs;
using Unity.VisualScripting;
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

    [SerializeField] float maxDownTime = 2.0f;

    [SerializeField] float gravity = 5.0f;

    /// <summary>
    ///                                             ジャンプ力
    /// </summary>
    [SerializeField] protected float jumpStrength = 2.0f;


    // キャラパラメータ **************************************************************************************

    [Header("継承パラメータ")]
    /// <summary>
    ///                                             走るマックススピード
    /// </summary>
    [SerializeField] protected float maxRunSpeed;

    /// <summary>
    ///                                             復活力
    ///                                             <br/>
    ///                                             値は0.0（弱い、早く治らない）から１．０（強い、早く治る）までとします。
    /// </summary>
    [SerializeField, Range(0f, 1f)] protected float constition;

    /// <summary>
    ///                                             ボールの扱いやすさ
    ///                                             <br/>
    ///                                             値は0.0（攻撃されたら絶対落とすなど）から１．０（ボール落とさないなど）までとします。
    /// </summary>
    [SerializeField, Range(0f, 1f)] protected float ballHandling;

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
    [SerializeField, Range(0f, 1f)] protected float airMobility = 0.1f;

    // キャラ状態 *******************************************************************************************

    [field: Header("状態")]

    [field: SerializeField] public int teamNo { get; private set; }

    [field: SerializeField] public bool hasBall { get; private set; } = false;

    public Vector3 velocity { get; private set; }

    /// <summary>
    /// trueなら動いている向きに回る、falseなら向かない
    /// </summary>
    bool strafing = false;
    bool moving = false;
    bool flying = false;
    bool moveLocked = false;

    private float maxSpeed 
    { 
        get 
        {
            if      (flying)    return maxFlySpeed;
            else if (strafing)  return maxRunSpeed * (0.1f + 0.9f * groundMobility);
            else                return maxRunSpeed;
        } 
    }

    float tackleTimer;
    float downTimer;

    public bool isDown { get { return downTimer > 0; } }

    CharacterBase passTarget = null;

    // 関数 *******************************************************************************************

    protected virtual void Awake()
    {
        body = GetComponent<CharacterController>();
        ball = transform.Find("Ball").gameObject;
        ball.SetActive(false);
        tackleHitbox = transform.Find("TackleHitbox").gameObject;
        tackleHitbox.SetActive(false);
    }

    void Start()
    {
        tackleTimer = tackleCooldown;
    }

    void Update()
    {
        if (hasBall)
        {
            CharacterBase newPassTarget = PassTarget();
            if (passTarget != newPassTarget)
            {
                if (passTarget) passTarget.MarkAsPassTarget(false);
                if (newPassTarget) newPassTarget.MarkAsPassTarget(true);
                passTarget = newPassTarget;
            }
        }

        if (tackleTimer < tackleCooldown)
        {
            tackleTimer += Time.deltaTime;
            if (tackleHitbox.activeSelf)
            {
                if (tackleTimer > tackleHitboxDuration) { tackleHitbox.SetActive(false); }
            }
        }

        if (isDown)
        {
            downTimer -= Time.deltaTime;
            velocity = Vector3.zero;
            return;
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
            Vector3 groundMovement = Vector3.zero;
            if (strafing) // 横移動
            {
                groundMovement = (groundVelocity * Time.deltaTime);
            }
            else // 普通に走る
            {
                if (moving && !moveLocked && groundVelocity != Vector3.zero)
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
                groundMovement = (forward * (Clamp01(dot) * speed * Time.deltaTime));
            }

            // 重力
            Vector3 airMovement = Vector3.zero;
            Vector3 airVelocity = Vector3.zero;
            airVelocity.y = velocity.y;
            if (!body.isGrounded)
            {
                Vector3 gravityVector = new(0, -gravity * Time.deltaTime, 0);
                velocity += gravityVector;
                airVelocity += gravityVector;
                airMovement = (airVelocity * Time.deltaTime);
            } 
            else
            {
                if (velocity.y > 0)
                {
                    airMovement = (airVelocity * Time.deltaTime);
                }
            }

            body.Move(groundMovement + airMovement);
        }
        if (!moving && !moveLocked) {
            float drag = Clamp01(1.0f - Time.deltaTime / 0.25f);
            velocity = Vector3.Scale(velocity, new Vector3(drag, 1, drag)); 
        }
    }

    /// <summary>
    /// キャラに速度を加える
    /// </summary>
    /// <param name="input">速度</param>
    public void Move(Vector3 input)
    {
        if (moveLocked) { return; }

        moving = (input != Vector3.zero) || !body.isGrounded;
        if (!moving) { return; }

        float mobility = body.isGrounded ? groundMobility : airMobility;

        float accelerationModifier = Mathf.Pow(2, (mobility - 1));

        Vector3 inputVelocity = input * maxSpeed * Time.deltaTime * accelerationModifier;
        Vector3 groundVelocity = new Vector3(velocity.x, 0, velocity.z);
        float dot = Vector3.Dot(inputVelocity.normalized, groundVelocity.normalized);

        if (dot < 0) // 移動先が動いている向きと違う
        {
            // 向きが違う分、早く向けるため、早くなる。
            inputVelocity += groundVelocity * dot * mobility;
        }

        velocity += inputVelocity;
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
        if (!body.isGrounded) { return; }
        if (canFly) 
        { 
            // 飛び始めて
        }
        else 
        {
            Vector3 jump;
            jump.x = jump.z = 0;
            jump.y = jumpStrength - velocity.y;
            velocity += jump;
        }
    }

    public void Tackle()
    {
        if (hasBall) return;
        if (tackleTimer < tackleCooldown) return;
        tackleTimer = 0;
        tackleHitbox.SetActive(true);
    }

    bool BallHandlingFail()
    {
        return Random.value > ballHandling;
    }

    bool BallHandlingSucess()
    {
        return Random.value < ballHandling;
    }

    public void GetBall()
    {
        hasBall = true;
        unlockMove();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "TackleHitbox")
        {
            CharacterBase opponent = other.GetComponentInParent<CharacterBase>();
            if (BallHandlingFail())
            {
                //if (opponent.teamNo == teamNo) { return; } // テストしやすいためオッフ

                GameBall gameBall = FindAnyObjectByType<GameBall>();

                gameBall.Kick(new GameBall.flight(this, opponent, 0, 20));
                gameBall.SetOwner(opponent);
                opponent.lockMove();
                hasBall = false;
                ClearPassTarget();

                // ダウン
                downTimer = maxDownTime * (1 - constition);
            }
        }
        else if (other.gameObject.name == "GoalZone")
        {
            // Enter Shooting Mode
            //
            // Shooting Mode is a separate script
            // Camera moves close into the shooter, and time slows down
            // Shooter moves a cursor (speed based on speed) to aim
            // If the defender is a player, they also have a cursor to aim their defense
            // Shooter shoots
            // Defender uses speed stat to attempt to close the distance (time limit emposed by shooter power)
            // If defender reaches, they use they're constitution (and maybe ballhandle) to try to block
            // Player defender gets a bonus in speed and block if they aimed correctly 
            //
            // 日本語 (機械翻訳)
            // シュートモードは別スクリプトで処理
            // カメラがシューターに接近し、時間の流れが遅くなる
            // シューターはカーソルを動かして狙いを定める（カーソルの移動速度はステータスの「スピード」に依存）
            // ディフェンダーがプレイヤーの場合、同様にカーソルを使って守備位置を狙う
            // シューターがシュートを放つ
            // ディフェンダーは「スピード」のステータスを活かして距離を詰めようとする（制限時間はシューターのパワーに依存）
            // ディフェンダーが到達した場合、「スタミナ（Constitution）」（および場合によっては「ボールハンドリング」）を使ってブロックを試みる
            // プレイヤーがディフェンダーの場合、狙いが正確であればスピードとブロックにボーナスが加算される
            // 
        }
    }

    public CharacterBase PassTarget(out List<RaycastHit> outBlockers)
    {
        CharacterBase bestCandidate = null;
        float bestDistance = float.MaxValue;
        List<RaycastHit> blockers = null;

        CharacterBase[] characters = FindObjectsByType<CharacterBase>();

        foreach (CharacterBase character in characters)
        {
            if (character == this) continue;
            if (character.teamNo != teamNo) continue;

            Vector3 toCharacter = character.transform.position - transform.position;
            float dot = Vector3.Dot(transform.forward, toCharacter.normalized);

            if (dot <= 0.7071) continue; // cosf(PI/4)

            float distance = toCharacter.magnitude;

            if (distance > bestDistance && blockers == null) { continue; }

            List<RaycastHit> hits = new(Physics.SphereCastAll(new Ray(transform.position, toCharacter), 0.5f, distance));
            hits.RemoveAll(hit =>
            {
                CharacterBase hitCharacter = hit.transform.GetComponent<CharacterBase>();
                return hitCharacter == null || hitCharacter.teamNo == teamNo;
            });


            if (blockers != null) { if (hits.Count > blockers.Count) { continue; } }

            if (distance < bestDistance)
            {
                bestCandidate = character;
                bestDistance = distance;
                blockers = hits;
                continue;
            }
        }

        outBlockers = blockers;
        return bestCandidate;
    }

    public CharacterBase PassTarget() { return PassTarget(out _); }

    //public CharacterBase GetPassTarget() { return passTarget; }

    public void MarkAsPassTarget(bool isTarget)
    {
        ball.SetActive(isTarget);
    }

    void ClearPassTarget()
    {
        if (passTarget == null) return;
        passTarget.MarkAsPassTarget(false);
        passTarget = null;
    }

    public void Shoot()
    {
        if (!hasBall) return;

        // Check Pass Or Goal

        List<RaycastHit> blockers = null;
        CharacterBase target = PassTarget(out blockers);

        if (target == null) return;

        // See if blocked or not
        if (blockers != null)
        {
            blockers.Sort((a, b) => a.distance.CompareTo(b.distance));
            foreach (RaycastHit blocker in blockers) { 
                CharacterBase character = blocker.collider.GetComponent<CharacterBase>();
                if (character == null) continue;
                if (character.BallHandlingSucess())
                {
                    target = character;
                    break;
                }
            }
        }

        GameBall gameBall = FindAnyObjectByType<GameBall>();
        gameBall.Kick(new GameBall.flight(this, target, 1.5f, 20));
        gameBall.SetOwner(target);
        target.lockMove();
        ClearPassTarget();
        hasBall = false;
    }

    public virtual void Skill()
    {

    }

    public void lockMove() { moveLocked = true; }
    public void unlockMove() { moveLocked = false; }
}
