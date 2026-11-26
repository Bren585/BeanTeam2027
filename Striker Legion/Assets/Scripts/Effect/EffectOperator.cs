using System.Linq;
using UnityEngine;

public class EffectOperator : MonoBehaviour
{
    private ParticleSystem dustParticle;
    [SerializeField] private float dustThreshold = 15f;
    private bool isDustPlaying = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DustParticle_Init();
    }

    // Update is called once per frame
    void Update()
    {
        DustParticle_Update();
    }


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
        CharacterController characterController = dustParticle.GetComponentInParent<CharacterController>();
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
}
