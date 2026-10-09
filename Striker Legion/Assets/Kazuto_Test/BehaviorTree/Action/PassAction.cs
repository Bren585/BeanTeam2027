using System;
using System.Collections.Generic;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Pass", 
    story: "Pass To Other Character", 
    category: "Action", 
    id: "072f5c0b970ac8587b9f39a11da9c895")]
public partial class PassAction : ActionBase
{
	IReadOnlyList<SampleCharacter> ActiveCharacters;

	protected override Status OnStart()
	{
		// 基底クラスの初期化
		base.OnStart();

		// 行動をデバッグで表示
		characterComponent.DebugText.SetText("パスをする");

		// アクティブなキャラ
		ActiveCharacters = MatchManager.Instance.GetActiveCharacters(characterComponent.TeamType);

		return Status.Running;
	}

	protected override Status OnUpdate()
	{
		// キャラがボールをロストしていたら、失敗で終了
		if (!characterComponent.isHoldingBall)
		{
			Debug.Log(characterComponent.name + ":パスの前にボールを奪われた");
			return Status.Failure;
		}
		if(ActiveCharacters.Count <= 0)
		{
			Debug.Log(characterComponent.name + ":アクティブキャラがゼロ");
			return Status.Failure;
		}

		Transform transform = GameObject.transform;

		// パスをするキャラのチーム
		CharacterTeam team = characterComponent.TeamType;
	
		// 敵のレイヤー
		LayerMask OpponentLayer = LayerMask.NameToLayer(team == CharacterTeam.Player ? "Enemy" : "Player");

		SampleCharacter TargetCharacter = null;
		float minDistance = float.MaxValue;
		// パスを受けられるキャラクターを探す
		foreach (SampleCharacter ActiveCharacter in ActiveCharacters)
		{
			// パスを出すキャラと同一にならないようにする
			if (ActiveCharacter == characterComponent)
				continue;
			
			// パスが通らないなら飛ばす
			if (!PassUtility.IsPassSuccess(characterComponent.transform, ActiveCharacter.transform, OpponentLayer))
				continue;
			
			// 条件をクリアした中で、最も近くにいるキャラを対象とする
			float Distance = Vector3.Distance(characterComponent.transform.position, ActiveCharacter.transform.position);
			if (Distance < minDistance)
			{
				TargetCharacter = ActiveCharacter;
				minDistance = Distance;
			}
		}

		if (TargetCharacter == null)
		{
			return Status.Failure;
		}
		else
		{
			TargetCharacter.isHoldingBall = true;
			characterComponent.isHoldingBall = false;
		}

		// 何回もパスしないよう、パスをしたらスタンする
		//characterComponent.StartStan();
		return Status.Success;
	}

	protected override void OnEnd()
    {
	}
}

