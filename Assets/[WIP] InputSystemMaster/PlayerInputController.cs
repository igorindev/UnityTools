using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputController : Singleton<PlayerInputController>
{
    internal PlayerInput playerInput;
    internal InputCheckDevice inputSystemCheckDevice;

    protected override void Awake()
    {
        base.Awake();

        playerInput = GetComponent<PlayerInput>();
        inputSystemCheckDevice = GetComponent<InputCheckDevice>();
    }
}
