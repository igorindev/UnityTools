using CanvasSubsystem;
using UnityEngine;

public class UICanvasOpener : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        UICanvas canvas = await UICanvasSubsystem.OpenCanvas<UICanvas>("p_UI_Canvas_Base");
    }
}
