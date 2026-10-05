using Unity.AppUI.UI;
using UnityEngine;

public class CharacterSensor : MonoBehaviour
{
	//[SerializeField] private float detectionRadius = 15f;
	private Collider[] hitColliders = new Collider[10];

	// 毎フレームではなく、0.1〜0.2秒ごとに実行（軽量化）
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

	// ボールのオブジェクトまでの距離
	public float GetBallDistance()
	{
		if (GetComponent<SampleCharacter>().isHoldingBall)
			return 0.0f;
		else
		{
			GameObject ball = GameObject.FindGameObjectWithTag("Ball");
			if (ball != null)
				return Vector3.Distance(transform.position, ball.transform.position);
			else
				return float.MaxValue;
		}
	}

	/// <summary>
	/// ボールを持ってるキャラクターを探す。
	/// ゲーム性として、キャラの探知範囲に制限をかけている
	/// </summary>
	public bool GetCharacterHavingBall(string Layer, float radius, out SampleCharacter character)
	{
		// 対象のレイヤー
		LayerMask enemyLayer = LayerMask.GetMask(Layer);

		// 検出数を保存する変数
		int numFound = 0;

		// 検出処理
		if (Layer != null && Layer != "")
			numFound = Physics.OverlapSphereNonAlloc(transform.position, radius, hitColliders, enemyLayer);
		else
			numFound = Physics.OverlapSphereNonAlloc(transform.position, radius, hitColliders);

		// ボールを持っている対象
		SampleCharacter havingBall = null;

		// 検出されたキャラクターの回数回す
		for (int i = 0; i < numFound; i++)
		{
			SampleCharacter sampleChar = hitColliders[i].GetComponent<SampleCharacter>();
			// ボールを持っているキャラクターを探す
			if (sampleChar != null && sampleChar.isHoldingBall)
			{
				havingBall = sampleChar;
				break;
			}
		}
		character = havingBall;
		return havingBall != null;
	}

	/// <summary>
	/// 敵でボールを持っているキャラクターを探す。
	/// 距離による判定はなし
	/// </summary>
	/// <param name="tag">自分のタグ</param>
	/// <returns></returns>
	public SampleCharacter GetEnemyHavingBall(string tag)
	{
		// 相手のタグをサーチする(マジックナンバーなので、修正予定)
		string SearchTag = (tag == "PlayerCP") ? "EnemyCP" : "PlayerCP";

		// タグから対象を見つける
		GameObject[] objects = GameObject.FindGameObjectsWithTag(SearchTag);

		foreach(GameObject o in objects)
		{
			SampleCharacter characterComponent = o.GetComponent<SampleCharacter>();

			// ボール持ちを探す
			if (characterComponent.isHoldingBall)
				return characterComponent;

		}

		Debug.Log("Enemy Having Ball is Null");
		return null;
	}
}

