using System.Collections.Generic;
using TMPro;

public class DependentDropdownOption : DependentOption<TMP_Dropdown, List<int>>
{
    private void Awake()
    {
        _dependsOn.onValueChanged.AddListener(OnDropdownChange);
    }

    private void OnDestroy()
    {
        _dependsOn.onValueChanged.RemoveListener(OnDropdownChange);
    }

    private void OnDropdownChange(int value)
    {
        gameObject.SetActive(_desiredState.Contains(value));

        Rebuild();
    }
}
