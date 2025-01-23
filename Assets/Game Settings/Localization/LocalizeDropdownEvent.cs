using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

[RequireComponent(typeof(TMP_Dropdown))]
public class LocalizeDropdownEvent : MonoBehaviour
{
    [SerializeField] private List<LocalizedString> dropdownOptions = new();
    [SerializeField] private List<TMP_Dropdown.OptionData> tmpDropdownOptions = new();

    public TMP_Dropdown TmpDropdown { get; private set; }

    private void Awake()
    {
        TmpDropdown = GetComponent<TMP_Dropdown>();

        LocalizationSettings.SelectedLocaleChanged += UpdateLocalization;

        UpdateLocalization(null);
    }

    private void UpdateLocalization(Locale _)
    {
        if (dropdownOptions.Count == 0)
        {
            return;
        }

        tmpDropdownOptions.Clear();
        foreach (LocalizedString dropdownOption in dropdownOptions)
        {
            tmpDropdownOptions.Add(new TMP_Dropdown.OptionData(dropdownOption.GetLocalizedString()));
        }

        TmpDropdown.options = tmpDropdownOptions;
    }

    public void AddOptions(List<TMP_Dropdown.OptionData> optionDatas, List<KeyValuePair<string, string>> dropdownLocalizations)
    {
        TmpDropdown.options = optionDatas;

        AddLocalizationValues(dropdownLocalizations);
    }

    public void AddLocalizationValues(List<KeyValuePair<string, string>> dropdownLocalizations)
    {
        dropdownOptions.Clear();
        for (int i = 0; i < dropdownLocalizations.Count; i++)
        {
            dropdownOptions.Add(new LocalizedString(dropdownLocalizations[i].Key, dropdownLocalizations[i].Value));
        }

        UpdateLocalization(null);
    }
}

public abstract class AddLocalizeDropdown
{
    [MenuItem("CONTEXT/TMP_Dropdown/Localize", false, 1)]
    private static void AddLocalizeComponent()
    {
        // add localize dropdown component to selected gameobject
        GameObject selected = Selection.activeGameObject;
        if (selected != null)
        {
            selected.AddComponent<LocalizeDropdownEvent>();
        }
    }
}
