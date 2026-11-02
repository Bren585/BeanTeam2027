using NUnit.Framework;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TrainingUIScript : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField]
    private List<GameObject> Buttons;

    [SerializeField]
    private Image outline;

    [SerializeField]
    private Text explanation1;

    [SerializeField]
    private Text explanation2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventSystem.current.SetSelectedGameObject(Buttons[0]);
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < Buttons.Count; ++i )
        {
            if (EventSystem.current.currentSelectedGameObject == Buttons[i])
            {
                GameObject selected = EventSystem.current.currentSelectedGameObject;
                RectTransform selectedButtonTransform = selected.GetComponent<RectTransform>();
                float3 pos = selectedButtonTransform.position;
                outline.GetComponent<RectTransform>().position = pos;
                float3 scale = selectedButtonTransform.localScale;
                outline.GetComponent<RectTransform>().localScale = scale;

                if (selected != null && selected.TryGetComponent<ModeButton>(out var modeButton))
                {
                    SetExplanation(modeButton.GetExplanation());
                }
            }
        }
    }

    private void SetExplanation(ModeExplanationScriptableObject explanationObject)
    {
        explanation1.text = explanationObject.modeName;
        explanation2.text = explanationObject.explanation;
    }

    public void PassTrainingMode()
    {
        SceneTransitionManager.instance.LoadScene((int)SceneID.PassTraining);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
