using UnityEngine;
using UnityEngine.UI;

public class ParamCube : MonoBehaviour
{
    public int band;

    public float startScale;
    public float scaleMultiplier;

    public bool useBuffer;
    Material material;
    public Color hightlightColor;

    public bool isUI;

    private void Start()
    {
        if (isUI)
        {
            material = GetComponent<Image>().material;
        }
        else
        {
            material = GetComponent<MeshRenderer>().materials[0];
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (useBuffer)
        {
            Vector3 value = new Vector3(transform.localScale.x, (AudioVisualizer.audioBandBuffer[band] * scaleMultiplier) + startScale, transform.localScale.z);
            if (float.IsNaN(value.y) == false)
            {
                transform.localScale = value;
            }
            else
            {
                transform.localScale = Vector3.one;
                return;
            }
            Color color = new Color(hightlightColor.r * AudioVisualizer.audioBandBuffer[band], hightlightColor.g * AudioVisualizer.audioBandBuffer[band], hightlightColor.b * AudioVisualizer.audioBandBuffer[band], 1);
            material.SetColor("_EmissionColor", color);
        }
        if (!useBuffer)
        {
            Vector3 value = new Vector3(transform.localScale.x, (AudioVisualizer.audioBand[band] * scaleMultiplier) + startScale, transform.localScale.z);
            if (float.IsNaN(value.y) == false)
            {
                transform.localScale = value;
            }
            else
            {
                transform.localScale = Vector3.one;
                return;
            }
            Color color = new Color(hightlightColor.r * AudioVisualizer.audioBand[band], hightlightColor.g * AudioVisualizer.audioBand[band], hightlightColor.b * AudioVisualizer.audioBand[band], 1); 
            material.SetColor("_EmissionColor", color);
        }
    }
}
