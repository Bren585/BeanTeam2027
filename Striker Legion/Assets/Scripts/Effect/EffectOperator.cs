using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

public class EffectOperator : MonoBehaviour
{
    CharacterController characterController = null;
    CharacterBase characterBase = null;

    //---DustParticle関連の変数--------------
    private ParticleSystem dustParticle;
    [SerializeField] private float dustThreshold = 15f;
    private bool isDustPlaying = false;
    //---------------------------------------

    //---Attack_Claw関連の変数--------------
    [SerializeField] private VisualEffect attackClawParticle;
    private bool isAttackClawPlaying = false;
    //---------------------------------------

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterController = GetComponentInParent<CharacterController>();
        characterBase = GetComponentInParent<CharacterBase>();
        DustParticle_Init();
        Attack_Claw_Init();
    }

    // Update is called once per frame
    void Update()
    {
        DustParticle_Update();
        Attack_Claw_Update();
    }



    //↓DustParticle////////////////////////////////////////////////////////////////////////////////////////
    // DustParticleの初期化
    void DustParticle_Init()
    {
        //砂埃パーティクルを取得し、初期化する
        dustParticle = transform.parent.Find("DustParticle").GetComponent<ParticleSystem>();
        dustParticle.Stop();
        isDustPlaying = false;
    }

    // DustParticleの更新
    void DustParticle_Update()
    {
        if (characterController == null) return; //nullチェック
        if (characterController.velocity.magnitude >= dustThreshold)
        {
            // 速度が閾値以上の場合、パーティクルを再生
            //Debug.Log(characterController.velocity.magnitude);
            if (!isDustPlaying)
            {
                dustParticle.Play();
                isDustPlaying = true;
            }
        }
        else
        {
            // 速度が閾値未満の場合、パーティクルを停止
            if (isDustPlaying)
            {
                dustParticle.Stop();
                isDustPlaying = false;
            }
        }
    }
    //↑DustParticle///////////////////////////////////////////////////////////////////////////////////

    //↓Attack_Claw//////////////////////////////////////////////////////////////////////////////////
    void Attack_Claw_Init()
    {
        //攻撃用のパーティクルを取得し、初期化する
        attackClawParticle = transform.parent.Find("Attack_Claw").GetComponent<VisualEffect>();
        attackClawParticle.Stop();
        isAttackClawPlaying = false;
    }

    void Attack_Claw_Update()
    {
        if (characterBase.isTacking)
        {
            if (!isAttackClawPlaying)
            {
                attackClawParticle.Play();
                isAttackClawPlaying = true;
            }
        }
        else
        {
            if (isAttackClawPlaying)
            {
                attackClawParticle.Stop();
                isAttackClawPlaying = false;
            }
        }
    }
}
