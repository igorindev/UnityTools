using UnityEngine;
using UnityEngine.InputSystem;

namespace Lamou.InputSystem.SpriteMap
{
    [CreateAssetMenu(fileName = "InputSpritesMap", menuName = "Input Bindings/Input Sprites Map")]
    public class InputSpritesMap : ScriptableObject
    {
        public InputActionAsset inputActionAsset;
        public ControlSchemeSpriteData[] _inputMaps;
    }
}
