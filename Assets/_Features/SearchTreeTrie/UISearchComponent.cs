using UnityEngine;
using UnityEngine.Localization.Components;

public class UISearchComponent : MonoBehaviour
{
    [SerializeField] LocalizeStringEvent _localizeStringEvent;

    public string Id => _localizeStringEvent.StringReference.GetLocalizedString().ToLower();
}
