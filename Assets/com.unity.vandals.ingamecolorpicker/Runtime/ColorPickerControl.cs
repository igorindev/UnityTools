using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ColorPickerControl : MonoBehaviour
{
    [SerializeField] RawImage hueImage;
    [SerializeField] RawImage satValImage;
    [SerializeField] RawImage outputImage;

    [SerializeField] Slider hueSlider;
    [SerializeField] TMP_InputField hexInputField;

    [SerializeField] UnityEvent<Color> OnUpdateColor;

    float currentHue = 1f;
    float currentSat = 0.8f;
    float currentVal = 0.8f;

    Texture2D hueTexture; 
    Texture2D svTexture; 
    Texture2D outputTexture;

    void Start()
    {
        CreateHueImage();

        CreateSVImage();

        CreateOutputImage();

        hueSlider.onValueChanged.AddListener(UpdateSVImage);
        hueSlider.value = currentHue;

        UpdateSVImage(currentHue);
    }

    public void Setup(Color initialColor, UnityAction<Color> onColorChange)
    {
        Color.RGBToHSV(initialColor, out currentHue, out currentSat, out currentVal);
        OnUpdateColor.AddListener(onColorChange);
    }

    public void Dispose(UnityAction<Color> onColorChange)
    {
        OnUpdateColor.RemoveListener(onColorChange);
    }

    public void ConfirmColor()
    {
        Color currentColor = Color.HSVToRGB(currentHue, currentSat, currentVal);
        OnUpdateColor?.Invoke(currentColor);
    }

    public void SetSV(float S, float V)
    {
        currentSat = S;
        currentVal = V;

        UpdateOutputImage();
    }

    public void UpdateSVImage(float value)
    {
        currentHue = value;

        for (int y = 0; y < svTexture.height; y++)
            for (int x = 0; x < svTexture.width; x++)
                svTexture.SetPixel(x, y, Color.HSVToRGB(currentHue, (float)x / svTexture.width, (float)y / svTexture.height));

        svTexture.Apply();

        UpdateOutputImage();
    }

    public void OnTextInput()
    {
        if (hexInputField.text.Length < 5)
            return;

        if (ColorUtility.TryParseHtmlString("#" + hexInputField.text, out Color newCol))
            Color.RGBToHSV(newCol, out currentHue, out currentSat, out currentVal);

        hueSlider.value = currentHue;

        hexInputField.text = "";

        UpdateOutputImage();
    }

    void CreateHueImage()
    {
        hueTexture = new Texture2D(1, 16);
        hueTexture.wrapMode = TextureWrapMode.Clamp;
        hueTexture.name = "HueTexture";

        for (int i = 0; i < hueTexture.height; i++)
        {
            hueTexture.SetPixel(0, i, Color.HSVToRGB((float)i / hueTexture.height, 1, 0.8f));
        }

        hueTexture.Apply();

        hueImage.texture = hueTexture;
    }

    void CreateSVImage()
    {
        svTexture = new Texture2D(16, 16);
        svTexture.wrapMode = TextureWrapMode.Clamp;
        svTexture.name = "SatValTexture";

        for (int y = 0; y < svTexture.height; y++)
        {
            for (int x = 0; x < svTexture.width; x++)
            {
                svTexture.SetPixel(x, y, Color.HSVToRGB(currentHue, x / svTexture.width, y / svTexture.height));
            }
        }

        svTexture.Apply();

        satValImage.texture = svTexture;
    }

    void CreateOutputImage()
    {
        outputTexture = new Texture2D(1, 16);
        outputTexture.wrapMode = TextureWrapMode.Clamp;
        outputTexture.name = "OutputTexture";

        Color currentColour = Color.HSVToRGB(currentHue, currentSat, currentVal);

        for (int i = 0; i < outputTexture.height; i++)
        {
            outputTexture.SetPixel(0, i, currentColour);
        }

        outputTexture.Apply();

        outputImage.texture = outputTexture;
    }

    void UpdateOutputImage()
    {
        Color currentColor = Color.HSVToRGB(currentHue, currentSat, currentVal);

        for (int i = 0; i < outputTexture.height; i++)
        {
            outputTexture.SetPixel(0, i, currentColor);
        }

        outputTexture.Apply();

        hexInputField.text = ColorUtility.ToHtmlStringRGB(currentColor);

        OnUpdateColor?.Invoke(currentColor);
    }
}
