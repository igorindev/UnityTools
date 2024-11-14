using UnityEditor;
using UnityEngine;

namespace CubemapRender
{
    public class CubemapToPNG : ScriptableWizard
    {
        public Cubemap m_cubemap = null;

        [MenuItem("Tools/Cubemap/Export to PNG...")]
        public static void SaveCubeMapToPng()
        {
            DisplayWizard<CubemapToPNG>("Save CubeMap To Png", "Save");
        }

        public void OnWizardUpdate()
        {
            helpString = "Select cubemap to save to individual .png";
            if (Selection.activeObject is Cubemap && m_cubemap == null)
            {
                m_cubemap = Selection.activeObject as Cubemap;
            }

            isValid = (m_cubemap != null);
        }

        public void OnWizardCreate()
        {
            CubemapRendererUtility.ExportCubemapToPNG(m_cubemap);
        }
    }
}
