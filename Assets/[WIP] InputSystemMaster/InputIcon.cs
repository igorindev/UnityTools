using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class InputIcon : MonoBehaviour
{
    public string inputActionName = "Click";
    public List<InputBinding> inputBinding;
    public Image Icon { get; set; }
    void Start()
    {
        Icon = GetComponent<Image>();
        PlayerInputController.instance.InputSystemCheckDevice.AddIconToList(this);
    }
}