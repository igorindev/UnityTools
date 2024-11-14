using UnityEngine;

namespace SpringSystem
{
    public class SpringManager : MonoBehaviour
    {
        // DynamicRatio is paramater for activated level of dynamic animation 
        [SerializeField, Range(0f, 1f)] float dynamicRatio = 1.0f;
        [SerializeField] float stiffnessForce;
        [SerializeField] AnimationCurve stiffnessCurve;
        [SerializeField] float dragForce;
        [SerializeField] AnimationCurve dragCurve;
        [SerializeField] SpringBone[] springBones;

        public float DynamicRatio => dynamicRatio;

#if UNITY_EDITOR
        private void Start()
        {
            UpdateParameters();
        }

        public void Setup(SpringBone[] bones)
        {
            springBones = bones;
        }

        private void Update()
        {
            SyncValues();
        }
#endif

        private void LateUpdate()
        {
            if (dynamicRatio > 0.0f)
            {
                for (int i = 0; i < springBones.Length; i++)
                {
                    if (dynamicRatio > springBones[i].threshold)
                    {
                        springBones[i].UpdateSpring();
                    }
                }
            }
            else
            {
                dynamicRatio = Mathf.Clamp(dynamicRatio, 0f, 1f);
            }
        }

        public void ApplyForce(Vector3 force)
        {
            for (int i = 0; i < springBones.Length; i++)
            {
                springBones[i].springForce = force;
            }
        }

        private void SyncValues()
        {
            UpdateParameters();
        }

        private void UpdateParameters()
        {
            UpdateParameter("stiffnessForce", stiffnessForce, stiffnessCurve);
            UpdateParameter("dragForce", dragForce, dragCurve);
        }

        private void UpdateParameter(string fieldName, float baseValue, AnimationCurve curve)
        {
#if UNITY_EDITOR
            float start = curve.keys[0].time;
            float end = curve.keys[curve.length - 1].time;

            System.Reflection.FieldInfo prop = springBones[0].GetType().GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            for (int i = 0; i < springBones.Length; i++)
            {
                if (!springBones[i].isUseEachBoneForceSettings)
                {
                    float scale = curve.Evaluate(start + (end - start) * i / (springBones.Length - 1));
                    prop.SetValue(springBones[i], baseValue * scale);
                }
            }
#endif
        }
    }
}
