using UnityEngine;
using UnityEngine.Localization.Components;

public class UISearchComponent : MonoBehaviour, ITrieSearchable
{
    [SerializeField] LocalizeStringEvent _localizeStringEvent;

    public string Id => _localizeStringEvent.StringReference.GetLocalizedString().ToLower();

    public void SetState(bool state)
    {
        gameObject.SetActive(state);
    }
}
