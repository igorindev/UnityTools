using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InputRebindUIButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _inputName;
    [SerializeField] TextMeshProUGUI _inputKey;
    [SerializeField] Button _rebindButton;

    private UnityAction<InputRebindUIButton> _rebindClick;

    public InputBindingData Binding { get; private set; }

    public void Setup(InputBindingData bindData, UnityAction<InputRebindUIButton> rebindClick)
    {
        UpdateBindData(bindData);

        _rebindClick = rebindClick;
        _rebindButton.onClick.AddListener(OnButtonClick);
    }

    private void OnDestroy()
    {
        _rebindButton.onClick.RemoveListener(OnButtonClick);
    }

    public void UpdateBindData(InputBindingData _currentBindingData)
    {
        _inputName.text = _currentBindingData.name;
        _inputKey.text = InputToSpriteUtility.GetIconString(_currentBindingData.inputAction, _currentBindingData.bindingIndex);
        Binding = _currentBindingData;
    }

    private void OnButtonClick()
    {
        _rebindClick.Invoke(this);
    }

    public void SetAsDuplicated(bool value)
    {
        _rebindButton.GetComponent<Image>().color = value ? Color.red : Color.white;
    }

    public void SetAsRebinding()
    {
        _inputKey.text = "Waiting Input";
    }
}
