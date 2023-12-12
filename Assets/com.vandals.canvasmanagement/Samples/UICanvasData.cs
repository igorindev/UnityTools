using UnityEngine;

namespace CanvasSubsystem
{
    [CreateAssetMenu(menuName = "UI Canvas/Data")]
    public class UICanvasData : ScriptableObject
    {
        [SerializeField] string ID;
        [SerializeField] UICanvas canvas;
    }
}