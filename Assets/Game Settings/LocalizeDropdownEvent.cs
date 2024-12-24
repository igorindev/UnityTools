using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

[RequireComponent(typeof(TMP_Dropdown))]
public class LocalizeDropdownEvent : MonoBehaviour
{
    private TMP_Dropdown _dropdown;
    private LocalizedString _localizedString;

    private void Awake()
    {
        _dropdown = GetComponent<TMP_Dropdown>();
        _dropdown.onValueChanged.AddListener(HandleValueChanged);

        LocalizationSettings.SelectedLocaleChanged += HandleLocalization;
    }

    private void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= HandleLocalization;
    }

    private void HandleValueChanged(int value)
    {
        LocalizedDropdownOption active = _dropdown.options[value] as LocalizedDropdownOption;

    }

    private void HandleLocalization(Locale locale)
    {
        _dropdown.captionText.text = _localizedString.GetLocalizedString();
    }

    internal void SetValueWithoutNotify(int input)
    {
        _dropdown.SetValueWithoutNotify(input);
    }

    internal void RefreshShownValue()
    {
        _dropdown.RefreshShownValue();
    }

    internal void AddOptions(List<TMP_Dropdown.OptionData> options)
    {
        _dropdown.AddOptions(options);
    }

    internal void AddOptions(List<string> options)
    {
        _dropdown.AddOptions(options);
    }
}
