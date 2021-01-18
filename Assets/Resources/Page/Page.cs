using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Page : MonoBehaviour
{
    [Header("Page")]
    [SerializeField] Transform content = null;

    [Space(10)]
    [SerializeField] int maxItensPerPage = 5;
    [SerializeField] int numOfPages = 5;
    [SerializeField] bool backToFirstOnEnable = false;
    [SerializeField] bool autoSizeNumOfPage = false;

    [Space(10)]
    [SerializeField] TextMeshProUGUI pageTextCount = null;

    [Space(10)]
    //[SerializeField] OnLoadSelectButton onLoad = null;

    int currentPage = 1;
    int startPage = 1;
    int result = 0;
    float resto = 0;

    public Transform Content { get => content; set => content = value; }
    public TextMeshProUGUI PageTextCount { get => pageTextCount; set => pageTextCount = value; }

    private void Awake()
    {
        currentPage = startPage;

        for (int i = 0; i < content.childCount; i++)
        {
            content.GetChild(i).gameObject.SetActive(false);
        }
    }
    [ContextMenu("Update")]
    public void OnEnable()
    {
        result = 0;
        resto = 0;
        numOfPages = 10;

        if (backToFirstOnEnable)
        {
            currentPage = startPage;
        }

        UpdateAll();
    }

    /// <summary>
    /// Update the current showing page
    /// </summary>
    void UpdatePage()
    {
        bool getFirst = false;
        if (content.childCount > 0)
        {
            for (int i = 0; i < content.childCount; i++)
            {
                content.GetChild(i).gameObject.SetActive(false);
            }

            for (int i = (maxItensPerPage * currentPage) - maxItensPerPage; i < maxItensPerPage * currentPage; i++)
            {
                if (i < content.childCount)
                {
                    content.GetChild(i).gameObject.SetActive(true);

                    if (getFirst == false)
                    {
                      //  if (onLoad != null)
                      //  {
                      //      int id = i;
                      //      onLoad.LoadButton(content.GetChild(id).gameObject);
                      //  }
                        getFirst = true;
                    }
                }
            }
        }
        
        UpdatePageCount();
    }

    /// <summary>
    /// Update the page Text cout
    /// </summary>
    void UpdatePageCount()
    {
        pageTextCount.text = currentPage + "/" + numOfPages;
    }

    public void MoveLeft()
    {
        if (currentPage > startPage)
        {
            currentPage -= 1;
        }

        CheckSize();

        UpdatePage();
    }

    public void MoveRight()
    {
        if (currentPage < numOfPages)
        {
            currentPage += 1;
        }

        CheckSize();

        UpdatePage();
    }


    void UpdateAll()
    {
        StartCoroutine(WaitToUpdate());
    }

    void CheckSize()
    {
        if (autoSizeNumOfPage && numOfPages > 0)
        {
            result = content.childCount / maxItensPerPage;
            resto = content.childCount % maxItensPerPage;
            if (resto != 0)
            {
                numOfPages = result + 1;
            }
            else
            {
                numOfPages = result;
            }

            if (currentPage > numOfPages)
            {
                currentPage = numOfPages;
            }
        }
    }

    IEnumerator WaitToUpdate()
    {
        yield return null;
        CheckSize();

        UpdatePage();
    }
}
