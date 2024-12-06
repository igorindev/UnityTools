using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerInputs;

public class PlayerInputBehavior : MonoBehaviour, IPlayerActions, IMenuActions
{
    public PlayerInputs inputAction;
    public PlayerInput playerInput;

    private void Awake()
    {
        inputAction = new PlayerInputs();

        inputAction.Player.click.performed += OnClick;
        inputAction.Player.move.performed += OnMove;
        //inputAction.Player.Newaction.performed += OnNewaction;

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
            DebugLogTest("Input by " + context.action.activeControl.name);
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
        Debug.Log("NEW");
    }

    public void OnMenu(InputAction.CallbackContext context)
    {

    }
}
