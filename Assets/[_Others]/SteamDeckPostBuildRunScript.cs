using System.Collections;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Networking;

public static class SteamDeckPostBuildRunScript
{
    static PostUploadToSteamDeckCoroutine postCoroutine;

    [PostProcessBuild(1000)]
    public static void UploadToSteamDeck(BuildTarget target, string pathToBuiltProject)
    {
        if (target == BuildTarget.StandaloneWindows64)
        {
            postCoroutine = new PostUploadToSteamDeckCoroutine();
            postCoroutine.Start();
        }
    }

    [System.Serializable]
    public class SteamDeckBuildJson
    {
        public string type = "build";
        public string status = "success";
        public string name = "RXC_Release"; // Enter "Name" from Steam Devkit Management Tool here
    }

    public class PostUploadToSteamDeckCoroutine
    {
        public void Start()
        {
            EditorCoroutineUtility.StartCoroutine(PostUploadToSteamDeck(), this);
        }
    }

    static IEnumerator PostUploadToSteamDeck()
    {
        string json = JsonUtility.ToJson(new SteamDeckBuildJson());
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest webRequest = UnityWebRequest.PostWwwForm("http://127.0.0.1:32018/post_event", ""))
        {
            using (UploadHandlerRaw ul = new UploadHandlerRaw(bytes))
            {
                webRequest.uploadHandler = ul;
                webRequest.SetRequestHeader("Content-Type", "application/json");
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log("Steam Deck upload error: " + webRequest.error);
                }
                else
                {
                    Debug.Log("Uploaded to Steam Deck");
                }
            }
        }
    }
}
