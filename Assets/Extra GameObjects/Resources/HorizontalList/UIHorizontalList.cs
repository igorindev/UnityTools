using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(HorizontalLayoutGroup))]
public class UIHorizontalList : MonoBehaviour
{
    [Space(12)]
    [SerializeField] TextMeshProUGUI selectedText;

    [Header("Conditions")]
    [SerializeField] bool executeOnStart;
    [SerializeField] bool loop;
    [Space(12)]
    [SerializeField] string[] elements;

    [Space(12)]
    [SerializeField] UnityEvent onValueChange;

    int currentSelected = 0;

    public string[] Elements { get => elements; set => elements = value; }
    public string CurrentElement { get => elements[CurrentSelected]; }
    public int CurrentSelected 
    {
        get => currentSelected; 

        set 
        {
            int temp = currentSelected;
            currentSelected = value; 

            if (temp != currentSelected)
            {
                onValueChange.Invoke();
            }
        } 
    }

    void Start()
    {
        selectedText.text = CurrentElement;

        if (executeOnStart)
        {
            onValueChange.Invoke();
        }
    }

    public void MoveNext()
    {
        if (CurrentSelected >= elements.Length - 1)
        {
            if (loop)
            {
                CurrentSelected = 0;
            }
            else
            {
                CurrentSelected = elements.Length - 1;
            }
        }
        else
        {
            CurrentSelected += 1;
        }

        selectedText.text = CurrentElement;
    }
    public void MoveBack()
    {
        if (CurrentSelected <= 0)
        {
            if (loop)
            {
                CurrentSelected = elements.Length - 1;
            }
            else
            {
                CurrentSelected = 0;
            }
        }
        else
        {
            CurrentSelected -= 1;
        }

        selectedText.text = CurrentElement;
    }

    public void SetElements(string[] _elements)
    {
        elements = _elements;
    }

    public void DebugTest()
    {
        Debug.Log("Test: " + CurrentSelected);
    }
}
