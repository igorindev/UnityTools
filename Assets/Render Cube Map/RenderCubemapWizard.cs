#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class RenderCubemapWizard : ScriptableWizard
{
    public Transform renderFromPosition;
    public TextureFormat textureFormat = TextureFormat.RGBA32;
    public int imageSize = 4096;
    [Range(0,10000)] public float RenderDistance = 10000;
   
    Cubemap cube;

    void OnWizardUpdate()
    {
        #pragma warning disable CS0219 // A variável é atribuída, mas seu valor nunca é usado
        string helpString = "Select transform to render from and cubemap to render into";
        #pragma warning restore CS0219 // A variável é atribuída, mas seu valor nunca é usado
        bool isValid = ((renderFromPosition != null) && IsPowerOfTwo(imageSize));
    }

    void OnWizardCreate()
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
        GameObject go = new GameObject("CubemapCamera");
        go.AddComponent<Camera>();
        go.GetComponent<Camera>().farClipPlane = RenderDistance;
        // place it on the object
        go.transform.position = renderFromPosition.position;
        go.transform.rotation = Quaternion.identity;
        // render into cubemap      
        go.GetComponent<Camera>().RenderToCubemap(cube);

        // destroy temporary camera
        DestroyImmediate(go);

        ConvertToPng();
    }

    bool IsPowerOfTwo(int x)
    {
        return (x != 0) && ((x & (x - 1)) == 0);
    }

    [MenuItem("Tools/Render/Render Cubemap")]
    static void RenderCubemap()
    {
        ScriptableWizard.DisplayWizard<RenderCubemapWizard>(
            "Render Cubemap", "Render");
    }

    public void ConvertToPng()
    {
        Cubemap m_cubemap = cube;
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
}
#endif