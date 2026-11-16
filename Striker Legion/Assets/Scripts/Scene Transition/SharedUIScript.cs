using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SharedUIScript : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
    }

    // ëOÇÃÉVÅ[ÉìÇ…ñﬂÇÈ
    public void SceneBackEvent()
    {
        SceneTransitionManager.instance.LoadPreviousScene();
    }
}
