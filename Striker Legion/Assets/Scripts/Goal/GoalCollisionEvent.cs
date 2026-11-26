using UnityEngine;

public class GoalCollisionEvent : MonoBehaviour
{
	CharacterTeam team;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		team = gameObject.tag == "Player" ? CharacterTeam.Player : CharacterTeam.Enemy;
		//team = LayerMask.LayerToName(gameObject.layer) == "Player" ? CharacterTeam.Player : CharacterTeam.Enemy;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	// 衝突が始まった瞬間
	public void OnTriggerEnter(Collider collider)
	{
		Debug.Log("ゴール衝突報告 衝突タグ:"+ collider.gameObject.tag);
		// 相手のタグで判定する場合
		if (collider.gameObject.CompareTag("Ball"))
		{
            //MatchManager.Instance.GoalEvent(team);

			GameResultManager.instance.AddScore(team == CharacterTeam.Player ? GameResultManager.ScoreSide.Enemy : GameResultManager.ScoreSide.Player, name);
		}
	}
}
