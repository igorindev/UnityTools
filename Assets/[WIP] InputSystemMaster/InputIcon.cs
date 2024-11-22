using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class InputIcon : MonoBehaviour
{
    public string inputActionName = "Click";
    public List<UnityEngine.InputSystem.InputBinding> inputBinding;

    public Image Icon { get; set; }

    private void Start()
    {
        Icon = GetComponent<Image>();
        PlayerInputController.Instance.inputSystemCheckDevice.AddIconToList(this);
    }
}
