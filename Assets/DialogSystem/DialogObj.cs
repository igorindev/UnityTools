using UnityEngine;

namespace DialogSystem
{
    [CreateAssetMenu(fileName = "Speech", menuName = "ScriptableObjects/Dialog/Speech")]
    public class DialogObj : ScriptableObject
    {
        public Speech[] dialogs;
    }
}