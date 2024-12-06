using System;
using TMPro;
using UnityEngine;

namespace Lamou.InputSystem.SpriteMap
{
    [Serializable]
    public class ControlSchemeSpriteData
    {
        public string DeviceLayoutName;
        public Texture2D SpriteAtlas;
        public TMP_SpriteAsset GeneratedSpriteAsset;
        public ControlSchemeBindingsPreset BindingsPreset;
    }
}
