using UnityEngine;

public class DebugMenu : MonoBehaviour
{
	//private Rect windowRect = new Rect(0, 0, 220, 200);
	private Rect windowRect = new Rect(0, 0, 250, 200);
	private Rect ScrollRect = new Rect(10, 0, 250, 250);

	private Vector2 scrollPosition = Vector2.zero;
	void OnGUI()
	{
		if (!Application.isPlaying) return;
		//scrollPosition = GUI.BeginScrollView(ScrollRect, scrollPosition, windowRect);
		//GUI.BeginGroup(windowRect);
		windowRect = GUI.Window(0, windowRect, DrawDebugWindow, "Debug Menu");
		//GUI.EndGroup();
	}

	void DrawDebugWindow(int windowID)
	{
		// スクロールバーの表示
		//scrollPosition = GUILayout.BeginScrollView(scrollPosition, );
		//scrollPosition = GUI.BeginScrollView(ScrollRect, scrollPosition, windowRect);
		//scrollPosition = GUI.BeginScrollView(ScrollRect, scrollPosition, windowRect);

		scrollPosition = GUILayout.BeginScrollView(
			scrollPosition
		);

		// マッチリセット
		if (GUILayout.Button("Match Restart"))
		{
			// MatchManager経由でリセットを実行
			if (MatchManager.Instance != null)
			{
				MatchManager.Instance.GoalEvent(CharacterTeam.Player);
			}
			else
			{
				Debug.LogWarning("MatchManagerが見つかりません");
			}
		}

		// マッチマネージャからデバッグ呼び出し 
		MatchManager.Instance.DebugGUI();

		//GUILayout.HorizontalSlider()


		GUI.DragWindow(new Rect(0, 0, 10000, 20));
		GUILayout.EndScrollView();
		//GUI.EndScrollView();
	}
}
