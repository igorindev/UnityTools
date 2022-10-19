using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class TabInputField : MonoBehaviour
{
    [SerializeField] TMP_InputField loginInputField;    //0
    [SerializeField] TMP_InputField passwordInputField; //1

    [SerializeField] UnityEvent OnSubmit;

    public int selectedField;

    private void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            selectedField++;
            if (selectedField > 1) selectedField = 0;
            SelectInputField();
        }

        if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame)
        {
            OnSubmit?.Invoke();
        }
    }

    void SelectInputField()
    {
        if (selectedField == 0) { loginInputField.Select(); return; }
        else if (selectedField == 1) { passwordInputField.Select(); return; }
    }

    public void SelectUsername() => selectedField = 0;
    public void SelectPassword() => selectedField = 1;
}
