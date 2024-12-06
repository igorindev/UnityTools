using TMPro;
using UnityEngine;

public class InputTextLinkerExample : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    private InputUpdateReceiver _inputUpdateReceiver;

    private void Start()
    {
        _inputUpdateReceiver = new InputUpdateReceiver("Click");
        _inputUpdateReceiver.Add(HandleUpdate);
    }

    private void OnDestroy()
    {
        _inputUpdateReceiver.Remove(HandleUpdate);
    }

    private void HandleUpdate(string iconString)
    {
        _text.text = $"Use the {iconString} to shot the weapon";
    }
}
