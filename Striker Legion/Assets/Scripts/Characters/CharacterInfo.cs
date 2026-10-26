using UnityEngine;

[CreateAssetMenu(fileName = "CharacterInfo", menuName = "Scriptable Objects/CharacterInfo")]
public class CharacterInfo : ScriptableObject
{
    [SerializeField, Range(0f, 1f)] public float speed = 0.5f;

    /// <summary>
    ///                                             復活力
    ///                                             <br/>
    ///                                             値は0.0（弱い、早く治らない）から１．０（強い、早く治る）までとします。
    /// </summary>
    [SerializeField, Range(0f, 1f)] public float constition = 0.5f;

    /// <summary>
    ///                                             ボールの扱いやすさ
    ///                                             <br/>
    ///                                             値は0.0（攻撃されたら絶対落とすなど）から１．０（ボール落とさないなど）までとします。
    /// </summary>
    [SerializeField, Range(0f, 1f)] public float ballHandling = 0.5f;

    /// <summary>
    ///                                             走るときの小回り力、横移動スピード、アクセル                      
    ///                                             <br/>            
    ///                                             値は０．０（最低に遅い）から１．０（最高に早い）までとします。
    /// </summary>
    [SerializeField, Range(0f, 1f)] public float groundMobility = 0.5f;

    /// <summary>
    ///                                             ショート力
    /// </summary>
    [SerializeField, Range(0f, 1f)] public float power = 0.5f;

    [Header("魑･邉ｻ")]
    /// <summary>
    ///                                             飛べるかどうか
    /// </summary>
    [SerializeField] public bool canFly = false;

    /// <summary>
    ///                                             飛ぶマックススピード
    /// </summary>
    [SerializeField] public float maxFlySpeed = 0;

    /// <summary>
    ///                                             飛ぶときの小回り力、横移動スピード、アクセル
    /// </summary>
    [SerializeField, Range(0f, 1f)] public float airMobility = 0.1f;
}
