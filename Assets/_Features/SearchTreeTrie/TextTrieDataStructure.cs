using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextTrieDataStructure : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    [SerializeField] TextMeshProUGUI elementPrefab;
    [SerializeField] Transform parent;

    private Trie<TextMeshProUGUI> trie;

    List<TrieNode<TextMeshProUGUI>> elementsList = new();

    [System.Serializable]
    public class Movies
    {
        public List<string> movies;
    }

    void Start()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("movies");

        if (jsonFile != null)
        {
            Movies moviesData = JsonUtility.FromJson<Movies>(jsonFile.text);
            List<string> elements = moviesData.movies;

            trie = new();

            foreach (string name in elements)
            {
                TextMeshProUGUI instance = Instantiate(elementPrefab, parent);
                instance.text = name;
                instance.gameObject.SetActive(false);
                trie.Insert(name.ToLower(), instance);
            }

            inputField.onValueChanged.AddListener(OnInputFieldChanged);
        }
        else
        {
            Debug.Log("JSON IS NULL");
        }
    }

    void OnInputFieldChanged(string input)
    {
        foreach (TrieNode<TextMeshProUGUI> item in elementsList)
        {
            item.Element.gameObject.SetActive(false);
        }

        if (string.IsNullOrEmpty(input))
        {
            return;
        }

        elementsList = trie.Get(input);

        foreach (TrieNode<TextMeshProUGUI> item in elementsList)
        {
            item.Element.gameObject.SetActive(true);
        }
    }
}
