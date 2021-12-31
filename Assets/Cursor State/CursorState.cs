using UnityEngine;

public class CursorState : MonoBehaviour
{
    [SerializeField] CursorLockMode cursorLockMode = CursorLockMode.None;
    [SerializeField] bool visible = true;

    void Start()
    {
        Cursor.visible = visible;
        Cursor.lockState = cursorLockMode;
    }

    public void SetCursorLocked(bool visibility)
    {
        Cursor.visible = visibility;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void SetCursorConfined(bool visibility)
    {
        Cursor.visible = visibility;
        Cursor.lockState = CursorLockMode.Confined;
    }
    public void SetCursorFree(bool visibility)
    {
        Cursor.visible = visibility;
        Cursor.lockState = CursorLockMode.None;
    }
}
