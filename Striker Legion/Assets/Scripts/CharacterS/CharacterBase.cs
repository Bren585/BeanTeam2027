using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using Unity.Collections;
using Unity.Collections.Tests.CoreCLR.TestJobs;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Mathf;

[RequireComponent(typeof(CharacterController))]
public class CharacterBase : BallHolder
{
    // コンポネント ******************************************************************************************

    private CharacterController body;
    private GameObject ball;
    private GameObject tackleHitbox;
    private Animator animator;

    // 繧ｭ繝｣繝ｩ螳夂ｾｩ *******************************************************************************************

    [Header("ベース設定")]
    [SerializeField] private CharacterBaseInfo baseInfo;

    // 繧ｭ繝｣繝ｩ繝代Λ繝｡繝ｼ繧ｿ **************************************************************************************

    [Header("継承パラメータ")]

    [SerializeField] private CharacterInfo info;

    // キャラ状態 *******************************************************************************************

    // スキル
    [SerializeField] private SkillEffectBase skillEffect;

    [field: Header("状態")]

    private Vector3 _velocity;
    public Vector3 velocity => _velocity;

    // オーバーライド ***************************************************************************************
    public override Vector3 GetHoldPosition() { return transform.Find("Feet").position; }
    public override Vector3 GetVelocity() { return velocity; }

    /// <summary>
    /// trueなら動いている向きに回る、falseなら向かない
    /// </summary>
    bool strafing = false;
    bool moving = false;
    bool flying = false;
    bool moveLocked = false;
    public bool isShooting { get; private set; } = false;

    private float maxSpeed 
    { 
        get 
        {
            if      (flying)    return info.maxFlySpeed;
            else if (strafing)  return info.speed * baseInfo.maxRunSpeed * (0.1f + 0.9f * info.groundMobility);
            else                return info.speed * baseInfo.maxRunSpeed;
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

        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        tackleTimer = baseInfo.tackleCooldown;
    }

    void Update()
    {
        animator.Update(Time.deltaTime);

        UpdatePassTarget();

        UpdateTackle();

        if (isDown)
        {
            _velocity.x = 0;
            _velocity.z = 0;
            downTimer -= Time.deltaTime;
            if (downTimer <= 0) { animator.SetTrigger("Recover"); }
            return;
        }

        UpdateMovement();
    }


    void UpdatePassTarget()
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
    }

    void UpdateTackle()
    {
        if (tackleTimer < baseInfo.tackleCooldown)
        {
            tackleTimer += Time.deltaTime;
            if (tackleHitbox.activeSelf)
            {
                if (tackleTimer > baseInfo.tackleHitboxDuration) { tackleHitbox.SetActive(false); }
            }
        }
    }

    void UpdateMovement()
    {
        if (flying) // 飛んでる
        {
        }
        else // 走ってる
        {
            // 速度チェック
            Vector3 groundVelocity = velocity;
            groundVelocity.y = 0;

            groundVelocity = Vector3.ClampMagnitude(groundVelocity, maxSpeed);

            Vector3 movement = Vector3.zero;

            if (strafing) // 横移動
            {
                movement = (groundVelocity * Time.deltaTime);
                animator.SetFloat("forward", 0);
            }
            else //　普通に走る
            {
                UpdateRotation(groundVelocity);

                float speed = groundVelocity.magnitude;

                if (speed > 0.001f)
                {
                    float forwardDegree = Vector3.Dot(groundVelocity / speed, body.transform.forward);
                    movement = (body.transform.forward * (Clamp01(forwardDegree) * speed * Time.deltaTime));

                    float angle = Vector3.SignedAngle(
                        body.transform.forward,
                        groundVelocity.normalized,
                        Vector3.up
                    );

                    animator.SetFloat("forward", angle / 180.0f);
                } 
                else { animator.SetFloat("forward", 0); }
            }

            movement += UpdateGravity();

            body.Move(movement);
        }

        if (!moving && !moveLocked)
        {
            float drag = Clamp01(1.0f - Time.deltaTime / 0.25f);
            _velocity.x *= drag;
            _velocity.z *= drag;
        }

        if (body.isGrounded && velocity.y < 0) { _velocity.y = 0; }

        animator.SetFloat("groundSpeed", new Vector2(velocity.x, velocity.z).magnitude);
        animator.SetFloat("yVelocity", velocity.y);
        animator.SetBool("grounded", body.isGrounded);
    }

    void UpdateRotation(Vector3 groundVelocity)
    {
        if (moving && (!moveLocked) && (groundVelocity.sqrMagnitude > 0.01f))
        {
            Quaternion targetRotation = Quaternion.LookRotation(groundVelocity);

            float turnSpeed = (baseInfo.minTurnSpeed + Lerp(baseInfo.minTurnSpeed, baseInfo.maxTurnSpeed, info.groundMobility)) * Time.deltaTime;

            body.transform.rotation = Quaternion.RotateTowards(
                body.transform.rotation,
                targetRotation,
                turnSpeed
            );

        }
    }

    Vector3 UpdateGravity()
    {
        _velocity.y += -baseInfo.gravity * Time.deltaTime;
        return Vector3.up * velocity.y * Time.deltaTime;
    }

