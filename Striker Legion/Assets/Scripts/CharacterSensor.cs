using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static UnityEditor.Experimental.GraphView.GraphView;

public class CharacterSensor : MonoBehaviour
{
	//[SerializeField] private float detectionRadius = 15f;
	private Collider[] hitColliders = new Collider[10];

	// 一番近いキャラクターを調べる
	public bool GetClosestCharacter(string Layer, float radius,out SampleCharacter character)
	{
		LayerMask enemyLayer = LayerMask.GetMask(Layer);
		int numFound = 0;

		// レイヤーが指定されている場合はそのレイヤーのみで検出する
		if(Layer != null && Layer != "")	
			numFound = Physics.OverlapSphereNonAlloc(transform.position, radius, hitColliders, enemyLayer);
		else
			numFound = Physics.OverlapSphereNonAlloc(transform.position, radius, hitColliders);

		SampleCharacter closest = null;
		float minDist = float.MaxValue;

		for (int i = 0; i < numFound; i++)
		{
			if(hitColliders[i].GetComponent<SampleCharacter>() == null)
				continue;
			if (hitColliders[i].GetComponent<SampleCharacter>() == this.GetComponent<SampleCharacter>())
				continue;

			float dist = Vector3.Distance(transform.position, hitColliders[i].transform.position);
			// 最も近いキャラクターのみ対象
			if (dist < minDist)
			{
				minDist = dist;
				closest = hitColliders[i].GetComponent<SampleCharacter>();
			}
		}
		character = closest;
		return closest != null;
	}

	
	/// <summary>
	/// ボールを持ってるキャラクターを探す。
	/// ゲーム性として、キャラの探知範囲に制限をかけている
	/// </summary>
	public bool GetCharacterHavingBall(string Layer, float radius, out SampleCharacter character)
	{
		// 呼び出し元へ帰す結果を初期化
		character = null;

		// 対象のレイヤー
		LayerMask enemyLayer = LayerMask.GetMask(Layer);

		// ボールを持っているキャラクター
		SampleCharacter BallerCharacter  = (MatchManager.Instance.CurrentBallHolder != null) ? MatchManager.Instance.CurrentBallHolder : null;

		// 距離判定
		if (Vector3.Distance(BallerCharacter.transform.position, gameObject.transform.position) > radius)
			return false;

		character = BallerCharacter;

		return BallerCharacter != null;
	}

	public bool IsEnemyInRange(string targetLayer, float radius)
	{
		// プレイヤーの正面方向に Ray を発射
		Ray ray = new Ray(transform.position, transform.forward);

		string enemyLayerStr = (targetLayer == "Player") ? "Enemy" : "Player";
		LayerMask enemyLayer = LayerMask.GetMask(enemyLayerStr);

		// レイがヒットしたか
		if (!Physics.Raycast(ray, out RaycastHit hit, radius, enemyLayer))
		{
			Debug.Log("敵が見つかりませんでした");

			return false;
		}
		// ヒットしたオブジェクトがキャラクターかどうかを判定
		if (!hit.collider.CompareTag("Character"))
		{
			return false;
		}
		return true;
	}


	public bool IsNearbyOpponent(float radius)
	{
		int OpponentLayer = LayerMask.GetMask(LayerMask.LayerToName(gameObject.layer) == "Player" ? "Enemy" : "Player"); 

		//LayerMask enemyLayer = LayerMask.GetMask(gameObject.layer);
		int numFound = 0;

		// 近くにいるか判定する
		numFound = Physics.OverlapSphereNonAlloc(transform.position, radius, hitColliders, OpponentLayer);
		
		// 検出された数が1以上ならtrue
		return (numFound > 0) ? true : false;
	}
}

