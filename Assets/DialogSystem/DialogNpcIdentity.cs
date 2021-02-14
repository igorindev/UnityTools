using UnityEngine;

namespace DialogSystem
{
    [CreateAssetMenu(fileName = "Identity", menuName = "ScriptableObjects/Dialog/Npc Identity", order = 1)]
    public class DialogNpcIdentity : ScriptableObject
    {
        [SerializeField] string whoIsTalking = "Steve";
        [SerializeField] Color whoIsTalkingColor = Color.white;
        public Color WhoIsTalkingColor { get => whoIsTalkingColor; set => whoIsTalkingColor = value; }
        public string WhoIsTalking { get => whoIsTalking; set => whoIsTalking = value; }
    }
}