    /// <summary>
    /// キャラに速度を加える
    /// </summary>
    /// <param name="input">速度</param>
    public void Move(Vector3 input)
    {
        if (moveLocked) { return; }

        moving = (input.sqrMagnitude > 0.001f) || (!body.isGrounded);
        if (input.sqrMagnitude < 0.001f) { return; }

        input = input.normalized;

        float mobility = body.isGrounded ? info.groundMobility : info.airMobility;

        float accelerationModifier = Mathf.Pow(2, (mobility - 1));

        float acceleration = maxSpeed * Time.deltaTime * accelerationModifier;
        Vector3 groundVelocity = velocity;
        groundVelocity.y = 0;

        if (groundVelocity.sqrMagnitude > 0.001f)
        {
            float dot = Vector3.Dot(input, groundVelocity.normalized);

            if (dot < 0)  // 移動先が動いている向きと違う
            {
                // 向きが違う分、早く向けるため、早くなる。
                acceleration *= 1 - dot * mobility;
            }
        }

        _velocity += input * acceleration;
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
        animator.SetTrigger("Jump");
        if (info.canFly) 
        {
            // 飛び始めて
        }
        else 
        {
            _velocity.y = baseInfo.jumpStrength;
        }
    }

    public void Tackle()
    {
        if (hasBall) return;
        if (tackleTimer < baseInfo.tackleCooldown) return;
        tackleTimer = 0;
        tackleHitbox.SetActive(true);
        animator.SetTrigger("Tackle");
    }

    bool BallHandlingFail()
    {
        return Random.value > info.ballHandling;
    }

    bool BallHandlingSuccess()
    {
        return Random.value < info.ballHandling;
    }

    protected override void OnGetBall()
    {
        unlockMove();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "TackleHitbox")
        {
            if (!hasBall) { return; }
            CharacterBase opponent = other.GetComponentInParent<CharacterBase>();
            if (BallHandlingFail())
            {
                //if (opponent.teamNo == teamNo) { return; } // テストしやすいためオッフ

                GameBall gameBall = FindAnyObjectByType<GameBall>();

                gameBall.Kick(new GameBall.flight(this, opponent, 0, 20));
                gameBall.SetOwner(opponent);
                opponent.lockMove();
                LoseBall();
                ClearPassTarget();
                SkillBar.instance.GainForTackle(opponent.team);

                // ダウン
                downTimer = baseInfo.maxDownTime * (1 - info.constition);
                animator.SetTrigger("Down");
            }
        }
        //HACK: if文が長すぎる
        else if (other.gameObject.tag == "Goal" && other.transform.Find("ShootCamOrigin").Find("AimMarker").GetComponent<AimMarkerController>().team != this.team && hasBall == true)
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

            //Debuging
            lockMove();
            isShooting = true;
            _velocity = Vector3.zero;
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
            if (character.team != team) continue;

            Vector3 toCharacter = character.transform.position - transform.position;
            float dot = Vector3.Dot(transform.forward, toCharacter.normalized);

            if (dot <= 0.7071) continue; // cosf(PI/4)

            float distance = toCharacter.magnitude;

            if (distance > bestDistance && blockers == null) { continue; }

            List<RaycastHit> hits = new(Physics.SphereCastAll(new Ray(transform.position, toCharacter), 0.5f, distance));

            // ヒット対象からキャラクター以外とチームメンバーを除外
            hits.RemoveAll(hit =>
            {
                CharacterBase hitCharacter = hit.transform.GetComponent<CharacterBase>();
                return hitCharacter == null || hitCharacter.team == team;
            });

            // ヒット数が前回までのヒット数より多かったら飛ばす
            if (blockers != null) { 
                if (hits.Count > blockers.Count) 
                    continue; 
            }

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

	public bool EnablePass()
	{
		CharacterBase target = PassTarget();

        // ボールを保持していて、パスターゲットがいるか
        if (!hasBall || target == null)
            return false;

        // パスターゲットがパスできる範囲内にいるか
        if ((target.transform.position - transform.position).magnitude > baseInfo.maxPassDistance)
            return false;

        return true;
	}

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


    public void KickBall()
    {
        if (!hasBall) return;
        GameBall gameBall = FindAnyObjectByType<GameBall>();

        // Check Pass Or Goal
        if (isShooting)
        {
            //--Shoot to Goal--
            AimMarkerController[] targets = FindObjectsByType<AimMarkerController>();
            foreach (AimMarkerController goalCandidate in targets)
            {
                if (goalCandidate.team == team) continue;
                Debug.Log("Shooting to Goal");
                // Kick the ball to Goal
                gameBall.Kick(new GameBall.flight(this, goalCandidate, 0, 20));
                gameBall.SetOwner(goalCandidate);
                break;
            }
            ClearPassTarget();
            LoseBall();
            isShooting = false;
            unlockMove();
            return;
        }

        //--Pass to Target--
        List<RaycastHit> blockers = null;
        CharacterBase target = PassTarget(out blockers);

        if (target == null) return;

        if ((target.transform.position - transform.position).magnitude > baseInfo.maxPassDistance) { return; }

        // See if blocked or not
        if (blockers != null)
        {
            blockers.Sort((a, b) => a.distance.CompareTo(b.distance));
            foreach (RaycastHit blocker in blockers) { 
                CharacterBase character = blocker.collider.GetComponent<CharacterBase>();
                if (character == null) continue;
                if (character.BallHandlingSuccess())
                {
                    target = character;
                    break;
                }
            }
        }

        // Kick the ball to Target
        gameBall.Kick(new GameBall.flight(this, target, 1.5f, 20));
        gameBall.SetOwner(target);
        target.lockMove();

        ClearPassTarget();
        LoseBall();
        animator.SetTrigger("Shoot");
    }

    public bool CanSkill()
    {
        return SkillBar.instance.Ready(team);
    }

    public void Skill()
    {
        SkillBar.instance.PayForSkill(team);
        skillEffect.Activate(this);
        Debug.Log("Used Skill");
    }

    public void lockMove() { moveLocked = true; }
    public void unlockMove() { moveLocked = false; }
}

