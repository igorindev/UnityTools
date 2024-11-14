using UnityEditor;
using UnityEngine;

namespace CubemapRender
{
    public class CubemapRenderer : ScriptableWizard
    {
        public Transform renderFromPosition;
        public TextureFormat textureFormat = TextureFormat.RGBA32;
        public int imageSize = 4096;
        [Range(0, 10000)] public float RenderDistance = 10000;

        private Cubemap cube;

        [MenuItem("Tools/Cubemap/Render...")]
        private static void RenderCubemap()
        {
            DisplayWizard<CubemapRenderer>("Cubamap Renderer", "Render");
        }

        private void OnWizardUpdate()
        {
#pragma warning disable CS0219 // A variável é atribuída, mas seu valor nunca é usado
            string helpString = "Select transform to render from and cubemap to render into";
#pragma warning restore CS0219 // A variável é atribuída, mas seu valor nunca é usado
            bool isValid = ((renderFromPosition != null) && IsPowerOfTwo(imageSize));
        }

        private void OnWizardCreate()
        {
            if (!IsPowerOfTwo(imageSize))
            {
                Debug.LogError("ImageSize should have a Power of Two value");
                return;
            }

            if (renderFromPosition == null)
            {
                Debug.LogError("renderFromPosition should receive a Transform");
                return;
            }

            cube = new Cubemap(imageSize, textureFormat, true);

            // create temporary camera for rendering
            GameObject go = new("CubemapCamera");
            Camera camera = go.AddComponent<Camera>();
            camera.farClipPlane = RenderDistance;
            // place it on the object
            go.transform.SetPositionAndRotation(renderFromPosition.position, Quaternion.identity);
            // render into cubemap
            camera.RenderToCubemap(cube);

            // destroy temporary camera
            DestroyImmediate(go);

            CubemapRendererUtility.ExportCubemapToPNG(cube);
        }

        private bool IsPowerOfTwo(int x)
        {
            return (x != 0) && ((x & (x - 1)) == 0);
        }
    }
}
