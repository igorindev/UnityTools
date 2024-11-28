using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InputRebindView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _inputName;
    [SerializeField] TextMeshProUGUI _inputKey;
    [SerializeField] Button _rebindButton;

    private UnityAction<InputRebindView> _rebindClick;

    public InputBindingData Binding { get; private set; }

    public void Setup(InputBindingData bindData, UnityAction<InputRebindView> rebindClick)
    {
        UpdateBindData(bindData);

        _rebindClick = rebindClick;
        _rebindButton.onClick.AddListener(OnButtonClick);
    }

    public void UpdateBindData(InputBindingData _currentBinding)
    {
        _inputName.text = _currentBinding.inputActionName;
        _inputKey.text = InputToSpriteUtility.GetIconString(_currentBinding.inputAction, _currentBinding.schema);
        Binding = _currentBinding;
    }

    private void OnDestroy()
    {
        _rebindButton.onClick.RemoveListener(OnButtonClick);
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
