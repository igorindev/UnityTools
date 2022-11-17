using UnityEngine;

namespace UmbraEvolution.PerLayerCameraCulling
{
    /// <summary> Helps users access certain camera properties using the inspector that are typically only accessible via scripting </summary>
#if UNITY_2019_1_OR_NEWER //conditional compilation for newer versions of Unity that lets us get an OnDestroy callback
    [ExecuteAlways]
#else
    [ExecuteInEditMode] //conditional compilation for older versions of Unity that lets us get an OnDestroy callback
#endif
    [RequireComponent(typeof(Camera))]
    public class PerLayerCulling : MonoBehaviour
    {
        public const int NUMBER_OF_UNITY_LAYERS = 32;
        public const float NON_ZERO_OFFSET = 0.01f;

        [System.Serializable]
        public class LayerCullInfo
        {
            [Tooltip("Layer that these settings apply to.")]
            internal string layerName;

            public bool useDefaultCull = true;

            [Tooltip("The distance at which to start culling objects on this layer.")]
            public float cullDistance;

            [Tooltip("If true, the gizmo for this layer can be drawn in the editor.")]
            public bool showGizmo = true;

            [Tooltip("The color of the gizmo.")]
            public Color gizmoColor = Color.cyan;
        }

        [Tooltip("Set this as the default distance you want culling to occur across the majority of layers.")]
        public float defaultCullDistance;

        [Tooltip("A multiplier applied to all distances you've entered for an easy way to modify performance dynamically")]
        public float DistanceMultiplier = 1f;

        [Tooltip("Eliminates the potential for objects to pop in and out of culling range when rotating the camera in place. Has a small performance cost.")]
        public bool UseSphericalCulling;

        [Tooltip("If true, the enabled gizmos for this PerLayerCulling component will always be drawn, even when the camera object isn't selected.")]
        public bool AlwaysShowGizmos = false;

        [Tooltip("In order to work properly, this must always have 32 elements, each corrisponding to the index of your layers. Set each index to set culling distance of a layer. For example: set Element 0 to 1000 and all objects on the default layer will cull at 1000 units from this camera. The layer names should automatically be appropriately assigned for you though, so you don't have to look them up.")]
        [NonReorderable] public LayerCullInfo[] layerCullInfoArray = new LayerCullInfo[NUMBER_OF_UNITY_LAYERS];

        private Camera _myCamera;
        public Camera MyCamera
        {
            get
            {
                if (_myCamera == null)
                {
                    if (!TryGetComponent<Camera>(out _myCamera))
                    {
                        Debug.LogError("A PerLayerCulling component requires a sibling Camera component.");
                    }
                }

                return _myCamera;
            }
        }

        public float MinClip => MyCamera.nearClipPlane + NON_ZERO_OFFSET;

        void OnValidate()
        {
            // Small initialization check since the DefaultCullDistance can't be zero except when the script is first loaded
            // We don't want to mess with this when validating during gameplay
            if (defaultCullDistance == 0f)
            {
                defaultCullDistance = MyCamera.farClipPlane;
            }

            TriggerUpdate();
        }
        void Awake()
        {
            OnValidate();
        }
        void OnDestroy()
        {
            // use _myCamera field because the whole GameObject may be being deleted right now,
            // in which case we can't actually reset anything
            if (_myCamera != null)
            {
                _myCamera.layerCullSpherical = false;
                _myCamera.layerCullDistances = new float[NUMBER_OF_UNITY_LAYERS];
            }
        }
        void OnDrawGizmosSelected()
        {
            DrawGizmos();
        }
        void OnDrawGizmos()
        {
            if (AlwaysShowGizmos)
                DrawGizmos();
        }

        public void TriggerUpdate()
        {
            ValidateAll();
            ApplyCurrentSettings();
        }

        void ValidateAll()
        {
            ValidateLayerCullInfoArray();
            UpdateLayerNames();

            UpdateAllDefaultCulls();

            ValidateAgainstNearClipPlane();
            ValidateAgainstFarClipPlane();
        }
        void DrawGizmos()
        {
            for (int i = 0; i < layerCullInfoArray.Length; i++)
            {
                LayerCullInfo cullInfo = layerCullInfoArray[i];

                if (cullInfo.layerName == "Layer Not Defined") continue;

                if (cullInfo.useDefaultCull == false && cullInfo.showGizmo)
                {
                    // aligns the gizmo to the local object's transform
                    Gizmos.matrix = transform.localToWorldMatrix;
                    // sets the gizmo colour as specified in the inspector
                    Gizmos.color = cullInfo.gizmoColor;

                    // draw a camera frustum gizmo to show typical culling bounds
                    Gizmos.DrawFrustum(Vector3.zero, MyCamera.fieldOfView, cullInfo.cullDistance * DistanceMultiplier, MyCamera.nearClipPlane, MyCamera.aspect);

                    // draw a spherical gizmo to show spherical culling bounds
                    if (UseSphericalCulling)
                    {
                        Gizmos.DrawWireSphere(Vector3.zero, cullInfo.cullDistance * DistanceMultiplier);
                    }

                    WriteOnView(cullInfo.layerName, transform.position + transform.forward * (cullInfo.cullDistance * DistanceMultiplier), 10000, 20, cullInfo.gizmoColor);
                }
            }
        }

