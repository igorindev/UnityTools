using UnityEngine;

namespace Utilities
{
    public class SafeAreaAnchor : MonoBehaviour
    {
        private RectTransform _transform;
        private Rect _safeArea;
        private Vector2 _anchorMin;
        private Vector2 _anchorMax;
        private ScreenOrientation _orientation;

        private void Awake()
        {
            _orientation = Screen.orientation;

            UpdateAnchoring();
        }

        private void Update()
        {
            ScreenOrientation orientation = Screen.orientation;

            if (orientation != _orientation)
            {
                _orientation = orientation;

                UpdateAnchoring();
            }
        }

        private void UpdateAnchoring()
        {
            _transform = GetComponent<RectTransform>();

            _safeArea = Screen.safeArea;

            _anchorMin = _safeArea.position;

            _anchorMax = _anchorMin + _safeArea.size;

            _anchorMin.x /= Screen.width;
            _anchorMin.y /= Screen.height;

            _anchorMax.x /= Screen.width;
            _anchorMax.y /= Screen.height;

            _transform.anchorMin = _anchorMin;
            _transform.anchorMax = _anchorMax;
        }
    }
}
