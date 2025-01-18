using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AnimatorLODSystem
{
    public class AnimatorLODManager : MonoBehaviour
    {
        private static AnimatorLODData[] _distanceCascades = new[] { AnimatorLODData.Default };
        private static Transform _targetTransform;
        private static List<AnimatorLOD> _animatorLODs = new();
        private Vector3 Target => _targetTransform != null ? _targetTransform.position : (_targetTransform = Camera.main.transform).position;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitializeSubsystem()
        {
            GameObject instance = new GameObject("AnimatorLODSubsystem");
            AnimatorLODManager animatorLODManager = instance.AddComponent<AnimatorLODManager>();
            DontDestroyOnLoad(instance);

            _distanceCascades = (Resources.Load("AnimatorLODSettings/AnimatorLODSettings") as AnimatorLODSettings).animatorLODDatas;
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            float frameCount = Time.frameCount;

            foreach (AnimatorLOD item in _animatorLODs)
            {
                AnimatorLODData AnimationCascadeData = GetCascadeUpdateRate(item.WorldPosition, out int cascadeLevel);

                item.UpdateDeltaTime(deltaTime, frameCount, AnimationCascadeData, cascadeLevel);
            }
        }

        public static void SetTarget(Transform target)
        {
            _targetTransform = target;
        }

        public static void Register(AnimatorLOD animationLOD)
        {
            _animatorLODs.Add(animationLOD);
        }

        public static void Deregister(AnimatorLOD animationLOD)
        {
            _animatorLODs.Remove(animationLOD);
        }

        private AnimatorLODData GetCascadeUpdateRate(Vector3 worldPosition, out int cascadeIndex)
        {
            for (int i = 0; i < _distanceCascades.Length; i++)
            {
                if ((Target - worldPosition).sqrMagnitude <= _distanceCascades[i].DistanceUnitsSqr)
                {
                    cascadeIndex = i;
                    return _distanceCascades[i];
                }
            }

            cascadeIndex = -1;
            return AnimatorLODData.Zero;
        }

        private void OnDrawGizmosSelected()
        {
            Color[] colors = new Color[] { Color.cyan, Color.red, Color.green, Color.blue, Color.magenta };

            for (int i = 0; i < _distanceCascades.Length; i++)
            {
                AnimatorLODData item = _distanceCascades[i];
                var color = colors[i % colors.Length];

                Vector3 t = Target;
                t = new Vector3(Target.x, 0, Target.z);

                Handles.color = color;
                Handles.DrawWireDisc(t, Vector3.up, item.distanceUnits);

                color.a = 0.01f;
                Handles.color = color;
                Handles.DrawSolidDisc(t, Vector3.up, item.distanceUnits);
            }
        }
    }
}
