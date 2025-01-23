using System;
using UnityEngine;

namespace AnimatorLODSystem
{
    [Serializable]
    public struct AnimatorLODData
    {
        [Min(0)] public float distanceUnits;
        [Header("Data")]
        [Min(1)] public int updateRate;
        public SkinQuality skinnedMeshSkinQuality;

        public float DistanceUnitsSqr => distanceUnits * distanceUnits;
        public static AnimatorLODData Default => new(100, 1, SkinQuality.Auto);
        public static AnimatorLODData Zero => new(float.MaxValue, 1, SkinQuality.Bone1);

        public AnimatorLODData(float distanceUnits, int updateRate, SkinQuality skinnedMeshSkinQuality)
        {
            this.distanceUnits = distanceUnits;
            this.updateRate = updateRate;
            this.skinnedMeshSkinQuality = skinnedMeshSkinQuality;
        }
    }
}
