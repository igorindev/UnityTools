using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using static UnityEngine.InputSystem.InputActionRebindingExtensions;

public class InputRebindView : MonoBehaviour
{
    [SerializeField] Transform _bindingsButtonsHolder;
    [SerializeField] InputRebindUIButton _bindingButtonPrefab;
    [SerializeField] TextMeshProUGUI _headerPrefab;

    [Header("Warnings")]
    [SerializeField] TextMeshProUGUI _cancelWarning;
    [SerializeField] TextMeshProUGUI _duplicateWarning;

    private readonly Dictionary<string, InputBindingsScheme> _schemas = new();
    private InputRebindUIButton _currentRebindingView;
    private InputRebindModal _inputRebindModal;

    private Action<InputBindingData> _updateBinding;
    private string _keyboardCancelInput;

    private void Start()
    {
        _inputRebindModal = GetComponent<InputRebindModal>();

        _inputRebindModal.onInitialize += HandleInitialize;
        _inputRebindModal.OnSchemeUpdated += HandleSchemeUpdated;
        _inputRebindModal.OnBeginNewInputMapSection += CreateBindingMapReference;
        _inputRebindModal.onBegin += HandleRebindBegan;
        _inputRebindModal.OnCreateInputBindingData += CreateBindingReference;
        _inputRebindModal.OnRebindComplete += HandleRebindCompleted;
        _inputRebindModal.OnRebindEnd += HandleRebindEnded;
    }

    private void OnDestroy()
    {
        _inputRebindModal.onInitialize -= HandleInitialize;
        _inputRebindModal.OnBeginNewInputMapSection -= CreateBindingMapReference;
        _inputRebindModal.onBegin -= HandleRebindBegan;
        _inputRebindModal.OnCreateInputBindingData -= CreateBindingReference;
        _inputRebindModal.OnRebindComplete -= HandleRebindCompleted;
        _inputRebindModal.OnRebindEnd -= HandleRebindEnded;
    }

    private void CreateBindingMapReference(InputActionMap inputBindingData)
    {
        TextMeshProUGUI headerView = Instantiate(_headerPrefab, _bindingsButtonsHolder);
        headerView.text = inputBindingData.name;
    }

    private void CreateBindingReference(InputBindingData inputBindingData, Action<InputBindingData> updateBinding)
    {
        _updateBinding = updateBinding;

        InputRebindUIButton rebindView = Instantiate(_bindingButtonPrefab, _bindingsButtonsHolder);
        rebindView.Setup(inputBindingData, HandleStartRebindingAction);

        AddToInputSchema(inputBindingData, inputBindingData.scheme, rebindView);
    }

    private void AddToInputSchema(InputBindingData bindingData, string scheme, InputRebindUIButton inputRebindView)
    {
        InputBindingsScheme inputBindingsScheme = _schemas[scheme];
        inputBindingsScheme.Bindings.Add(bindingData);
        inputBindingsScheme.UpdateUIBindingButton(bindingData.inputEffectivePath, bindingData.inputEffectivePath, inputRebindView);
    }

    private void SetCancelRebindPopupActive(bool value)
    {
        _cancelWarning.text = $"Cancel Rebind {_keyboardCancelInput}";
        _cancelWarning.gameObject.SetActive(value);
    }

    private void SetButtonViewAsRebinding()
    {
        _currentRebindingView.SetAsRebinding();
    }

    private void HandleInitialize(ReadOnlyArray<InputControlScheme> controlSchemes, string cancelInput)
    {
        _keyboardCancelInput = cancelInput;
        _schemas.Clear();

        foreach (InputControlScheme scheme in controlSchemes)
        {
            _schemas.Add(scheme.name, new InputBindingsScheme()
            {
                Scheme = scheme.name,
                Bindings = new List<InputBindingData>(),
                uiButtonsWithBinding = new Dictionary<string, List<InputRebindUIButton>>()
            });
        }
    }

    private void HandleSchemeUpdated(string _)
    {
        foreach (KeyValuePair<string, InputBindingsScheme> scheme in _schemas)
        {
            scheme.Value.Dispose();
        }
    }

    private void HandleStartRebindingAction(InputRebindUIButton inputRebindUIButton)
    {
        _currentRebindingView = inputRebindUIButton;
        _updateBinding?.Invoke(_currentRebindingView.Binding);
    }

    private void HandleRebindBegan(RebindingOperation rebindingOperation, InputBindingData inputBindingData)
    {
        SetButtonViewAsRebinding();
        SetCancelRebindPopupActive(true);
    }

    private void HandleRebindCompleted(string oldBingingPath, InputBindingData updatedInput)
    {
        _schemas[updatedInput.scheme].UpdateUIBindingButton(oldBingingPath, updatedInput.inputEffectivePath, _currentRebindingView);
    }

    private void HandleRebindEnded(bool success, InputBindingData inputBindingData)
    {
        SetCancelRebindPopupActive(false);
        _currentRebindingView.UpdateBindData(inputBindingData);
    }
}
