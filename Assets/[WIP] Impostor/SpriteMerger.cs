using System.Threading;
using UnityEngine;

public class SpriteMerger : MonoBehaviour
{
    [SerializeField] Texture2D[] texturesToMerge = null;
    [SerializeField] SpriteRenderer finalSpriteRenderer = null;
    [SerializeField] TexturesSize texturesSize = TexturesSize.c;
    [SerializeField] Color backgroundColor = Color.black;
    [SerializeField, Min(1)] int lines = 2;
    [SerializeField, Min(0)] int pixelsOffset = 10;

    public enum TexturesSize 
    {
        [InspectorName("512")]
        a = 512,
        [InspectorName("1024")]
        b = 1024,
        [InspectorName("2048")]
        c = 2048,
        [InspectorName("4096")]
        d = 4096
    }

    [ContextMenu("Merge")]
    void CreateTexture()
    {
        int lineCount = 0;
        int toUp = 1;
        int toSide = 0;

        Resources.UnloadUnusedAssets();

        var newTex = new Texture2D((int)texturesSize, (int)texturesSize);
        Debug.Log(newTex.width);
        for (int x = 0; x < newTex.width; x++)
        {
            for (int y = 0; y < newTex.height; y++)
            {
                newTex.SetPixel(x, y, backgroundColor);
            }
        }

        for (int i = 0; i < texturesToMerge.Length; i++)
        {
            Texture2D t = TextureScaler.Scaled(texturesToMerge[i], (int)((float)newTex.width / lines) - pixelsOffset * 2, (int)((float)newTex.height / lines) - pixelsOffset * 2);

            for (int x = 0; x < t.width; x++)
            {
                for (int y = 0; y < t.height; y++)
                {
                    var color = t.GetPixel(x, y).a != 1 ? newTex.GetPixel(x, y) + t.GetPixel(x, y) : t.GetPixel(x, y);

                    newTex.SetPixel((x + pixelsOffset) + (toSide * (int)((float)newTex.width / lines)),
                                    (y + pixelsOffset) - (toUp * (int)((float)newTex.height / lines)),
                                    color);
                }
            }

            toSide += 1;
            lineCount += 1;
            if (lineCount == lines)
            {
                toUp += 1;
                toSide = 0;
                lineCount = 0;
            }
        }

        newTex.Apply();
        var finalSprite = Sprite.Create(newTex, new Rect(0, 0, newTex.width, newTex.height), new Vector2(0.5f, 0.5f));

        finalSprite.name = "New Sprite";
        finalSpriteRenderer.sprite = finalSprite;
    }
}

public class TextureScaler
{
    /// <summary>
    /// Returns a scaled copy of given texture. 
    /// </summary>
    /// <param name="tex">Source texure to scale</param>
    /// <param name="width">Destination texture width</param>
    /// <param name="height">Destination texture height</param>
    /// <param name="mode">Filtering mode</param>
    public static Texture2D Scaled(Texture2D src, int width, int height, FilterMode mode = FilterMode.Trilinear)
    {
        Rect texR = new Rect(0, 0, width, height);
        Gpu_scale(src, width, height, mode);

        //Get rendered data back to a new texture
        Texture2D result = new Texture2D(width, height, TextureFormat.ARGB32, true);
        result.Reinitialize(width, height);
        result.ReadPixels(texR, 0, 0, true);
        return result;
    }

    /// <summary>
    /// Scales the texture data of the given texture.
    /// </summary>
    /// <param name="tex">Texure to scale</param>
    /// <param name="width">New width</param>
    /// <param name="height">New height</param>
    /// <param name="mode">Filtering mode</param>
    public static void Scale(Texture2D tex, int width, int height, FilterMode mode = FilterMode.Trilinear)
    {
        Rect texR = new Rect(0, 0, width, height);
        Gpu_scale(tex, width, height, mode);

        // Update new texture
        tex.Reinitialize(width, height);
        tex.ReadPixels(texR, 0, 0, true);
        tex.Apply(true);    //Remove this if you hate us applying textures for you :)
    }

    // Internal unility that renders the source texture into the RTT - the scaling method itself.
    static void Gpu_scale(Texture2D src, int width, int height, FilterMode fmode)
    {
        //We need the source texture in VRAM because we render with it
        src.filterMode = fmode;
        src.Apply(true);

        //Using RTT for best quality and performance. Thanks, Unity 5
        RenderTexture rtt = new RenderTexture(width, height, 32);

        //Set the RTT in order to render to it
        Graphics.SetRenderTarget(rtt);

        //Setup 2D matrix in range 0..1, so nobody needs to care about sized
        GL.LoadPixelMatrix(0, 1, 1, 0);

        //Then clear & draw the texture to fill the entire RTT.
        GL.Clear(true, true, new Color(0, 0, 0, 0));
        Graphics.DrawTexture(new Rect(0, 0, 1, 1), src);
    }
}