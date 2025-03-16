using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SliderField : MonoBehaviour
{
    [SerializeField] Slider _slider;
    [SerializeField] TMP_InputField _inputField;
    [SerializeField] Button _leftButton;
    [SerializeField] Button _rightButton;

    [Header("Customization")]
    [SerializeField] bool _wholeNumber;
    [SerializeField, Min(0)] float _buttonIncrement = 1;

    [Header("Events")]
    public UnityEvent<string> onValueChangedString;
    public UnityEvent<float> onValueChangedFloat;

    private string _oldInputValue;

    private void Start()
    {
        _inputField.onDeselect.AddListener(RefreshValue);
        _inputField.onSubmit.AddListener(RefreshValue);

        _inputField.onValueChanged.AddListener(HandleInputFieldUpdate);
        _slider.onValueChanged.AddListener(HandleSliderUpdate);

        _leftButton.onClick.AddListener(HandleClickRemove);
        _rightButton.onClick.AddListener(HandleClickAdd);

        _slider.wholeNumbers = _wholeNumber;
        _inputField.contentType = _wholeNumber ? TMP_InputField.ContentType.IntegerNumber : TMP_InputField.ContentType.DecimalNumber;

        Setup(_slider.value, _slider.minValue, _slider.maxValue, _wholeNumber, _buttonIncrement);
    }


    private void OnDestroy()
    {
        _inputField.onDeselect.RemoveListener(RefreshValue);
        _inputField.onSubmit.RemoveListener(RefreshValue);

        _inputField.onValueChanged.RemoveListener(HandleInputFieldUpdate);
        _slider.onValueChanged.RemoveListener(HandleSliderUpdate);

        _leftButton.onClick.RemoveListener(HandleClickAdd);
        _rightButton.onClick.RemoveListener(HandleClickRemove);
    }

    public void Setup(float initialValue, float min, float max, bool wholeNumber, float buttonIncrementAmount = 1f)
    {
        _slider.minValue = min;
        _slider.maxValue = max;
        _wholeNumber = wholeNumber;
        _buttonIncrement = buttonIncrementAmount;

        _slider.wholeNumbers = _wholeNumber;
        _inputField.contentType = _wholeNumber ? TMP_InputField.ContentType.IntegerNumber : TMP_InputField.ContentType.DecimalNumber;

        SetValue(initialValue);
    }

    public void SetValue(float value)
    {
        _slider.value = value;
        _inputField.text = value.ToString("0.##", CultureInfo.InvariantCulture);
    }

    public void ForceNotify()
    {
        _slider.onValueChanged.Invoke(_slider.value);
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

    private void HandleClickAdd()
    {
        _slider.value += _buttonIncrement;
    }

    private void HandleClickRemove()
    {
        _slider.value -= _buttonIncrement;
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
