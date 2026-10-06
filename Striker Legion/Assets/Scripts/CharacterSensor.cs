using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static UnityEditor.Experimental.GraphView.GraphView;

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

	public float GetDistanceToGoal(bool isMyGoal)
	{
		// ゴールのオブジェクトを取得する（2つあるはず）
		GameObject[] goalObjects = GameObject.FindGameObjectsWithTag("Goal");

		// 対象のレイヤー番号
		// 自分のゴールか相手のゴールかでレイヤーを決定する
		int Layer = isMyGoal ? gameObject.layer : (gameObject.layer == LayerMask.NameToLayer("Player") ? LayerMask.NameToLayer("Enemy") : LayerMask.NameToLayer("Player"));

		// ゴールの種類を決定する
		GameObject targetGoal = null;
		foreach(GameObject goalObject in goalObjects)
		{
			if(goalObject.layer == Layer)
			{
				targetGoal = goalObject;
				break;
			}
		}

		if (targetGoal == null)
		{
			Debug.Log("ゴールが見つかりませんでした");
			return float.MaxValue;
		}
		// 距離を計算
		return Vector3.Distance(transform.position, targetGoal.transform.position);
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
		// ヒットしたオブジェクトが敵かどうかを判定
		if (!hit.collider.CompareTag(enemyLayerStr == "Player" ? "PlayerCP" : "EnemyCP"))
		{
			return false;
		}
		return true;
	}

	// 味方の指定したロールのキャラクターを探す
	public SampleCharacter GetAllyByRole(CharacterRole role)
	{
		// 自身のタグを味方のタグとして使用する
		string allyTag = gameObject.tag;

		// 味方のオブジェクトを取得する
		GameObject[] allies = GameObject.FindGameObjectsWithTag(allyTag);
		foreach (GameObject ally in allies)
		{
			SampleCharacter sampleChar = ally.GetComponent<SampleCharacter>();

			// キャラクターのコンポーネントをヌルチェック
			if (sampleChar == null)
				continue;

			// ロールが指定したものかチェックする
			if (sampleChar.characterRole == role)
				return sampleChar;
			
		}
		return null;
	}

	// パスが成功するかどうかをレイキャストで判定する
	public bool IsPassSuccess(Transform TargetTransform)
	{
		// 引数の位置に向けてのレイを作成
		Ray ray = new Ray(transform.position, Vector3.Normalize(TargetTransform.position - transform.position));

		float rayDistance = Vector3.Distance(TargetTransform.position ,transform.position);

		// 敵のレイヤー
		string enemyLayerStr = (LayerMask.LayerToName(gameObject.layer) == "Player") ? "Enemy" : "Player";
		LayerMask enemyLayer = LayerMask.GetMask(enemyLayerStr);

		// レイがヒットしたか
		if (!Physics.Raycast(ray, out RaycastHit hit, rayDistance, enemyLayer))
		{
			Debug.Log("敵が見つかりませんでした");

			return false;
		}
		// ヒットしたオブジェクトが敵かどうかを判定
		if (!hit.collider.CompareTag(enemyLayerStr == "Player" ? "PlayerCP" : "EnemyCP"))
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

