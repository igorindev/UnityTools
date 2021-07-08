using UnityEngine;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(CameraFreeMovement))]
public class PhotoMode : MonoBehaviour
{
    [SerializeField] KeyCode keyToShot = KeyCode.F1;
    Camera photoCamera;
    CameraFreeMovement photoCameraFreeMovement;
    Canvas photoModeInterface;

    void Start()
    {
        photoCamera = GetComponent<Camera>();
        photoCameraFreeMovement = GetComponent<CameraFreeMovement>();
        photoModeInterface = GetComponentInChildren<Canvas>();
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(keyToShot))
        {
            TakePhoto();
        }
    }

    [ContextMenu("Enable")]
    public void EnablePhotoMode()
    {
        photoModeInterface.enabled = true;
        photoCameraFreeMovement.StartLooking();
        gameObject.SetActive(true);
    }
    [ContextMenu("Disable")]
    public void DisablePhotoMode()
    {
        photoModeInterface.enabled = false;
        photoCameraFreeMovement.StopLooking();
        gameObject.SetActive(false);
    }

    public void TakePhoto()
    {
        RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
        photoCamera.targetTexture = rt;

        Texture2D screenShot = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
        photoCamera.Render();

        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);

        photoCamera.targetTexture = null;
        RenderTexture.active = null;

        Destroy(rt);

        byte[] bytes = screenShot.EncodeToPNG();
        string filename = ScreenShotName(Screen.width, Screen.height);
        System.IO.File.WriteAllBytes(filename, bytes);

        Debug.Log(string.Format("Took screenshot to: {0}", filename));
    }

    public static string ScreenShotName(int width, int height)
    {
        return string.Format("Assets/screen_{1}x{2}_{3}.png",
                             Application.dataPath,
                             width, height,
                             System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }
}
