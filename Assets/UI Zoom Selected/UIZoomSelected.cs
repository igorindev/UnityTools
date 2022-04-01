using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIZoomSelected : MonoBehaviour
{
    [SerializeField] float normalScale = 1f;
    [SerializeField] float zoomScale = 1.5f;
    [SerializeField] float zoomDuration = 0.4f;
    [SerializeField] Transform graphic;

    Coroutine zoom;
    void Start()
    {
        if (TryGetComponent(out Toggle t))
        {
            if (t.isOn)
            { 
                graphic.localScale = Vector3.one * zoomScale; 
            }
        }
    }
    public void SetSelected(bool value)
    {
        if (value)
        {
            if(ReferenceEquals(zoom, null) == false)
            {
                StopCoroutine(zoom);
            }
            zoom = StartCoroutine(ZoomIn());
        }
        else
        {
            if (ReferenceEquals(zoom, null) == false)
            {
                StopCoroutine(zoom);
            }
            zoom = StartCoroutine(ZoomOut());
        }
    }

    IEnumerator ZoomIn()
    {
        Vector3 initial = graphic.localScale;
        float count = 0;
        while (count < zoomDuration)
        {
            graphic.localScale = Vector3.Lerp(initial, Vector3.one * zoomScale, count / zoomDuration);
            count += Time.deltaTime;
            yield return null;
        }

        graphic.localScale = Vector3.one * zoomScale;
    }
    IEnumerator ZoomOut()
    {
        Vector3 initial = graphic.localScale;
        float count = 0;
        while (count < zoomDuration)
        {
            graphic.localScale = Vector3.Lerp(initial, Vector3.one * normalScale, count / zoomDuration);
            count += Time.deltaTime;
            yield return null;
        }

        graphic.localScale = Vector3.one * normalScale;
    }
}
