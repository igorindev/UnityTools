using Lamou.InputSystem.SpriteMap;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputController : Singleton<PlayerInputController>
{
    [SerializeField] private InputSpritesMap _inputSpritesMap;

    private ReadOnlyArray<InputControlScheme> schemes;

    public PlayerInputUpdateHandler PlayerInputUpdateHandler { get; private set; }
    public PlayerInput PlayerInput { get; private set; }
    public InputToSpriteUtility InputSprite { get; private set; }
    public ReadOnlyArray<InputControlScheme> Schemes => schemes;

    protected override void Awake()
    {
        base.Awake();

        PlayerInput = GetComponent<PlayerInput>();
        schemes = PlayerInput.actions.controlSchemes;

        PlayerInputUpdateHandler = new PlayerInputUpdateHandler(PlayerInput, schemes);
        InputSprite = new InputToSpriteUtility(_inputSpritesMap, PlayerInputUpdateHandler, PlayerInput.actions);
    }
}
