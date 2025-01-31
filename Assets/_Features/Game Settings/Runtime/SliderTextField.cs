using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SliderTextField : MonoBehaviour
{
    [SerializeField] Slider _slider;
    [SerializeField] TMP_InputField _inputField;
    [SerializeField] bool _wholeNumber;
    [Space]
    public UnityEvent<string> onValueChangedString;
    public UnityEvent<float> onValueChangedFloat;

    private string _oldInputValue;

    private void Start()
    {
        _inputField.onDeselect.AddListener(RefreshValue);
        _inputField.onSubmit.AddListener(RefreshValue);

        _inputField.onValueChanged.AddListener(HandleInputFieldUpdate);
        _slider.onValueChanged.AddListener(HandleSliderUpdate);

        _slider.wholeNumbers = _wholeNumber;
        _inputField.contentType = _wholeNumber ? TMP_InputField.ContentType.IntegerNumber : TMP_InputField.ContentType.DecimalNumber;

        SetValue(_slider.value);
    }


    private void OnDestroy()
    {
        _inputField.onDeselect.RemoveListener(RefreshValue);
        _inputField.onSubmit.RemoveListener(RefreshValue);

        _inputField.onValueChanged.RemoveListener(HandleInputFieldUpdate);
        _slider.onValueChanged.RemoveListener(HandleSliderUpdate);
    }

    public void SetValue(float value)
    {
        _slider.SetValueWithoutNotify(value);
        _inputField.SetTextWithoutNotify(value.ToString("0.##", CultureInfo.InvariantCulture));
    }

    private void HandleInputFieldUpdate(string value)
    {
        if (!float.TryParse(value, out float result))
        {
            _inputField.text = _oldInputValue;
            return;
        }

        if (result > _slider.maxValue)
        {
            result = _slider.maxValue;
        }

        if (result < _slider.minValue)
        {
            result = _slider.minValue;
        }

        _oldInputValue = result.ToString();
        _slider.SetValueWithoutNotify(result);
        UpdateValue();
    }

    private void HandleSliderUpdate(float value)
    {
        _inputField.SetTextWithoutNotify(value.ToString("0.##", CultureInfo.InvariantCulture));
        _oldInputValue = _inputField.text;
        UpdateValue();
    }

    private void RefreshValue(string value)
    {
        _inputField.text = _oldInputValue;
    }

    private void UpdateValue()
    {
        onValueChangedString?.Invoke(_inputField.text);
        onValueChangedFloat?.Invoke(_slider.value);
    }
}
