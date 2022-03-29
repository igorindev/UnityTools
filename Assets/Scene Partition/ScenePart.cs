using UnityEngine;
using UnityEngine.SceneManagement;

public enum LOADSTREAMCHECKMETHOD
{
    Distance,
    Trigger
}

public enum LOADSTATE
{
    Loading,
    Loaded,
    Unloading,
    Unloaded,
}

public class ScenePart : MonoBehaviour
{
    [SerializeField] LOADSTREAMCHECKMETHOD checkMethod;
    [SerializeField, Scene] string sceneToLoad;
    [SerializeField] float loadDistance = 300;

    string scenePart;

    bool shouldLoad;

    LOADSTATE loadState;

    void Awake()
    {
        string[] result = sceneToLoad.Split('/');
        scenePart = result[result.Length - 1].Split('.')[0];
    }

    public void UpdateCheck(Vector3 playerPos)
    {
        //Checking which method to use
        if (checkMethod == LOADSTREAMCHECKMETHOD.Distance)
        {
            DistanceCheck(playerPos);
        }
        else if (checkMethod == LOADSTREAMCHECKMETHOD.Trigger)
        {
            TriggerCheck();
        }
    }

    void DistanceCheck(Vector3 playerPos)
    {
        //Checking if the player is within the range
        if (Vector3.Distance(playerPos, transform.position) < loadDistance)
        {
            LoadScene();
        }
        else
        {
            UnloadScene();
        }
    }
    void TriggerCheck()
    {
        //shouldLoad is set from the Trigger methods
        if (shouldLoad)
        {
            LoadScene();
        }
        else
        {
            UnloadScene();
        }
    }
    void LoadScene()
    {
        if (loadState == LOADSTATE.Unloaded)
        {
            //We set it to true to avoid loading the scene twice
            loadState = LOADSTATE.Loading;
            SceneManager.LoadSceneAsync(scenePart, LoadSceneMode.Additive).completed += SetAsLoaded;
        }
    }
    void UnloadScene()
    {
        if (loadState == LOADSTATE.Loaded)
        {
            SceneManager.UnloadSceneAsync(scenePart).completed += SetAsUnloaded;
            loadState = LOADSTATE.Unloading;
        }
    }

    public void SetAsLoaded(AsyncOperation asyncOperation)
    {
        loadState = LOADSTATE.Loaded;
    }
    public void SetAsUnloaded(AsyncOperation asyncOperation)
    {
        loadState = LOADSTATE.Unloaded;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shouldLoad = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shouldLoad = false;
        }
    }
}