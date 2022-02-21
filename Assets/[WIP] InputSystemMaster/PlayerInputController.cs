using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputController : Singleton<PlayerInputController>
{
    public PlayerInput PlayerInput { get; private set; }
    public InputCheckDevice InputSystemCheckDevice { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        PlayerInput = GetComponent<PlayerInput>();
        InputSystemCheckDevice = GetComponent<InputCheckDevice>();
    }
}
