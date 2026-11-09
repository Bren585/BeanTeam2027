using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectManager : MonoBehaviour
{
    private CharacterSelect characterSelect;

    private void Start()
    {
        characterSelect = GetComponent<CharacterSelect>();
    }

    private void Update()
    {
        // キャラクター選択シーンの場合のみ処理
        if (!SceneTransitionManager.instance.InProgressScene((int)SceneID.CharacterSelect))
            return;

        StateExchange();

        switch (CharacterSelectManager.instance.CurrentExecuteState())
        {
            case ExecuteState.None:
                break;

            case ExecuteState.CharacterSelect:
                characterSelect.Update();
                break;

            case ExecuteState.PositionSelect:
                
                break;

            case ExecuteState.Complete:

                break;
        }
    }

    // ステート切り替え
    private void StateExchange()
    {
        switch (CharacterSelectManager.instance.CurrentExecuteState())
        {
            case ExecuteState.CharacterSelect:
                if(!characterSelect.GetIsActive())
                {
                    characterSelect.Initialize();
                }
                break;

            case ExecuteState.PositionSelect:

                break;

            case ExecuteState.Complete:

                break;
        }
    }

    // 選択を戻る
    public void BackSelectItem(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        BackSelectItem();
    }

    public void BackSelectItem()
    {
        // ステートに合わせて切り替え
        switch (CharacterSelectManager.instance.CurrentExecuteState())
        {
            case ExecuteState.None:
                break;

            case ExecuteState.CharacterSelect:
                characterSelect.BackSelectItem();
                break;

            // ポジション選択はポジション割り当てを戻す。出来なければキャラクター情報の最後尾を削除
            case ExecuteState.PositionSelect:
                if (!CharacterSelectManager.instance.DeleteLastPositionData())
                {
                    CharacterSelectManager.instance.DeleteLastCharacterData();
                }
                break;

            // 次シーンへの選択はポジション割り当ての最後尾を削除
            case ExecuteState.Complete:
                CharacterSelectManager.instance.DeleteLastPositionData();
                break;
        }
    }
}
