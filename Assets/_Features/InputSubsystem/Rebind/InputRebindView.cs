using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using static UnityEngine.InputSystem.InputActionRebindingExtensions;

public class InputRebindView : MonoBehaviour
{
    [SerializeField] string _scheme = "Key";
    [SerializeField] Transform _bindingsButtonsHolder;
    [SerializeField] InputRebindUIButton _bindingButtonPrefab;
    [SerializeField] TextMeshProUGUI _headerPrefab;

    [Header("Warnings")]
    [SerializeField] TextMeshProUGUI _cancelWarning;
    [SerializeField] TextMeshProUGUI _duplicateWarning;

    private InputBindingsScheme _schemeReference;
    private InputRebindUIButton _currentRebindingView;
    private InputRebindModal _inputRebindModal;

    private Action<InputBindingData> _updateBinding;
    private string _cancelInput;

    private void Awake()
    {
        _inputRebindModal = GetComponent<InputRebindModal>();

        _inputRebindModal.onInitialize += HandleInitialize;
        _inputRebindModal.OnBeginNewInputMapSection += CreateBindingMapReference;
        _inputRebindModal.onRebindStart += HandleRebindStarted;
        _inputRebindModal.OnCreateInputBindingData += CreateBindingReference;
        _inputRebindModal.OnRebindComplete += HandleRebindCompleted;
        _inputRebindModal.OnRebindCancel += HandleRebindEnded;
    }

    private void OnDestroy()
    {
        _inputRebindModal.onInitialize -= HandleInitialize;
        _inputRebindModal.OnBeginNewInputMapSection -= CreateBindingMapReference;
        _inputRebindModal.OnCreateInputBindingData -= CreateBindingReference;
        _inputRebindModal.onRebindStart -= HandleRebindStarted;
        _inputRebindModal.OnRebindComplete -= HandleRebindCompleted;
        _inputRebindModal.OnRebindCancel -= HandleRebindEnded;
    }


    private void CreateBindingMapReference(InputActionMap inputBindingData)
    {
        TextMeshProUGUI headerView = Instantiate(_headerPrefab, _bindingsButtonsHolder);
        headerView.text = inputBindingData.name;
    }

    private void CreateBindingReference(string bindingScheme, InputBindingData inputBindingData, Action<InputBindingData> updateBinding)
    {
        if (_scheme != bindingScheme)
        {
            return;
        }

        _updateBinding = updateBinding;

        InputRebindUIButton rebindView = Instantiate(_bindingButtonPrefab, _bindingsButtonsHolder);
        rebindView.Setup(inputBindingData, HandleStartRebindingAction);

        AddToInputSchema(inputBindingData, rebindView);
    }

    private void AddToInputSchema(InputBindingData bindingData, InputRebindUIButton inputRebindView)
    {
        _schemeReference.Bindings.Add(bindingData);
        _schemeReference.UpdateUIBindingButton(bindingData.inputEffectivePath, bindingData.inputEffectivePath, inputRebindView);
    }

    private void HandleInitialize(string controlScheme, string cancelInput)
    {
        _cancelInput = cancelInput;

        _schemeReference = new InputBindingsScheme()
        {
            Scheme = controlScheme,
            Bindings = new List<InputBindingData>(),
            uiButtonsWithBinding = new Dictionary<string, List<InputRebindUIButton>>()
        };
    }

    private void HandleStartRebindingAction(InputRebindUIButton inputRebindUIButton)
    {
        _currentRebindingView = inputRebindUIButton;
        _updateBinding?.Invoke(_currentRebindingView.Binding);
    }

    private void HandleRebindStarted(InputBindingData inputBindingData)
    {
        SetButtonViewAsRebinding();
        SetCancelRebindPopupActive(true);
    }

    private void HandleRebindCompleted(string oldBingingPath, InputBindingData updatedInput)
    {
        _schemeReference.UpdateUIBindingButton(oldBingingPath, updatedInput.inputEffectivePath, _currentRebindingView);
    }

    private void HandleRebindEnded(InputBindingData inputBindingData)
    {
        SetCancelRebindPopupActive(false);
        _currentRebindingView.UpdateBindData(inputBindingData);
    }

    private void SetCancelRebindPopupActive(bool value)
    {
        _cancelWarning.text = $"Cancel Rebind {_cancelInput}";
        _cancelWarning.gameObject.SetActive(value);
    }

    private void SetButtonViewAsRebinding()
    {
        _currentRebindingView.SetAsRebinding();
    }
}
