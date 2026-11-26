using UnityEngine;
using UnityEngine.EventSystems;

public class MousePointerSelect : MonoBehaviour,
    IPointerEnterHandler
{
    //カーソルに選ばれると自身のオブジェクトを引き渡す
    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
