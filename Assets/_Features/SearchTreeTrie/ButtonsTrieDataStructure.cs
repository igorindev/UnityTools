using System;
using System.Collections.Generic;
using Gma.DataStructures.StringSearch;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using static TextTrieDataStructure;

public class ButtonsTrieDataStructure : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    [SerializeField] Transform parent;
    [SerializeField] bool updateWithLocalization = true;

    private List<UISearchComponent> allButtons = new();

    private UkkonenTrie<char, UISearchComponent> searchTree;

    private IEnumerable<UISearchComponent> elementsList;

    private List<UISearchComponent> allSearchObjects = new();

    private List<string> elements = new();

    private string searchInput = string.Empty;

    private void Awake()
    {
        LocalizationSettings.SelectedLocaleChanged += m_SelectedLocaleChanged;
    }

    private void Start()
    {
        inputField.onValueChanged.AddListener(OnInputFieldChanged);

        parent.GetComponentsInChildren(allButtons);

        allSearchObjects = new List<UISearchComponent>(allButtons.Count);
        LoadJson();
        for (int i = 0; i < allButtons.Count; i++)
        {
            UISearchComponent searchComponent = allButtons[i];
            allSearchObjects.Add(searchComponent);
        }

        Initialize();
    }

    private void Initialize()
    {
        Debug.Log("INITIALIZE SEARCH TREE");
        searchTree = new();

        foreach (UISearchComponent wordAndLine in allSearchObjects)
        {
            searchTree.Add(wordAndLine.Id.AsMemory(), wordAndLine);

            Debug.Log(wordAndLine.Id);
        }
    }

    void LoadJson()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("movies");

        if (jsonFile != null)
        {
            Movies moviesData = JsonUtility.FromJson<Movies>(jsonFile.text);
            foreach (string name in moviesData.movies)
            {
                elements.Add(name);
            }
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
            foreach (UISearchComponent item in allSearchObjects)
            {
                item.gameObject.SetActive(true);
            }

            return;
        }

        elementsList = searchTree.Retrieve(input.AsSpan());

        foreach (UISearchComponent item in allSearchObjects)
        {
            item.gameObject.SetActive(false);
        }

        foreach (UISearchComponent item in elementsList)
        {
            item.gameObject.SetActive(true);
        }
    }

    //internal void RegisterToSearchEngine(UISearchComponent searchComponent, string old, string id)
    //{
    //    searchTree.Add(1"hello");
    //
    //    Debug.Log($"REGISTER TO SEARCH TREE with id {id}");
    //}
}
