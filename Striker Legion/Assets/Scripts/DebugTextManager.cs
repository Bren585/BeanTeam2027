using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class DebugTextManager : MonoBehaviour
{
	public static DebugTextManager Instance { get; private set; }

	private int TextCount = 0;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	private void Awake()
    {
		// シングルトンのチェック
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
		TextCount = 0;
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	public void AddDebugText(out Text textComponent)
	{
		// 空のGameObjectを生成
		GameObject newObj = new GameObject("DebugText" + TextCount);

		// Canvasの子要素に設定 (第2引数を false にしてローカル座標を維持)
		newObj.transform.SetParent(gameObject.transform, false);

		// textコンポーネントを追加
		textComponent = newObj.AddComponent<Text>();

		// テキストの設定
		textComponent.text = "DebugText" + TextCount;
		textComponent.fontSize = 12;
		textComponent.color = Color.white;
		textComponent.alignment = TextAnchor.MiddleCenter;
		// フォントの設定 (デフォルトフォントを取得)
		textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

		// アウトラインを追加、必要だったら変数で保持するように変更すること
		Outline outline = newObj.AddComponent<Outline>();
		outline.effectColor = Color.black; // 縁の色
		outline.effectDistance = new Vector2(1f, -1f); // 縁の太さ・方向

		TextCount++;
	}
}
