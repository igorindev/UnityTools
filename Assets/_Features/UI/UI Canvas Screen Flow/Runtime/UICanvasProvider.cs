using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace CanvasSubsystem
{
    public class UICanvasProvider
    {
        public static async UniTask<UICanvas> Get(string canvasId)
        {
            AsyncOperationHandle<GameObject> op = Addressables.LoadAssetAsync<GameObject>(canvasId);
            GameObject canvas = await op;
            GameObject inst = Object.Instantiate(canvas);
            return inst.GetComponent<UICanvas>();
        }
    }
}
