using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class ButtonHandler : MonoBehaviour, IPointerClickHandler
{
    public Action OnButtonClicked;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnButtonClicked?.Invoke();
    }
}
