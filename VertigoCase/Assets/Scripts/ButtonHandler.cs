using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class ButtonHandler : MonoBehaviour, IPointerClickHandler
{
    // Olayı tanımlama
    public Action OnButtonClicked;

    public void OnPointerClick(PointerEventData eventData)
    {
        // Olayı tetikle
        OnButtonClicked?.Invoke();
    }
}
