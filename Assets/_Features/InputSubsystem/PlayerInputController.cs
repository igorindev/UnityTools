using Lamou.InputSystem.SpriteMap;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputController : Singleton<PlayerInputController>
{
    [SerializeField] private InputSpritesMap _inputSpritesMap;

    private ReadOnlyArray<InputControlScheme> _schemes;

    public PlayerInputUpdateHandler PlayerInputUpdateHandler { get; private set; }
    public PlayerInput PlayerInput { get; private set; }
    public InputToSpriteUtility InputSprite { get; private set; }
    public InputBindingsSaveData InputSaveData { get; private set; }
    public ReadOnlyArray<InputControlScheme> Schemes => _schemes;

    protected override void Awake()
    {
        base.Awake();

        PlayerInput = GetComponent<PlayerInput>();
        _schemes = PlayerInput.actions.controlSchemes;

        PlayerInputUpdateHandler = new PlayerInputUpdateHandler(PlayerInput, _schemes);
        InputSprite = new InputToSpriteUtility(_inputSpritesMap, PlayerInputUpdateHandler, PlayerInput.actions);

        InputSaveData = new InputBindingsSaveData(PlayerInput);
    }
}
