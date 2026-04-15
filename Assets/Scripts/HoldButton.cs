using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnityEvent onPress;
    public UnityEvent onRelease;

    public void OnPointerDown(PointerEventData eventData) => onPress.Invoke();
    public void OnPointerUp(PointerEventData eventData) => onRelease.Invoke();
}
