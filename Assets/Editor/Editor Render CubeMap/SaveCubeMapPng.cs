#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SaveCubeMapPng : ScriptableWizard
{
    #region private members
    public Cubemap m_cubemap = null;
    #endregion

    #region wizard methods.
    [MenuItem("Tools/Render/Save CubeMap To Png")]
    public static void SaveCubeMapToPng()
    {
        DisplayWizard<SaveCubeMapPng>("Save CubeMap To Png", "Save");
    }

    public void OnWizardUpdate()
    {
        helpString = "Select cubemap to save to individual .png";
        if (Selection.activeObject is Cubemap && m_cubemap == null)
            m_cubemap = Selection.activeObject as Cubemap;

        isValid = (m_cubemap != null);
    }

    public void OnWizardCreate()
    {
        Debug.Log("Exporting to ... " + Application.dataPath + "/" + m_cubemap.name + "_????.png");

        int width = m_cubemap.width;
        int height = m_cubemap.height;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);

        // Iterate through 6 faces.
        for (int i = 0; i < 6; ++i)
        {
            // Encode texture into PNG.
            tex.SetPixels(m_cubemap.GetPixels((CubemapFace)i));

            // Flip pixels on both axis (they are rotated for some reason).
            FlipPixels(tex, true, true);

            string faceName;

            // Save as PNG.
            if (((CubemapFace)i).ToString() == "NegativeX")
            {
                faceName = "PositiveX";
            }
            else if (((CubemapFace)i).ToString() == "PositiveX")
            {
                faceName = "NegativeX";
            }
            else
            {
                faceName = ((CubemapFace)i).ToString();
            }
            File.WriteAllBytes(Application.dataPath + "/" + m_cubemap.name + "_" + faceName + ".png", tex.EncodeToPNG());
        }

        DestroyImmediate(tex);
    }
    public static void FlipPixels(Texture2D texture, bool flipX, bool flipY)
    {
        Color32[] originalPixels = texture.GetPixels32();

        var flippedPixels = Enumerable.Range(0, texture.width * texture.height).Select(index =>
        {
            int x = index % texture.width;
            int y = index / texture.width;
            if (flipX)
                x = texture.width - 1 - x;

            if (flipY)
                y = texture.height - 1 - y;

            return originalPixels[y * texture.width + x];
        }
        );

        texture.SetPixels32(flippedPixels.ToArray());
        texture.Apply();
    }
    #endregion
}

#endif
