using System;
using System.Collections.Generic;
using Gma.DataStructures.StringSearch;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class UISearchElementController : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    [SerializeField] Transform parent;
    [SerializeField] bool updateWithLocalization = true;

    private UkkonenTrie<char, ITrieSearchable> searchTree;
    private List<ITrieSearchable> allSearchablesComponents = new();
    private List<ITrieSearchable> allSearchObjects = new();
    private string searchInput = string.Empty;

    private void Awake()
    {
        LocalizationSettings.SelectedLocaleChanged += m_SelectedLocaleChanged;
    }

    private void Start()
    {
        inputField.onValueChanged.AddListener(OnInputFieldChanged);

        parent.GetComponentsInChildren(allSearchablesComponents);

        allSearchObjects = new List<ITrieSearchable>(allSearchablesComponents.Count);

        for (int i = 0; i < allSearchablesComponents.Count; i++)
        {
            ITrieSearchable searchComponent = allSearchablesComponents[i];
            allSearchObjects.Add(searchComponent);
        }

        Initialize();
    }

    private void Initialize()
    {
        searchTree = new();

        foreach (ITrieSearchable wordAndLine in allSearchObjects)
        {
            searchTree.Add(wordAndLine.Id.AsMemory(), wordAndLine);
        }
    }

    private void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= m_SelectedLocaleChanged;
        inputField.onValueChanged.RemoveListener(OnInputFieldChanged);
    }

    void RestartSearchEngine()
    {
        Initialize();

        if (!string.IsNullOrEmpty(searchInput))
        {
            inputField.onValueChanged.Invoke(searchInput);
        }
    }

    private void m_SelectedLocaleChanged(Locale locale)
    {
        if (!updateWithLocalization)
        {
            return;
        }

        RestartSearchEngine();
    }

    private void OnInputFieldChanged(string input)
    {
        searchInput = input.ToLower();

        if (string.IsNullOrEmpty(searchInput))
        {
            foreach (ITrieSearchable item in allSearchObjects)
            {
                item.SetState(true);
            }

            return;
        }

        foreach (ITrieSearchable item in allSearchObjects)
        {
            item.SetState(false);
        }

        IEnumerable<ITrieSearchable> foundElements = searchTree.Retrieve(input.AsSpan());

        foreach (ITrieSearchable item in foundElements)
        {
            item.SetState(true);
        }
    }
}