        void UpdateAllDefaultCulls()
        {
            for (int i = 0; i < NUMBER_OF_UNITY_LAYERS; i++)
            {
                if (layerCullInfoArray[i].useDefaultCull)
                {
                    layerCullInfoArray[i].cullDistance = defaultCullDistance;
                }
            }
        }

        void ValidateLayerCullInfoArray()
        {
            // if the array was resized to be smaller than the correct size, fill the remaining slots in the temp array with defaults
            // can also happen if this is the first validation pass and some layers weren't initialized yet
            for (int index = 0; index < NUMBER_OF_UNITY_LAYERS; index++)
            {
                if (layerCullInfoArray[index] == null)
                {
                    layerCullInfoArray[index] = new LayerCullInfo();
                    layerCullInfoArray[index].cullDistance = defaultCullDistance;
                }
            }
        }

        public void UpdateLayerNames()
        {
            for (int index = 0; index < NUMBER_OF_UNITY_LAYERS; index++)
            {
                if (layerCullInfoArray[index] != null)
                {
                    layerCullInfoArray[index].layerName = !string.IsNullOrEmpty(LayerMask.LayerToName(index)) ? LayerMask.LayerToName(index) : "Layer Not Defined";
                }
                else
                {
                    Debug.LogError("Null layer in LayerCullInfoArray. Call ValidateLayerCullInfoArray() first, or make sure there are no null layers.");
                }
            }
        }

        /// <summary>
        /// Nothing can be closer to the camera than the near clip plane, so we have to make sure that's true
        /// </summary>
        public void ValidateAgainstNearClipPlane()
        {
            //have to make sure that the default distance is not less than the near clip plane (since nothing should be culled closer than that)
            if (defaultCullDistance < MyCamera.nearClipPlane)
            {
                defaultCullDistance = MyCamera.nearClipPlane + NON_ZERO_OFFSET;
            }

            //makes sure the cull distance for each layer isn't smaller than the near clip plane
            foreach (LayerCullInfo cullInfo in layerCullInfoArray)
            {
                if (cullInfo.cullDistance < MyCamera.nearClipPlane)
                {
                    cullInfo.cullDistance = MyCamera.nearClipPlane + NON_ZERO_OFFSET;
                }
            }

            //Makes sure the distance multiplier won't make any of the cull distances smaller than the near clip plane
            float nearestCullDistance = layerCullInfoArray[0].cullDistance;
            foreach (LayerCullInfo cullInfo in layerCullInfoArray)
            {
                if (cullInfo.cullDistance < nearestCullDistance)
                {
                    nearestCullDistance = cullInfo.cullDistance;
                }
            }
            if (nearestCullDistance * DistanceMultiplier < (MyCamera.nearClipPlane + NON_ZERO_OFFSET))
            {
                DistanceMultiplier = (MyCamera.nearClipPlane + NON_ZERO_OFFSET) / nearestCullDistance;
            }
        }

        /// <summary>
        /// Nothing can be farther from the camera than the far clip plane, so we have to make sure that's true
        /// </summary>
        public void ValidateAgainstFarClipPlane()
        {
            float furthestCullDistance = layerCullInfoArray[0].cullDistance;
            foreach (LayerCullInfo cullInfo in layerCullInfoArray)
            {
                if (cullInfo.cullDistance > furthestCullDistance)
                {
                    furthestCullDistance = cullInfo.cullDistance;
                }
            }

            MyCamera.farClipPlane = furthestCullDistance * DistanceMultiplier;
        }

        /// <summary>
        /// Take the current data from this instance of PerLayerCulling and apply it to the sibling camera
        /// </summary>
        public void ApplyCurrentSettings()
        {
            //we can only assign culling plane distances with a float array, so we'll take the float values from the custom class and put them into one after applying our multiplier
            float[] cullFloatArray = new float[NUMBER_OF_UNITY_LAYERS];
            for (int index = 0; index < NUMBER_OF_UNITY_LAYERS; index++)
            {
                cullFloatArray[index] = layerCullInfoArray[index].cullDistance * DistanceMultiplier;
            }

            // assigns culling distances to all layers
            MyCamera.layerCullDistances = cullFloatArray;

            // apply the spherical culling setting
            MyCamera.layerCullSpherical = UseSphericalCulling;
        }

        public void ResetAllToDefault()
        {
            foreach (LayerCullInfo cullDistance in layerCullInfoArray)
            {
                cullDistance.useDefaultCull = true;
                cullDistance.cullDistance = defaultCullDistance;
            }
        }

        public static void WriteOnView(string text, Vector3 pos, float maxViewDistance = 2.5f, int fontSize = 20, Color? color = null, FontStyle fontStyle = FontStyle.Bold)
        {
#if UNITY_EDITOR
            if (Vector3.Distance(pos, UnityEditor.SceneView.currentDrawingSceneView.camera.transform.position) > maxViewDistance)
            {
                return;
            }

            GUIStyle style = new GUIStyle();

            style.fontSize = fontSize;
            style.fontStyle = fontStyle;
            style.normal.textColor = color ?? Color.white;

            UnityEditor.Handles.Label(pos, text, style);
#endif
        }
    }
}
