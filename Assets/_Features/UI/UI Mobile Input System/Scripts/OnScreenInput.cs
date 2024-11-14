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
    [SerializeField] RectTransform backgroundRect;

    Image handle;
    Image background;

    Vector2 newPos = Vector2.zero;
    Vector2 delta = Vector2.zero;

    void Start()
    {
        handle = handleRect.GetComponent<Image>();
        background = backgroundRect.GetComponent<Image>();

        m_StartPos = handleRect.anchoredPosition;

        if (hideWhenNotTouching)
        {
            background.enabled = false;
            handle.enabled = false;
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData == null)
            throw new System.ArgumentNullException(nameof(eventData));

        RectTransformUtility.ScreenPointToLocalPointInRectangle(backgroundRect, eventData.position, eventData.pressEventCamera, out m_PointerDownPos);

        if (dynamicMove)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, eventData.position, eventData.pressEventCamera, out m_PointerDownPos);
            ((RectTransform)background.transform).anchoredPosition = m_PointerDownPos;
            m_StartPos = handleRect.anchoredPosition = Vector2.zero;
        }
        
        if (hideWhenNotTouching)
        {
            background.enabled = true;
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
            RectTransformUtility.ScreenPointToLocalPointInRectangle(backgroundRect, eventData.position, eventData.pressEventCamera, out position);
        }
        delta = position - m_PointerDownPos;

        delta = Vector2.ClampMagnitude(delta, MovementRange);
        handleRect.anchoredPosition = m_StartPos + (Vector3)delta;

        newPos.Set(delta.x / MovementRange, delta.y / MovementRange);
        SendValueToControl(newPos);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        handleRect.anchoredPosition = m_StartPos;
        SendValueToControl(Vector2.zero);

        if (hideWhenNotTouching)
        {
            background.enabled = false;
            handle.enabled = false;
        }
    }

    public float MovementRange
    {
        get => m_MovementRange;
        set => m_MovementRange = value;
    }

    [FormerlySerializedAs("movementRange")]
    [SerializeField]
    private float m_MovementRange = 50;

    [InputControl(layout = "Vector2")]
    [SerializeField]
    private string m_ControlPath;

    private Vector3 m_StartPos;
    private Vector2 m_PointerDownPos;

    protected override string controlPathInternal
    {
        get => m_ControlPath;
        set => m_ControlPath = value;
    }
}