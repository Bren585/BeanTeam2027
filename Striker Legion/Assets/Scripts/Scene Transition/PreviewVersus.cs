using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

public class PreviewVersus
{
    [SerializeField]
    private Transform leftStartPosition;

    [SerializeField]
    private Transform rightStartPosition;

    // 左側の動物の位置オフセット
    [SerializeField]
    private Transform[] leftOffsets;

    // 右側の動物の位置オフセット
    [SerializeField]
    private Transform[] rightOffsets;

    // 表示前のランダムシャッフル
    public void Shuffle<T>(T[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            int rand = Random.Range(i, array.Length);

            (array[i], array[rand]) =
            (array[rand], array[i]);
        }
    }

    public void AddLeftOffset(GameObject[] animalObjects)
    {

    }

    public void AddRightOffset(Sprite[] animalObjects)
    {
       
    }
}
