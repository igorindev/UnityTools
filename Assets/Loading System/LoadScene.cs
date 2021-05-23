using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
   [SerializeField] bool loadOnStart;
   [SerializeField] string sceneName;

    // Start is called before the first frame update
    void Start()
    {
        if (loadOnStart)
        {
            LoadSceneName();
        }
    }

    public void LoadSceneName()
    {
        SceneManager.LoadSceneAsync(sceneName);
    }


}
