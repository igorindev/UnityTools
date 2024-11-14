#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

// Create a texture and fill it with Perlin noise.
// Try varying the xOrg, yOrg and scale values in the inspector
// while in Play mode to see the effect they have on the noise.

public class PerlinNoiseGenerator : EditorWindow
{
    // Width and height of the texture in pixels.
    [Header("Sizes")]
    [SerializeField] int size = 2048;

    // The origin of the sampled area in the plane.
    [Header("Positions")]
    [SerializeField] float xOrg = 0;
    [SerializeField] float yOrg = 0;

    // The number of cycles of the basic noise pattern that are repeated
    // over the width and height of the texture.
    [SerializeField] float scale = 20f;

    [Header("Location")]
    [SerializeField] string noiseName = "NewNoise";
    [SerializeField] string path;

    private Texture2D noiseTex;
    private Color[] pix;

    [MenuItem("Tools/Perlin Noise")]
    static void CreateNoise()
    {
        var window = GetWindow<PerlinNoiseGenerator>("Perlin Noise");
        window.position = new Rect(0, 0, 400, 600);
        window.Show();
    }

    private void OnEnable()
    {
        noiseTex = new Texture2D(256, 256);
    }

    private void Update()
    {
        PreviewNoise();
    }

    void OnGUI()
    {
        GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));

        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        size = EditorGUILayout.IntField("Size", size);

        GUILayout.Space(5);
        GUILayout.Label("Positions", EditorStyles.boldLabel);
        if (size < 1)
        {
            size = 1;
        }

        xOrg = EditorGUILayout.FloatField("xOrg", xOrg);
        yOrg = EditorGUILayout.FloatField("yOrg", yOrg);
        scale = EditorGUILayout.FloatField("Scale", scale);

        GUILayout.Space(5);
        GUILayout.Label("Path", EditorStyles.boldLabel);

        noiseName = EditorGUILayout.TextField("Name", noiseName);
        path = EditorGUILayout.TextField("Path", path);

        if (GUI.Button(new Rect(10, 220, 80, 20), "Create"))
        {
            CalcNoise();
        }

        GUI.EndGroup();

        GUI.BeginGroup(new Rect(5, 330, 260, Screen.height));
        GUI.EndGroup();

        GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));
        GUI.DrawTexture(new Rect(10, 250, 256, 256), noiseTex);

        GUI.EndGroup();
    }

    public void ConvertToPng()
    {
        byte[] _bytes = noiseTex.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/" + noiseName + ".png", _bytes);
        Debug.Log(_bytes.Length / 1024 + "Kb was saved as: " + path);
    }

    void CalcNoise()
    {
        noiseTex = new Texture2D(size, size);
        pix = new Color[noiseTex.width * noiseTex.height];
        // For each pixel in the texture...
        float y = 0.0F;

        while (y < noiseTex.height)
        {
            float x = 0.0F;
            while (x < noiseTex.width)
            {
                float xCoord = xOrg + x / noiseTex.width * scale;
                float yCoord = yOrg + y / noiseTex.height * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                pix[(int)y * noiseTex.width + (int)x] = new Color(sample, sample, sample);
                x++;
            }
            y++;
        }

        // Copy the pixel data to the texture and load it into the GPU.
        noiseTex.SetPixels(pix);
        noiseTex.Apply();

        ConvertToPng();
    }
    void PreviewNoise()
    {
        noiseTex = new Texture2D(256, 256);
        pix = new Color[noiseTex.width * noiseTex.height];
        // For each pixel in the texture...
        float y = 0.0F;

        while (y < noiseTex.height)
        {
            float x = 0.0F;
            while (x < noiseTex.width)
            {
                float xCoord = xOrg + x / noiseTex.width * scale;
                float yCoord = yOrg + y / noiseTex.height * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                pix[(int)y * noiseTex.width + (int)x] = new Color(sample, sample, sample);
                x++;
            }
            y++;
        }

        // Copy the pixel data to the texture and load it into the GPU.
        noiseTex.SetPixels(pix);
        noiseTex.Apply();
    }
}
#endif