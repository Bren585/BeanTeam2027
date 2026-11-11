using System.Linq;
using UnityEngine;

public class EffectOperator : MonoBehaviour
{
    [SerializeField] private ParticleSystem ballEffect;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private GameObject player;
    private PlayerController playerController;

    private ParticleSystem[] dustParticle;
    [SerializeField] private float dustThreshold = 15f;
    private bool[] isDustPlaying;

    [SerializeField] private ParticleSystem ballSpeedLine;
    private bool isFlyng = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DustParticle_Init();
        ballEffect.Stop();
        playerController = player.GetComponent<PlayerController>();
        ballSpeedLine.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        DustParticle_Update();

        //----ボール飛行時のスピードラインエフェクト-----いらなくなったら消す
        GameBall gameBall = ballSpeedLine.GetComponentInParent<GameBall>();
        if (gameBall.inFlight && !isFlyng)
        {
            ballSpeedLine.Play();
            isFlyng = true;

        }
        else if (!gameBall.inFlight && isFlyng)
        {
            ballSpeedLine.Stop();
            isFlyng = false;
        }
        //--------

        if (playerController.playerCharacter.isShooting)
        {
            ballEffect.Play();
            trailRenderer.enabled = true;
        }
        else
        {
            ballEffect.Stop();
            trailRenderer.enabled = false;
        }
    }

    // DustParticleの初期化
    void DustParticle_Init()
    {
        //砂埃パーティクルを取得し、初期化する
        dustParticle = GameObject.FindObjectsByType<ParticleSystem>();
        foreach (var dust in dustParticle)
            dust.Stop(); 
        isDustPlaying = new bool[dustParticle.Length];
    }

    // DustParticleの更新
    void DustParticle_Update()
    {
        for (int i = 0; i < dustParticle.Length; i++)
        {
            var dust = dustParticle[i];
            CharacterController characterController = dust.GetComponentInParent<CharacterController>();
            if (characterController == null) continue; //nullチェック
            if (characterController.velocity.magnitude >= dustThreshold)
            {
                // 速度が閾値以上の場合、パーティクルを再生
                //Debug.Log(characterController.velocity.magnitude);
                if (!isDustPlaying[i])
                {
                    dust.Play();
                    isDustPlaying[i] = true;
                }
            }
            else
            {
                // 速度が閾値未満の場合、パーティクルを停止
                if (isDustPlaying[i])
                {
                    dust.Stop();
                    isDustPlaying[i] = false;
                }
            }
        }
    }
}
