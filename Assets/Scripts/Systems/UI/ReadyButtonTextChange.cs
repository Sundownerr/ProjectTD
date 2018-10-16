using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ReadyButtonTextChange : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Text ReadyButtonText;

    void Start ()
    {
        ReadyButtonText = GetComponentInChildren<Text>();	
	}

    public void OnPointerEnter(PointerEventData eventData)
    {
        ReadyButtonText.text = "I will defend this land!";
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ReadyButtonText.text = "Are you ready?";
    }
}
