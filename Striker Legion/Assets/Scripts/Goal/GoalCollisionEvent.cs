using UnityEngine;

public class GoalCollisionEvent : MonoBehaviour
{
	CharacterTeam team;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		team = LayerMask.LayerToName(gameObject.layer) == "Player" ? CharacterTeam.Player : CharacterTeam.Enemy;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	// 衝突が始まった瞬間
	private void OnCollisionEnter(Collision collision)
	{
		Debug.Log("ゴール衝突報告 衝突タグ:"+collision.gameObject.tag);
		// 相手のタグで判定する場合
		if (collision.gameObject.CompareTag("Ball"))
		{
            MatchManager.Instance.GoalEvent(team);
		}
	}
}
