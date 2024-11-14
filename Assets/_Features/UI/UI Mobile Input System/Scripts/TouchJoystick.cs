using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class TouchJoystick : OnScreenControl
{
    [InputControl(layout = "Vector2")]
    [SerializeField] string m_ControlPath;
    [SerializeField] RectTransform touchArea;
    protected override string controlPathInternal { get => m_ControlPath; set => m_ControlPath = value; }

    List<RaycastResult> results = new List<RaycastResult>();

    ReadOnlyArray<Touch> activeTouches;

    protected override void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        TouchSimulation.Enable();

        Touch.onFingerDown += FingerDown;
        Touch.onFingerUp += FingerUp;
        Touch.onFingerMove += FingerMove;

        base.OnEnable();
    }
    protected override void OnDisable()
    {
        Touch.onFingerDown -= FingerDown;
        Touch.onFingerUp -= FingerUp;
        Touch.onFingerMove -= FingerMove;

        EnhancedTouchSupport.Disable();
        TouchSimulation.Disable();

        base.OnDisable();
    }
    void FingerDown(Finger finger)
    {

    }
    void FingerUp(Finger finger)
    {
        SendValueToControl(Vector2.zero);
    }
    void FingerMove(Finger finger)
    {
    }

    void Update()
    {
        activeTouches = Touch.activeTouches;
        for (var i = 0; i < activeTouches.Count; ++i)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(touchArea, activeTouches[i].screenPosition))
            {            
                //if (IsPointerOverUIObject(activeTouches[i]))
                //{
                //    Debug.Log("Cant press");
                //    SendValueToControl(Vector2.zero);
                //    return;
                //}

                if (activeTouches[i].phase == UnityEngine.InputSystem.TouchPhase.Stationary)
                {
                    SendValueToControl(Vector2.zero);
                    Debug.Log("Stationary");
                }
                else if (activeTouches[i].phase == UnityEngine.InputSystem.TouchPhase.Ended)
                {
                    SendValueToControl(Vector2.zero);
                    Debug.Log("Ended");
                }
                else
                {
                    SendValueToControl(activeTouches[i].delta);
                    Debug.Log(activeTouches[i].delta + " | " + activeTouches[i].delta.normalized);
                }
            }
            else
            {
                if (activeTouches[i].phase == UnityEngine.InputSystem.TouchPhase.Stationary)
                {
                    SendValueToControl(Vector2.zero);
                    Debug.Log("Stationary");
                }
            }
        }
    }

    bool IsPointerOverUIObject(Touch touch)
    {
        results.Clear();

        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = touch.screenPosition;

        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
