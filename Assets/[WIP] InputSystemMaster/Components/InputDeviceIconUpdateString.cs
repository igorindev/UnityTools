using UnityEngine;
using UnityEngine.Events;

public class InputDeviceIconUpdateString : MonoBehaviour
{
    [SerializeField] UnityEvent<string> onDeviceChanged;

    public void OnDeviceUpdated()
    {
        string deviceLayoutName = "";

        onDeviceChanged?.Invoke(deviceLayoutName);
    }
}
