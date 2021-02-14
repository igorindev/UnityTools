using UnityEngine;

namespace DialogSystem
{
    [CreateAssetMenu(fileName = "Speech", menuName = "ScriptableObjects/Dialog/Speech", order = 1)]
    public class DialogObj : ScriptableObject
    {
        public Speech[] dialogs;
    }
}