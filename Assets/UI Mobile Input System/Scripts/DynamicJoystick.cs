using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

namespace MobileController
{
    public enum AxisOptions { Both, Horizontal, Vertical }
    public class Joystick : OnScreenControl, IPointerDownHandler, IDragHandler, IPointerUpHandler, IEndDragHandler
    {
        [InputControl(layout = "Vector2")]
        [SerializeField] string m_ControlPath;
        protected override string controlPathInternal { get => m_ControlPath; set => m_ControlPath = value; }
        public int inputMultiplier = 1;

        [Header("Handler")]
        [SerializeField] protected float handleRange = 1;
        [SerializeField] private float deadZone = 0;

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
            OnDrag(eventData);
        }
        public void OnDrag(PointerEventData eventData)
        {
            cam = null;
            if (cam == null && canvas.renderMode == RenderMode.ScreenSpaceCamera)
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
            SetInput(input * inputMultiplier);

        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            input = Vector2.zero;
            handle.anchoredPosition = Vector2.zero;
            SetInput(Vector2.zero);
        }
        public virtual void OnEndDrag(PointerEventData eventData)
        {
            input = Vector2.zero;
            SetInput(Vector2.zero);
        }

        protected void FormatInput()
        {
            if (axisOptions == AxisOptions.Horizontal)
                input = new Vector2(input.x, 0f);
            else if (axisOptions == AxisOptions.Vertical)
                input = new Vector2(0f, input.y);
        }

        float SnapFloat(float value, AxisOptions snapAxis)
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

        protected Vector2 ScreenPointToAnchoredPosition(Vector2 screenPosition)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(baseRect, screenPosition, cam, out Vector2 localPoint))
            {
                Vector2 pivotOffset = baseRect.pivot * baseRect.sizeDelta;
                return localPoint - (background.anchorMax * baseRect.sizeDelta) + pivotOffset;
            }
            return Vector2.zero;
        }

        public void SetInput(Vector2 input)
        {
            SendValueToControl(input * inputMultiplier);
        }

    }
    public class DynamicJoystick : Joystick
    {
        [Header("Dynamic")]
        [SerializeField] bool dynamicMove = true;
        [SerializeField] bool moveToWhereClicked = false;
        [SerializeField] RectTransform rectTransformLimit;
        [SerializeField] float limitAdjust = 0;
        [SerializeField] float moveThreshold = 1;
        [SerializeField] private bool verifyIfStopDrag;

        [Header("Display")]
        [SerializeField] bool hideWhenNotTouching = true;
        [SerializeField] bool hideCompletely = false;

        readonly Vector3[] canvasCorner = new Vector3[4];
        Vector2 StartingPosition;

        PointerEventData cached = new PointerEventData(EventSystem.current);
        PointerEventData pointer = new PointerEventData(EventSystem.current);

        public float MoveThreshold { get { return moveThreshold; } set { moveThreshold = Mathf.Abs(value); } }

        Image backgroundImage;
        Image handleImage;

        Vector2 lastPointerPos;

        int currentPointer = -99;

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

            pointer = new PointerEventData(EventSystem.current);

            base.Start();
        }

        [ContextMenu("Force")]
        public void ForcePointerUp()
        {
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            ExecuteEvents.Execute(gameObject, pointer, ExecuteEvents.endDragHandler);
        }
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (currentPointer != -99 && currentPointer != eventData.pointerId) return;
            if (Time.timeScale <= 0) return;

            cam = null;
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
                cam = canvas.worldCamera;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(background, eventData.position, cam, out Vector2 localPoint);

            if (dynamicMove || moveToWhereClicked)
                background.localPosition = localPoint + StartingPosition;

            if (hideWhenNotTouching && hideCompletely == false)
            {
                backgroundImage.enabled = true;
                handleImage.enabled = true;
            }

            base.OnPointerDown(eventData);
            pointer = eventData;
            currentPointer = pointer.pointerId;
        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (currentPointer != -99 && currentPointer != eventData.pointerId) return;
            if (Time.timeScale <= 0) return;

            // return to start position
            if (dynamicMove || moveToWhereClicked)
                background.anchoredPosition = startPositionBackground;

            if (hideWhenNotTouching && hideCompletely == false)
            {
                backgroundImage.enabled = false;
                handleImage.enabled = false;
            }

            base.OnPointerUp(eventData);
            pointer = eventData;
            currentPointer = -99;
        }
        public override void OnEndDrag(PointerEventData eventData)
        {
            if (currentPointer != -99 && currentPointer != eventData.pointerId) return;
            base.OnEndDrag(eventData);
            pointer = eventData;
            currentPointer = -99;
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

        void Update()
        {
            if (verifyIfStopDrag && pointer.position == lastPointerPos)
            {
                input = Vector2.zero;
                SetInput(Vector2.zero);
            }

            lastPointerPos = pointer.position;
        }
    }
}