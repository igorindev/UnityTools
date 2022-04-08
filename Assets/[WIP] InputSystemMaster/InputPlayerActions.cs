using UnityEngine;
using UnityEngine.InputSystem;

public class InputPlayerActions : MonoBehaviour
{
    public void DebugLogTest(string log)
    {
        Debug.Log(log);
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            DebugLogTest("Input by " + PlayerInputController.Instance.PlayerInput.currentControlScheme);
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