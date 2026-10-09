using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DebugText : MonoBehaviour
{
	private Vector3 worldOffset = new Vector3(0, 2.0f, 0);
	private Text text;
	private Camera mainCamera;

	// 常に表示するメッセージ(SetTextで消えない)
	private string DefaultMessage = "";

	private void Start()
	{
		mainCamera = Camera.main;

		// デバッグテキストの生成
		DebugTextManager.Instance.AddDebugText(out text);
	}

	private void LateUpdate()
	{
		if(text == null)
		{
			Debug.Log("テキスト生成失敗");
				return;
		}
		
		if (mainCamera == null)
		{
			SetText("mainCameraがnull");
			return;
		}
		// このコンポーネントを持っているオブジェクトの座標からオフセット分調整
		// その後、スクリーン座標にする
		Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position + worldOffset);
		
		if (screenPos.z < 0)
		{
			text.gameObject.SetActive(false);
			return;
		}

		// テキストの座標調整
		text.rectTransform.position = screenPos;
	}

	// オブジェクト破棄時にUIテキストも一緒に消去
	private void OnDestroy()
	{
		if (text != null)
			Destroy(text.gameObject);
	}

	public void SetText(string textStr)
	{
		//text.text = textStr;

		text.text = DefaultMessage + '\n' +textStr;
	}
	public void SetDefaultText(string defaultStr)
	{
		DefaultMessage = defaultStr;
	}

}
