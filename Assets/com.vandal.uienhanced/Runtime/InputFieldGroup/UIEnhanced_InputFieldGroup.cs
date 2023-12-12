using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIEnhanced_InputFieldGroup : MonoBehaviour
{
    [SerializeField] List<UIEnhanced_InputField> inputFieldsOrder;
    [SerializeField] bool loop;
    [SerializeField] bool trySetAsFirstSelected;

    int selectedField;
    bool isSelected;

    private void Awake()
    {
        foreach (UIEnhanced_InputField item in inputFieldsOrder)
        {
            item.SetupGroup(this);
        }
    }

    private void Start()
    {
        if (trySetAsFirstSelected)
        {
            inputFieldsOrder[selectedField].ActivateInputField();
        }
    }

    private void Update()
    {
        if (isSelected == false) return;

#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            NextInputField();
            SelectInputField();
        }

#elif ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            NextInputField();
            SelectInputField();
        }
#endif
    }

    public void NextInputField()
    {
        if (loop)
        {
            selectedField += 1;
            selectedField %= inputFieldsOrder.Count;
        }
        else
        {
            selectedField += 1;

            if (selectedField > inputFieldsOrder.Count - 1)
            {
                selectedField = inputFieldsOrder.Count - 1;
            }
        }
    }

    public void SelectInputField()
    {
        inputFieldsOrder[selectedField].ActivateInputField();
    }

    internal void SelectInputField(UIEnhanced_InputField uIEnhanced_InputField)
    {
        selectedField = inputFieldsOrder.IndexOf(uIEnhanced_InputField);

        isSelected = true;
    }

    internal void SetFieldDisabled(UIEnhanced_InputField uIEnhanced_InputField)
    {
        isSelected = false;
    }

    internal void AddToGroup(UIEnhanced_InputField uIEnhanced_InputField, int groupIndex)
    {
        inputFieldsOrder.Insert(groupIndex, uIEnhanced_InputField);
    }

    internal void RemoveFromGroup(UIEnhanced_InputField uIEnhanced_InputField)
    {
        inputFieldsOrder.Remove(uIEnhanced_InputField);
    }
}
