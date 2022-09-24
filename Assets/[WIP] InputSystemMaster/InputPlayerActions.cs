using UnityEngine;
using UnityEngine.InputSystem;

public class InputPlayerActions : MonoBehaviour
{
    public PlayerInputs inputAction;
    public PlayerInput playerInput;

    private void Awake()
    {
        inputAction = new PlayerInputs();

        inputAction.Player.Click.performed += OnClick;
        inputAction.Player.Move.performed += OnMove;
        inputAction.Player.Newaction.performed += OnNewaction;

        playerInput = GetComponent<PlayerInput>();
        playerInput.actions = inputAction.asset;
        
        playerInput.ActivateInput();
    }

    public void DebugLogTest(string log)
    {
        Debug.Log(log);
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            DebugLogTest("Input by " + inputAction.KeyScheme);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            DebugLogTest(context.ReadValue<Vector2>().ToString());
        }
    }

    public void OnNewaction(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }
}