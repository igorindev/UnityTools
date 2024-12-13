using System;
using System.Collections.Generic;
using UnityEngine.Pool;

[Serializable]
public class InputBindingsScheme : IDisposable
{
    public string Scheme;
    public List<InputBindingData> Bindings;
    public Dictionary<string, List<InputRebindUIButton>> uiButtonsWithBinding;

    public void Dispose()
    {
        Bindings.Clear();
        uiButtonsWithBinding.Clear();
    }

    public void UpdateUIBindingButton(string oldBindingPath, string newBindingPath, InputRebindUIButton inputRebindUIButton)
    {
        RemoveUIButtonWithBinding(oldBindingPath, inputRebindUIButton);
        AddUIButtonWithBinding(newBindingPath, inputRebindUIButton);
    }

    private void AddUIButtonWithBinding(string bindingPath, InputRebindUIButton inputRebindUIButton)
    {
        if (uiButtonsWithBinding.TryGetValue(bindingPath, out List<InputRebindUIButton> inputButtons))
        {
            inputButtons.Add(inputRebindUIButton);
        }
        else
        {
            List<InputRebindUIButton> inputUIButtonList = ListPool<InputRebindUIButton>.Get();
            inputUIButtonList.Add(inputRebindUIButton);

            uiButtonsWithBinding.Add(bindingPath, inputUIButtonList);
        }

        CheckDuplicate(bindingPath);
    }

    private void RemoveUIButtonWithBinding(string bindingPath, InputRebindUIButton inputRebindUIButton)
    {
        if(!uiButtonsWithBinding.TryGetValue(bindingPath, out List<InputRebindUIButton> list))
        {
            return;
        }

        list.Remove(inputRebindUIButton);

        CheckDuplicate(bindingPath);

        if (list.Count == 0)
        {
            ListPool<InputRebindUIButton>.Release(list);
            uiButtonsWithBinding.Remove(bindingPath);
        }
    }

    private void CheckDuplicate(string oldBingingPath)
    {
        List<InputRebindUIButton> inputRebindViews = uiButtonsWithBinding[oldBingingPath];
        foreach (InputRebindUIButton view in inputRebindViews)
        {
            view.SetAsDuplicated(inputRebindViews.Count > 1);
        }
    }
}
