using TMPro;
using UnityEngine.Localization;

public class LocalizedDropdownOption : TMP_Dropdown.OptionData
{
    public LocalizedString LocalizedString { get; }

    public LocalizedDropdownOption(string value)
    {
        text = value;
    }

    public LocalizedDropdownOption(string table, string localizationKey)
    {
        LocalizedString = new(table, localizationKey);
        text = LocalizedString.GetLocalizedString();

        LocalizedString.StringChanged += LanguageUpdated;
    }

    ~LocalizedDropdownOption()
    {
        LocalizedString.StringChanged -= LanguageUpdated;
    }

    private void LanguageUpdated(string value)
    {
        text = value;
    }
}
