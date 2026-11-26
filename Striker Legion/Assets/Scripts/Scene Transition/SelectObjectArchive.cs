using UnityEngine;
using UnityEngine.EventSystems;

public class SelectObjectArchive : MonoBehaviour
{
    public static SelectObjectArchive instance { get; private set; }

    // 最後に選択したオブジェクト情報
    public GameObject objectArchive;

    // 代わりに代入するオブジェクト
    public GameObject substituteObject;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        objectArchive = null;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateObjectArchive();
    }

    // アーカイブの更新
    private void UpdateObjectArchive()
    {
        // オブジェクトが選択中でない場合アーカイブまたは代わりのオブジェクトで復旧する
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            if(objectArchive != null)
            {
                EventSystem.current.SetSelectedGameObject(objectArchive);
            }
            else if (substituteObject != null)
                EventSystem.current.SetSelectedGameObject(substituteObject);
        }
        // 選択中の場合アーカイブを更新
        else
        {
            objectArchive = EventSystem.current.currentSelectedGameObject;
        }
    }

    // アーカイブオブジェクトを取得
    public GameObject GetObjectArchive()
    {
        return objectArchive;
    }

    // 代わりのオブジェクトを更新
    public void SetSubstituteObject(GameObject substitute)
    {
        substituteObject = substitute;
    }

    //アーカイブの削除
    public void ClearObjectArchive()
    {
        objectArchive = null;
    }
}
