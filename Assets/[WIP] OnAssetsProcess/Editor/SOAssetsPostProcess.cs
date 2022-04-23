using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(fileName = "AssetsPostProcess", menuName = "ScriptableObjects/AssetsPostProcess", order = 1)]
public class SOAssetsPostProcess : ScriptableObject
{
    public static Action<string> onReimportAssetAction;
    public static Action<string> onDeleteAssetAction;
    public static Action<string, string> onMoveAssetAction;

    [SerializeField] UnityEvent<string> onReimportAsset;
    [SerializeField] UnityEvent<string> onDeleteAsset;
    [SerializeField] UnityEvent<string, string> onMoveAsset;
 
    void OnValidate()
    {
        for (int i = 0; i < onReimportAsset.GetPersistentEventCount(); i++)
        {
            onReimportAsset.SetPersistentListenerState(i, UnityEventCallState.EditorAndRuntime);
        }
        for (int i = 0; i < onDeleteAsset.GetPersistentEventCount(); i++)
        {
            onDeleteAsset.SetPersistentListenerState(i, UnityEventCallState.EditorAndRuntime);
        }
        for (int i = 0; i < onMoveAsset.GetPersistentEventCount(); i++)
        {
            onMoveAsset.SetPersistentListenerState(i, UnityEventCallState.EditorAndRuntime);
        }
    }

    void OnEnable()
    {
        SOAssetsPostProcess.onReimportAssetAction -= OnReimportAsset;
        SOAssetsPostProcess.onDeleteAssetAction -= OnDeleteAsset;
        SOAssetsPostProcess.onMoveAssetAction -= OnMoveAsset;

        SOAssetsPostProcess.onReimportAssetAction += OnReimportAsset;
        SOAssetsPostProcess.onDeleteAssetAction += OnDeleteAsset;
        SOAssetsPostProcess.onMoveAssetAction += OnMoveAsset;
    }

    void OnReimportAsset(string assetPath)
    {
        onReimportAsset?.Invoke(assetPath);
    }
    void OnDeleteAsset(string assetPath)
    {
        onDeleteAsset?.Invoke(assetPath);
    }
    void OnMoveAsset(string assetPath, string oldPath)
    {
       onMoveAsset?.Invoke(assetPath, oldPath);
    }

    public void Test(string value)
    {
        Debug.Log("Event: " + value);
    }
}

[System.Serializable]
public class AssetsPostProcess : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string assetPath in importedAssets)
        {
            SOAssetsPostProcess.onReimportAssetAction?.Invoke(assetPath);
            Debug.Log("Reimported Asset: " + assetPath);
        }
        foreach (string assetPath in deletedAssets)
        {
            SOAssetsPostProcess.onDeleteAssetAction?.Invoke(assetPath);
            Debug.Log("Deleted Asset: " + assetPath);
        }
        for (int i = 0; i < movedAssets.Length; i++)
        {
            SOAssetsPostProcess.onMoveAssetAction?.Invoke(movedAssets[i], movedFromAssetPaths[i]);
            Debug.Log("Moved Asset: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
        }
    }
}