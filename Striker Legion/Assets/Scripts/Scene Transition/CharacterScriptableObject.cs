using UnityEngine;

[CreateAssetMenu(fileName = "CharacterScriptableObject", menuName = "Scriptable Objects/CharacterScriptableObject")]
public class CharacterScriptableObject : ScriptableObject
{
    public int runningAbility; // 走力
    public int durability; // 耐久力
    public int keepAbility; // キープ力
    public int explosiveness; // 瞬発力
    public int muscleStrength; // 筋力

    public string characterName; // キャラクター名
    public Sprite icon; // キャラクター選択用アイコン
    public Sprite profileImage; // プロフィール画像
    public string description; // キャラクター説明

    public AudioClip[] selectVoices; // 選択時に再生されるボイスデータ

    public Character_Position position = Character_Position.None; // ポジション情報
}
