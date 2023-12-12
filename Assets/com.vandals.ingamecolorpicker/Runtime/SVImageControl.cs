using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SVImageControl : MonoBehaviour, IDragHandler, IPointerClickHandler
{
    [SerializeField] ColorPickerControl colorPicker;
    [SerializeField] Image pickerImage;

    RectTransform rectTransform; 
    RectTransform pickerTransform;

    void Start()
    {
        rectTransform = transform as RectTransform;
        pickerTransform = pickerImage.transform as RectTransform;
        pickerTransform.position = rectTransform.sizeDelta * -0.5f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdateColor(eventData.position);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UpdateColor(eventData.position);
    }

    void UpdateColor(Vector3 position)
    {
        Vector2 pos = rectTransform.InverseTransformPoint(position);

        float deltaX = rectTransform.sizeDelta.x * 0.5f;
        float deltaY = rectTransform.sizeDelta.y * 0.5f;

        if (pos.x < -deltaX) 
            pos.x = -deltaX;
        else if (pos.x > deltaX) 
            pos.x = deltaX;

        if (pos.y < -deltaY)
            pos.y = -deltaY;
        else if (pos.y > deltaY)
            pos.y = deltaY;

        float x = pos.x + deltaX;
        float y = pos.y + deltaY;

        float xNorm = x / rectTransform.sizeDelta.x;
        float yNorm = y / rectTransform.sizeDelta.y;

        pickerTransform.localPosition = pos;
        pickerImage.color = Color.HSVToRGB(0, 0, 1 - yNorm);

        colorPicker.SetSV(xNorm, yNorm);
    }
}
