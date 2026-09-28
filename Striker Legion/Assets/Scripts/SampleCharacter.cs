using UnityEngine;

public class SampleCharacter : MonoBehaviour
{
    // ボールのオブジェクト
    [SerializeField] private GameObject ballObject;

	// ボール保持者かどうかのフラグ
	[SerializeField] public bool isHoldingBall = false;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        if(ballObject == null)
        {
            Debug.Log("ボールのオブジェクトの登録忘れ");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isHoldingBall)
        {
            ballObject.transform.SetPositionAndRotation(transform.position + Vector3.up * 0.5f, transform.rotation);
			//Debug.Log("ボールの位置更新");
        }
    }
}
