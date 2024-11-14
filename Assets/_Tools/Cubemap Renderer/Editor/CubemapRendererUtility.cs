using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace CubemapRender
{
    public class CubemapRendererUtility
    {
        public static void FlipPixels(Texture2D texture, bool flipX, bool flipY)
        {
            Color32[] originalPixels = texture.GetPixels32();

            IEnumerable<Color32> flippedPixels = Enumerable.Range(0, texture.width * texture.height).Select(index =>
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

        public static void ExportCubemapToPNG(Cubemap cubemap)
        {
            Debug.Log("Exporting to ... " + Application.dataPath + "/" + cubemap.name + "_????.png");

            int width = cubemap.width;
            int height = cubemap.height;
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);

            // Iterate through 6 faces.
            for (int i = 0; i < 6; ++i)
            {
                // Encode texture into PNG.
                tex.SetPixels(cubemap.GetPixels((CubemapFace)i));

                // Flip pixels on both axis (they are rotated for some reason).
                CubemapRendererUtility.FlipPixels(tex, true, true);

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

                File.WriteAllBytes(Application.dataPath + "/" + cubemap.name + "_" + faceName + ".png", tex.EncodeToPNG());
            }

            Object.DestroyImmediate(tex);
        }
    }
}
