using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class AnimatorOverrideGroup : ScriptableObject
{
    [Serializable]
    public class AnimatorOverride
    {
        public string overrideName;
        public AnimatorOverrideController overrideController;
    }

    [SerializeField] AnimatorOverride[] overrides;

    public int Count => overrides.Length;

    public AnimatorOverride GetOverride(string overrideKey)
    {
        return overrides.FirstOrDefault(x => x.overrideName == overrideKey);
    }

}
