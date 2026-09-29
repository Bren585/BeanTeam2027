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

	// ボールを持っているキャラクターを取得
	public bool GetCharacterHavingBall(string Layer, float radius, out SampleCharacter character)
	{
		LayerMask enemyLayer = LayerMask.GetMask(Layer);
		int numFound = 0;
		if (Layer != null && Layer != "")
			numFound = Physics.OverlapSphereNonAlloc(transform.position, radius, hitColliders, enemyLayer);
		else
			numFound = Physics.OverlapSphereNonAlloc(transform.position, radius, hitColliders);
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
}

