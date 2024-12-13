using System.Collections.Generic;
using System.Linq;
using Gma.DataStructures.StringSearch;
using TreeEditor;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;

public class Trie<T>
{
    //The starting point of the Trie.
    private TrieNode<T> root;

    public Trie()
    {
        root = new TrieNode<T>(default);
    }

    // Insertion
    public void Insert(string word, T element)
    {
        TrieNode<T> node = root;
        //Starts from the root node and iterates over each character in the word.
        foreach (char c in word)
        {
            if (!node.Children.ContainsKey(c))
            {
                //If a character does not have a corresponding child node, it creates one.
                node.Children[c] = new TrieNode<T>(element);
            }
            node = node.Children[c];
        }
        //Marks the last node as the end of the word 
        node.IsEndOfTheWord = true;
    }

    public bool Remove(string word)
    {
        TrieNode<T> current = root;
        foreach (char c in word)
        {
            if (!current.Children.ContainsKey(c))
            {
                return false; // Word not found
            }
            current = current.Children[c];
        }

        // If the node is a leaf node, delete it
        if (current.IsEndOfTheWord)
        {
            current = null;
            return true;
        }

        // Mark the node as deleted if it's an internal node
        current.IsEndOfTheWord = false;
        return true;
    }

    public TrieNode<T> TrieDelete(string key)
    {
        return TrieDelete(root, key);
    }

    public TrieNode<T> TrieDelete(TrieNode<T> node, string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            // Empty key, check for end of word marker
            if (node != null && node.IsEndOfTheWord)
            {
                node.IsEndOfTheWord = false;
            }

            // Check if all children are null, then remove the node
            if (node != null && node.Children.Count == 0)
            {
                return null;
            }
            return node;
        }

        char firstChar = key[0];
        node.Children[firstChar] = TrieDelete(node.Children[firstChar], key.Substring(1));

        // Check if the child node became empty after deletion
        if (node.Children[firstChar] == null)
        {
            // Remove empty child nodes
            node.Children.Remove(firstChar);
        }

        return node;
    }

    // Searching
    //Finds the node corresponding to a given prefix.
    private TrieNode<T> SearchNode(string prefix)
    {
        TrieNode<T> node = root;
        //Starts from the root node and iterates over each character in the prefix.
        foreach (char c in prefix)
        {
            if (!node.Children.ContainsKey(c))
            {
                //f a character does not have a corresponding child node, it returns null.
                return null;
            }
            // it moves to the child node and continues.
            node = node.Children[c];
        }
        return node;
    }

    // Collect all words
    // Recursively collects all words starting from a given node.
    private void CollectWords(TrieNode<T> node, string prefix, List<string> results)
    {
        //If the node is null, it returns.
        if (node == null) return;
        if (node.IsEndOfTheWord)
        {
            //If the node marks the end of a word, it adds the word (prefix) to the results.
            results.Add(prefix);
            Debug.Log("Found word: " + prefix);
        }

        foreach (var child in node.Children)
        {
            //Recursively collects words from all child nodes, appending each character to the prefix.
            CollectWords(child.Value, prefix + child.Key, results);
        }
    }

    private void CollectNodes(TrieNode<T> node, string prefix, List<TrieNode<T>> results)
    {
        //If the node is null, it returns.
        if (node == null) return;
        if (node.IsEndOfTheWord)
        {
            //If the node marks the end of a word, it adds the word (prefix) to the results.
            results.Add(node);
        }

        foreach (KeyValuePair<char, TrieNode<T>> child in node.Children)
        {
            //Recursively collects words from all child nodes, appending each character to the prefix.
            CollectNodes(child.Value, prefix + child.Key, results);
        }
    }

    /// <summary>
    /// Returns all words in the Trie that start with the given prefix.
    /// </summary>
    /// <param name="prefix"></param>
    /// <returns></returns>
    public List<string> Autocomplete(string prefix)
    {
        //Uses SearchNode to find the node corresponding to the prefix.
        TrieNode<T> node = SearchNode(prefix);
        if (node == null)
        {
            //If the node is null, it logs a message and returns an empty list.
            Debug.Log("No node found for prefix: " + prefix);
            return new List<string>();
        }
        //Otherwise, it collects all words starting from that node and returns them.
        List<string> results = new List<string>();
        CollectWords(node, prefix, results);
        return results;
    }

    /// <summary>
    /// Get nodes with the prefix
    /// </summary>
    /// <param name="prefix"></param>
    /// <returns></returns>
    public List<TrieNode<T>> Get(string prefix)
    {
        //Uses SearchNode to find the node corresponding to the prefix.
        TrieNode<T> node = SearchNode(prefix);
        if (node == null)
        {
            //If the node is null, it logs a message and returns an empty list.
            Debug.Log("No node found for prefix: " + prefix);
            return new List<TrieNode<T>>();
        }
        //Otherwise, it collects all words starting from that node and returns them.
        List<TrieNode<T>> results = new List<TrieNode<T>>();
        CollectNodes(node, prefix, results);
        return results;
    }
}
