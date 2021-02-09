using UnityEngine;

namespace Scripts.Utility
{
    public class HierarchyHighlighter : MonoBehaviour
    {
        public static readonly Color DEFAULT_BACKGROUND_COLOR = new Color(0.76f, 0.76f, 0.76f, 1f);

        public static readonly Color DEFAULT_BACKGROUND_COLOR_INACTIVE = new Color(0.306f, 0.396f, 0.612f, 1f);

        public static readonly Color DEFAULT_TEXT_COLOR = Color.black;

        [Header("Active State")]
        public Color Text_Color = DEFAULT_TEXT_COLOR;

        public FontStyle TextStyle = FontStyle.Bold;

        /// <summary>
        /// Color of the background.  Set alpha 0 to not draw a background period.
        /// </summary>
        public Color Background_Color = DEFAULT_BACKGROUND_COLOR;
    }
}
