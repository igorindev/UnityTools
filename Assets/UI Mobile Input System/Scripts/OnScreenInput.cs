using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class OnScreenInput : OnScreenControl, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Customization")]
    [SerializeField] bool dynamicMove;
    [SerializeField] bool hideWhenNotTouching;
    
    [Header("Transforms")]
    [SerializeField] RectTransform handleRect;
    [SerializeField] RectTransform joystickRect;

    [FormerlySerializedAs("movementRange")]
    [SerializeField]
    float movementRange = 100;

    [InputControl(layout = "Vector2")]
    [SerializeField] string inputControlPath;

    Vector3 startPos;
    Vector2 pointerDownPos;
    Vector2 newPos;
    Vector2 delta;

    public float MovementRange
    {
        get => movementRange;
        set => movementRange = value;
    }
    protected override string controlPathInternal
    {
        get => inputControlPath;
        set => inputControlPath = value;
    }

    Image handle;
    Image joystick;

    void Start()
    {
        handle = handleRect.GetComponent<Image>();
        joystick = joystickRect.GetComponent<Image>();

        startPos = handleRect.anchoredPosition;

        if (hideWhenNotTouching)
        {
            joystick.enabled = false;
            handle.enabled = false;
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData == null)
            throw new System.ArgumentNullException(nameof(eventData));

        RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickRect, eventData.position, eventData.pressEventCamera, out pointerDownPos);

        if (dynamicMove)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, eventData.position, eventData.pressEventCamera, out pointerDownPos);
            ((RectTransform)joystick.transform).anchoredPosition = pointerDownPos;
            startPos = handleRect.anchoredPosition = Vector2.zero;
        }
        
        if (hideWhenNotTouching)
        {
            joystick.enabled = true;
            handle.enabled = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData == null)
            throw new System.ArgumentNullException(nameof(eventData));
        Vector2 position;
        if (dynamicMove)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, eventData.position, eventData.pressEventCamera, out position);
        }
        else
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickRect, eventData.position, eventData.pressEventCamera, out position);
        }
        delta = position - pointerDownPos;

        delta = Vector2.ClampMagnitude(delta, MovementRange);
        handleRect.anchoredPosition = startPos + (Vector3)delta;

        newPos.Set(delta.x / MovementRange, delta.y / MovementRange);
        SendValueToControl(newPos);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        handleRect.anchoredPosition = startPos;
        SendValueToControl(Vector2.zero);

        if (hideWhenNotTouching)
        {
            joystick.enabled = false;
            handle.enabled = false;
        }
    }
}
