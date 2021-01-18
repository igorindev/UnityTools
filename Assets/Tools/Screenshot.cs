using UnityEngine;

public class Screenshot : MonoBehaviour
{
    public int resWidth = 1920;
    public int resHeight = 1080;
    public KeyCode keyToShot = KeyCode.F1;
    
    private bool takeHiResShot = false;
    Camera cameraUsed;
    public static string ScreenShotName(int width, int height)
    {
        return string.Format("Assets/screen_{1}x{2}_{3}.png",
                             Application.dataPath,
                             width, height,
                             System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }

    public void TakeHiResShot()
    {
        takeHiResShot = true;
    }

    private void Awake()
    {
        cameraUsed = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        takeHiResShot |= Input.GetKeyDown(keyToShot);
        if (takeHiResShot)
        {
            RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
            cameraUsed.targetTexture = rt;
            Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            cameraUsed.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            cameraUsed.targetTexture = null;
            RenderTexture.active = null; // JC: added to avoid errors
            Destroy(rt);
            byte[] bytes = screenShot.EncodeToPNG();
            string filename = ScreenShotName(resWidth, resHeight);
            System.IO.File.WriteAllBytes(filename, bytes);
            Debug.Log(string.Format("Took screenshot to: {0}", filename));
            takeHiResShot = false;
        }
    }
}