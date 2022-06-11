using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovableWindow : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    RectTransform limitWindow;
    RectTransform popupTransform;

    // Dimensions of the popup divided by 2
    Vector2 halfSize;

    Vector2 normalizedPosition;

    // Coroutines for simple code-based animations
    IEnumerator moveToPosCoroutine = null;

    void Awake()
    {
        popupTransform = (RectTransform)transform;
        limitWindow = (RectTransform)GetComponentInParent<Canvas>().transform;

        halfSize = popupTransform.sizeDelta * 0.5f;

        Vector2 pos = popupTransform.anchoredPosition;
        if (pos.x != 0f || pos.y != 0f)
            normalizedPosition = pos.normalized; // Respect the initial popup position set in the prefab
        else
            normalizedPosition = new Vector2(0.5f, 0f); // Right edge by default
    }

    // A simple smooth movement animation
    private IEnumerator MoveToPosAnimation(Vector2 targetPos)
    {
        float modifier = 0f;
        Vector2 initialPos = popupTransform.anchoredPosition;

        while (modifier < 1f)
        {
            modifier += 4f * Time.unscaledDeltaTime;
            popupTransform.anchoredPosition = Vector2.Lerp(initialPos, targetPos, modifier);

            yield return null;
        }
    }

    // Hides the log window and shows the popup
    public void Show()
    {
        // Update position in case resolution was changed while the popup was hidden
        UpdatePosition(true);
    }

    public void OnBeginDrag(PointerEventData data)
    {
        // If a smooth movement animation is in progress, cancel it
        if (moveToPosCoroutine != null)
        {
            StopCoroutine(moveToPosCoroutine);
            moveToPosCoroutine = null;
        }
    }

    // Reposition the popup
    public void OnDrag(PointerEventData data)
    {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(limitWindow, data.position, data.pressEventCamera, out localPoint))
            popupTransform.anchoredPosition = localPoint;
    }

    // Smoothly translate the popup to the nearest edge
    public void OnEndDrag(PointerEventData data)
    {
        UpdatePosition(false);
    }

    public void UpdatePosition(bool immediately)
    {
        Vector2 canvasSize = limitWindow.rect.size;

        float canvasWidth = canvasSize.x;
        float canvasHeight = canvasSize.y;

        // normalizedPosition allows us to glue the popup to a specific edge of the screen. It becomes useful when
        // the popup is at the right edge and we switch from portrait screen orientation to landscape screen orientation.
        // Without normalizedPosition, popup could jump to bottom or top edges instead of staying at the right edge
        Vector2 pos = immediately ? new Vector2(normalizedPosition.x * canvasWidth, normalizedPosition.y * canvasHeight) : popupTransform.anchoredPosition;

        // Find distances to all four edges
        float distToLeft = canvasWidth * 0.5f + pos.x;
        float distToRight = canvasWidth - distToLeft;

        float distToBottom = canvasHeight * 0.5f + pos.y;
        float distToTop = canvasHeight - distToBottom;

        float horDistance = Mathf.Min(distToLeft, distToRight);
        float vertDistance = Mathf.Min(distToBottom, distToTop);

        // Find the nearest edge's coordinates
        if (horDistance < vertDistance)
        {
            if (distToLeft < distToRight)
                pos = new Vector2(canvasWidth * -0.5f + halfSize.x, pos.y);
            else
                pos = new Vector2(canvasWidth * 0.5f - halfSize.x, pos.y);

            pos.y = Mathf.Clamp(pos.y, canvasHeight * -0.5f + halfSize.y, canvasHeight * 0.5f - halfSize.y);
        }
        else
        {
            if (distToBottom < distToTop)
                pos = new Vector2(pos.x, canvasHeight * -0.5f + halfSize.y);
            else
                pos = new Vector2(pos.x, canvasHeight * 0.5f - halfSize.y);

            pos.x = Mathf.Clamp(pos.x, canvasWidth * -0.5f + halfSize.x, canvasWidth * 0.5f - halfSize.x);
        }

        normalizedPosition.Set(pos.x / canvasWidth, pos.y / canvasHeight);

        // If another smooth movement animation is in progress, cancel it
        if (moveToPosCoroutine != null)
        {
            StopCoroutine(moveToPosCoroutine);
            moveToPosCoroutine = null;
        }

        if (immediately)
            popupTransform.anchoredPosition = pos;
        else
        {
            // Smoothly translate the popup to the specified position
            moveToPosCoroutine = MoveToPosAnimation(pos);
            StartCoroutine(moveToPosCoroutine);
        }
    }
}
