using UnityEngine;
using UnityEngine.InputSystem;

public abstract class InputTo : MonoBehaviour
{
    protected virtual void Start()
    {
        PlayerInputController.Instance.PlayerInputUpdateHandler.RegisterCallback(HandleInputUpdate);
    }

    protected virtual void OnDestroy()
    {
        PlayerInputController.Instance.PlayerInputUpdateHandler.RemoveCallback(HandleInputUpdate);
    }

    protected virtual void HandleInputUpdate(string oldScheme, string newScheme)
    {

    }
}

public abstract class InputTo<T> : InputTo where T : MonoBehaviour
{
    [SerializeField] protected InputActionReference _inputActionReference;

    protected InputAction _inputAction;

    public T Conversion { get; private set; }

    protected override void Start()
    {
        Conversion = GetComponent<T>();

        _inputAction = InputToSpriteUtility.GetInputAction(_inputActionReference.action.name);

        base.Start();
    }
}
