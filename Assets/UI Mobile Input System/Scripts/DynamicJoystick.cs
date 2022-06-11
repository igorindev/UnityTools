using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

public enum AxisOptions { Both, Horizontal, Vertical }
public class Joystick : OnScreenControl, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [InputControl(layout = "Vector2")]
    [SerializeField] string m_ControlPath;
    protected override string controlPathInternal { get => m_ControlPath; set => m_ControlPath = value; }
    public int inputMultiplier = 1;
    [SerializeField] private bool keepSendingEventOnStop;

    [Header("Handler")]
    [SerializeField] protected float handleRange = 1;
    [SerializeField] private float deadZone = 0;
    bool interacting;

    [Header("Axis")]
    public AxisOptions axisOptions = AxisOptions.Both;
    [SerializeField] private bool snapX = false;
    [SerializeField] private bool snapY = false;

    [Header("Rects")]
    [SerializeField] protected RectTransform background = null;
    [SerializeField] protected RectTransform handle = null;

    protected RectTransform baseRect = null;
    protected Vector2 startPositionBackground;

    protected Canvas canvas;
    protected Camera cam;

    protected Vector2 input = Vector2.zero;
    public float Horizontal { get { return (snapX) ? SnapFloat(input.x, AxisOptions.Horizontal) : input.x; } }
    public float Vertical { get { return (snapY) ? SnapFloat(input.y, AxisOptions.Vertical) : input.y; } }
    public Vector2 Direction { get { return new Vector2(Horizontal, Vertical); } }
    public float HandleRange
    {
        get { return handleRange; }
        set { handleRange = Mathf.Abs(value); }
    }
    public float DeadZone
    {
        get { return deadZone; }
        set { deadZone = Mathf.Abs(value); }
    }
    public bool SnapX { get { return snapX; } set { snapX = value; } }
    public bool SnapY { get { return snapY; } set { snapY = value; } }

    protected virtual void Start()
    {
        HandleRange = handleRange;
        DeadZone = deadZone;
        baseRect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            Debug.LogError("The Joystick is not placed inside a canvas");

        Vector2 center = new Vector2(0.5f, 0.5f);
        startPositionBackground = background.anchoredPosition;
        background.pivot = center;
        handle.anchorMin = center;
        handle.anchorMax = center;
        handle.pivot = center;
        handle.anchoredPosition = Vector2.zero;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        interacting = true;
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        cam = null;
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            cam = canvas.worldCamera;

        Vector2 position = RectTransformUtility.WorldToScreenPoint(cam, background.position);
        Vector2 radius = background.sizeDelta / 2;
        input = (eventData.position - position) / (radius * canvas.scaleFactor);
        FormatInput();
        HandleInput(input.magnitude, input.normalized, radius, cam);
        Vector2 newPos = input * radius * handleRange;
        handle.anchoredPosition = newPos;
    }

    protected virtual void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
    {
        if (magnitude > deadZone)
        {
            if (magnitude > 1)
                input = normalised;
        }
        else
        {
            input = Vector2.zero;
        }
        SendValueToControl(input * inputMultiplier);
    }

    protected void FormatInput()
    {
        if (axisOptions == AxisOptions.Horizontal)
            input = new Vector2(input.x, 0f);
        else if (axisOptions == AxisOptions.Vertical)
            input = new Vector2(0f, input.y);
    }

    private float SnapFloat(float value, AxisOptions snapAxis)
    {
        if (value == 0)
            return value;

        if (axisOptions == AxisOptions.Both)
        {
            float angle = Vector2.Angle(input, Vector2.up);
            if (snapAxis == AxisOptions.Horizontal)
            {
                if (angle < 22.5f || angle > 157.5f)
                    return 0;
                else
                    return (value > 0) ? 1 : -1;
            }
            else if (snapAxis == AxisOptions.Vertical)
            {
                if (angle > 67.5f && angle < 112.5f)
                    return 0;
                else
                    return (value > 0) ? 1 : -1;
            }
            return value;
        }
        else
        {
            if (value > 0)
                return 1;
            if (value < 0)
                return -1;
        }
        return 0;
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        interacting = false;
        input = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
        SendValueToControl(input * inputMultiplier);
    }

    void Update()
    {
        if (keepSendingEventOnStop && interacting)
        {
            SendValueToControl(input * inputMultiplier);
        }
    }

    protected Vector2 ScreenPointToAnchoredPosition(Vector2 screenPosition)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(baseRect, screenPosition, cam, out Vector2 localPoint))
        {
            Vector2 pivotOffset = baseRect.pivot * baseRect.sizeDelta;
            return localPoint - (background.anchorMax * baseRect.sizeDelta) + pivotOffset;
        }
        return Vector2.zero;
    }
}
public class DynamicJoystick : Joystick
{
    [Header("Dynamic")]
    [SerializeField] bool dynamicMove = true;
    [SerializeField] RectTransform rectTransformLimit;
    [SerializeField] float limitAdjust = 0;
    [SerializeField] float moveThreshold = 1;

