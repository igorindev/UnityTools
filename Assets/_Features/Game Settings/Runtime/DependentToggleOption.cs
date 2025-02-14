using UnityEngine;
using UnityEngine.UI;

public class DependentToggleOption : DependentOption<Toggle, bool>
{
    private void Awake()
    {
        _dependsOn.onValueChanged.AddListener(OnToogleChange);
    }

    private void OnDestroy()
    {
        _dependsOn.onValueChanged.RemoveListener(OnToogleChange);
    }

    private void OnToogleChange(bool value)
    {
        gameObject.SetActive(value == _desiredState);

        Rebuild();
    }
}
