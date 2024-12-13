using System.Collections.Generic;

public class TrieNode<T>
{
    public Dictionary<char, TrieNode<T>> Children { get; private set; }
    public bool IsEndOfTheWord { get; set; }
    public T Element { get; set; }

    public TrieNode(T element)
    {
        Children = new Dictionary<char, TrieNode<T>>();
        IsEndOfTheWord = false;
        Element = element;
    }
}
