using UnityEngine;

namespace CanvasSubsystem
{
    [CreateAssetMenu(menuName = "UI Canvas/Database")]
    public class UICanvasDatabase : ScriptableObject
    {
        [SerializeField] UICanvasData[] canvasDatas;
    }
}