    [Header("Display")]
    [SerializeField] bool hideWhenNotTouching = true;
    [SerializeField] bool hideCompletely = false;

    readonly Vector3[] canvasCorner = new Vector3[4];
    Vector2 StartingPosition;
    public float MoveThreshold { get { return moveThreshold; } set { moveThreshold = Mathf.Abs(value); } }

    Image backgroundImage;
    Image handleImage;

    protected override void Start()
    {
        StartingPosition = background.localPosition;
        MoveThreshold = moveThreshold;

        backgroundImage = background.GetComponent<Image>();
        handleImage = handle.GetComponent<Image>();

        if (hideWhenNotTouching || hideCompletely)
        {
            backgroundImage.enabled = false;
            handleImage.enabled = false;
        }

        base.Start();
    }

    public void ForcePointerUp()
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(gameObject, pointer, ExecuteEvents.pointerUpHandler);
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (Time.timeScale <= 0) return;

        cam = null;
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            cam = canvas.worldCamera;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(background, eventData.position, cam, out Vector2 localPoint);
        
        if (dynamicMove)
            background.localPosition = localPoint + StartingPosition; // @Dan: This is Hardcoded according to the base localPosition. Changing the canvas will cause things to go awry.

        if (hideWhenNotTouching && hideCompletely == false)
        {
            backgroundImage.enabled = true;
            handleImage.enabled = true;
        }

        base.OnPointerDown(eventData);
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        if (Time.timeScale <= 0) return;

        // return to start position
        if (dynamicMove)
            background.anchoredPosition = startPositionBackground;

        if (hideWhenNotTouching && hideCompletely == false)
        {
            backgroundImage.enabled = false;
            handleImage.enabled = false;
        }

        base.OnPointerUp(eventData);
    }
    protected override void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
    {
        if (magnitude > moveThreshold && dynamicMove)
        {
            Vector2 difference = normalised * (magnitude - moveThreshold) * radius;
            Vector2 newPos = background.anchoredPosition + difference;
            rectTransformLimit.GetLocalCorners(canvasCorner);
            if (newPos.x - background.rect.width * limitAdjust > canvasCorner[2].x) { newPos.x = canvasCorner[2].x + background.rect.width * limitAdjust; }
            if (newPos.y - background.rect.height * limitAdjust > canvasCorner[2].y) { newPos.y = canvasCorner[2].y + background.rect.height * limitAdjust; }
            if (newPos.x + background.rect.width * limitAdjust < canvasCorner[0].x) { newPos.x = canvasCorner[0].x - background.rect.width * limitAdjust; }
            if (newPos.y + background.rect.height * limitAdjust < canvasCorner[0].y) { newPos.y = canvasCorner[0].y - background.rect.height * limitAdjust; }
            background.anchoredPosition = newPos;
        }
        base.HandleInput(magnitude, normalised, radius, cam);
    }
}