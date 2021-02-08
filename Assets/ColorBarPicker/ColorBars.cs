using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorBars : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] Scrollbar r;
    [SerializeField] Scrollbar g;
    [SerializeField] Scrollbar b;

    [Header("Values")]
    [SerializeField] TextMeshProUGUI rValue;
    [SerializeField] TextMeshProUGUI gValue;
    [SerializeField] TextMeshProUGUI bValue;

    Image displayColor;
    Image interacted;

    public void OpenColorPicker(Image image)
    {
        interacted = image;
        gameObject.SetActive(true);
        ReadColor(interacted.color);
    }

    void ReadColor(Color color)
    {
        r.value = color.r;
        g.value = color.g;
        b.value = color.b;

        rValue.text = color.r.ToString();
        gValue.text = color.g.ToString();
        bValue.text = color.b.ToString();

        displayColor.color = interacted.color;
    }

    public void SetRed(float red)
    {
        SetColor(red, g.value, b.value);
    }
    public void SetGreen(float green)
    {
        SetColor(r.value, green, b.value);
    }
    public void SetBlue(float blue)
    {
        SetColor(r.value, g.value, blue);
    }

    void SetColor(float r, float g, float b)
    {
        Color newColor = new Vector4(r, g, b, 1);
        rValue.text = newColor.r.ToString("F1");
        gValue.text = newColor.g.ToString("F1");
        bValue.text = newColor.b.ToString("F1");
        displayColor.color = newColor;
    }

    public void ApplyFinalColor()
    {
        interacted.color = displayColor.color;
    }
}