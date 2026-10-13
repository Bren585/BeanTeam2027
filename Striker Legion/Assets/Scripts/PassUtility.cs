using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using UnityEngine;
using UnityEngine.UIElements;

public static class PassUtility
{
    public static bool IsPassSuccess(Transform Start, Transform End, LayerMask OpponentLayer)
	{
		return IsPassSuccess(Start.position, End.position, OpponentLayer);
	}

    public static bool IsPassSuccess(Vector3 Start, Vector3 End, LayerMask OpponentLayer)
    {
		// レイの長さ
		float rayDistance = Vector3.Distance(End, Start);

		// レイを作成
		Ray ray = new Ray(Start, Vector3.Normalize(End - Start));

		// 敵に当たらない＝成功
		if (!Physics.Raycast(ray, out RaycastHit hit, rayDistance, OpponentLayer))
			return true;

		// ヒットしたオブジェクトがキャラクターかではない場合、成功判定
		if (!hit.collider.CompareTag("Character"))
		{
			return true;
		}

		return false;
    }

	// パスを受けられる仲間がいるかどうか
	public static bool CanPassToAnyAlly(IReadOnlyList<SampleCharacter> ActiveCharacters, SampleCharacter PassCharacter)
	{
		// キャラクターのチーム
		CharacterTeam team = PassCharacter.TeamType;


		// 敵のレイヤー
		LayerMask OpponentLayer = LayerMask.NameToLayer(team == CharacterTeam.Player ? "Enemy" : "Player");

		// パスを受けられるキャラクターを探す
		for (int i = 0; i < ActiveCharacters.Count; i++ )
		{
			// パスを出すキャラと同一にならないようにする
			if (ActiveCharacters[i] == PassCharacter)
				continue;

			// スタンしておらず、パスが通る対象がいるならTrueを返す
			if (PassUtility.IsPassSuccess(PassCharacter.transform, ActiveCharacters[i].transform, OpponentLayer))
				return true;
		}

		return false;
	}

	// パスをするコスト(低いほどパスをしたがる)
	public static float CostPassToAnyAlly(IReadOnlyList<SampleCharacter> ActiveCharacters, SampleCharacter PassCharacter)
	{
		// キャラクターのチーム
		CharacterTeam team = PassCharacter.TeamType;

		// 敵のレイヤー
		LayerMask OpponentLayer = LayerMask.NameToLayer(team == CharacterTeam.Player ? "Enemy" : "Player");

		float MinCost = 1.0f;

		// パスを受けられるキャラクターを探す
		for (int i = 0; i < ActiveCharacters.Count; i++)
		{
			float cost = 0.0f;

			// パスを出すキャラと同一にならないようにする
			if (ActiveCharacters[i] == PassCharacter)
				continue;

			// パスが通らないなら飛ばす
			if (!PassUtility.IsPassSuccess(PassCharacter.transform, ActiveCharacters[i].transform, OpponentLayer))
				cost += 0.6f;


			// パスの対象のゴールまでの距離
			float TargetGoalDistance = ActiveCharacters[i].GetDistanceToGoal();

			if (TargetGoalDistance > PassCharacter.GetDistanceToGoal())
				cost += 0.4f;

			if (MinCost > cost)
				MinCost = cost;
		}

		return MinCost;
	}
}
