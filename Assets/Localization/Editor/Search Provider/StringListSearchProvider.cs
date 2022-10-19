using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class StringListSearchProvider : ScriptableObject, ISearchWindowProvider
{
    string[] listItems;
    bool sortList;
    Action<string> onSetIndexCallback;

    internal StringListSearchProvider Initialize(string[] items, Action<string> callback, bool sort = false)
    {
        listItems = items;
        onSetIndexCallback = callback;
        sortList = sort;

        return this;
    }
    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        List<SearchTreeEntry> searchTreeEntries = new List<SearchTreeEntry>();
        searchTreeEntries.Add(new SearchTreeGroupEntry(new GUIContent("List"), 0));

        if (sortList)
        {
            List<string> sortedListItems = listItems.ToList();
            sortedListItems.Sort((a, b) =>
            {
                string[] splits1 = a.Split('/');
                string[] splits2 = b.Split('/');
                for (int i = 0; i < splits1.Length; i++)
                {
                    if (i >= splits2.Length)
                    {
                        return 1;
                    }

                    int value = splits1[i].CompareTo(splits2[i]);
                    if (value != 0)
                    {
                    // Make sure that leaves go before nodes
                    if (splits1.Length != splits2.Length && (i == splits1.Length - 1 || i == splits2.Length - 1))
                            return splits1.Length < splits2.Length ? 1 : -1;

                        return value;
                    }
                }
                return 0;
            });
            listItems = sortedListItems.ToArray();
        }

        List<string> groups = new List<string>();
        foreach (string item in listItems)
        {
            string[] entryTitle = item.Split('/');

            string groupName = "";
            for (int i = 0; i < entryTitle.Length - 1; i++)
            {
                groupName += entryTitle[i];
                if (!groups.Contains(groupName))
                {
                    searchTreeEntries.Add(new SearchTreeGroupEntry(new GUIContent(entryTitle[i]), i + 1));
                    groups.Add(groupName);
                }
                groupName += "/";
            }
            SearchTreeEntry entry = new SearchTreeEntry(new GUIContent(entryTitle.Last()));
            entry.level = entryTitle.Length;
            entry.userData = entryTitle.Last();
            searchTreeEntries.Add(entry);
        }

        return searchTreeEntries;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        onSetIndexCallback?.Invoke((string)SearchTreeEntry.userData);
        return true;
    }
}