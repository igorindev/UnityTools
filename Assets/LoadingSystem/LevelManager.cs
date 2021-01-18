using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    private string actualWorld;
    bool loading;

    [Header("LevelSprites")]
    public Sprite[] loadingSprites;

    [Header("Load Screen")]
    public Button button;
    public CanvasGroup loadScreen;
    //public Image backgroundImage;
    //public Text LevelName;
    public Slider loadFill;
    const float FADESPEED = 0.4f;

    [Header("Tips")]
    //public Text tipstext;

    [TextArea]
    //public string[] tips;

    AsyncOperation scene;

    public string ActualWorld { get => actualWorld; set => actualWorld = value; }
    public bool Loading { get => loading; set => loading = value; }

    void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Ja existe um Level manager");
            return;
        }
        instance = this;
    }

    [ContextMenu("Load")]
    public void Load()
    {
        Loading = true;
        ActualWorld = "S2";
        StartCoroutine(LoadScreenFadeIn());
    }

    public void LoadScene(string level)
    {
        Loading = true;
        ActualWorld = level;
        StartCoroutine(LoadScreenFadeIn());
    }
    public void LoadMenuScene(string level = "MainMenu")
    {
        Loading = true;
        ActualWorld = level;
        StartCoroutine(LoadScreenFadeIn());
    }

    public void Activate()
    {
        scene.allowSceneActivation = true;
        StartCoroutine(LoadScreenFadeOut());
    }

    IEnumerator LoadAsyncOperation(string level)
    {
        scene = SceneManager.LoadSceneAsync(level);
        scene.allowSceneActivation = false;
        while (scene.progress < 0.9f)
        {
            loadFill.value = scene.progress;
            yield return new WaitForEndOfFrame();
        }
        loadFill.value = scene.progress;
        button.gameObject.SetActive(true);
    }

    IEnumerator LoadScreenFadeIn()
    {
        loadFill.value = 0;
       
        float counter = 0;
        loadScreen.gameObject.SetActive(true);
        while (counter < FADESPEED)
        {
            counter += Time.deltaTime;
            loadScreen.alpha = Mathf.Lerp(0, 1, counter / FADESPEED);

            yield return null;
        }
        loadScreen.alpha = 1;

        StartCoroutine(LoadAsyncOperation(ActualWorld));
    }
    IEnumerator LoadScreenFadeOut()
    {
        float counter = 0;
        while (counter < FADESPEED)
        {
            counter += Time.deltaTime;
            loadScreen.alpha = Mathf.Lerp(1, 0, counter / FADESPEED);

            yield return null;
        }
        loadScreen.alpha = 0;
        loadScreen.gameObject.SetActive(false);
        Loading = false;
    }

    void SetLoadScreenInfo()
    {
        //LevelName.text = actualWorld.ToUpper();
        //backgroundImage.sprite = SelectLevelSprite();
    }

    Sprite SelectLevelSprite()
    {
        return loadingSprites[Random.Range(0, loadingSprites.Length)];
    }
}