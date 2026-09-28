using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterBase : MonoBehaviour
{
    // コンポネント ******************************************************************************************

    protected CharacterController body;

    // キャラパラメータ **************************************************************************************

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
    /// </summary>
    [SerializeField] protected float evasion;

    /// <summary>
    ///                                             走るときの小回り力、横移動スピード、アクセル
    /// </summary>
    [SerializeField] protected float groundMobility;

    /// <summary>
    ///                                             ショート力
    /// </summary>
    [SerializeField] protected float power;

    // 飛ぶ動物だけ

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
    bool strafing = false;

    // 関数 *******************************************************************************************


    protected virtual void Awake()
    {
        body = GetComponent<CharacterController>();
    }

    void Start()
    {
        stamina = maxStamina;
    }

    void Update()
    {
        body.Move(velocity * Time.deltaTime * maxRunSpeed);
        if (strafing)
        {
            velocity = velocity * Mathf.Clamp01(1.0f - Time.deltaTime / 0.25f);
        }
        else
        {

        }
    }

    /// <summary>
    /// キャラに速度を加える
    /// </summary>
    /// <param name="input">速度</param>
    public void Move(Vector3 input)
    {
        velocity += input * Time.deltaTime;    
    }

    /// <summary>
    /// 移動先に向くべきか
    /// </summary>
    /// <param name="strafe">trueなら動いている向きに回る、falseなら向かない</param>
    public void SetStrafe(bool strafe)
    {
        strafing = strafe;
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

    }

    public void Shoot()
    {

    }

    public virtual void Skill()
    {

    }
}
