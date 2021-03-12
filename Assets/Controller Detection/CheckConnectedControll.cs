using UnityEngine;

public class CheckConnectedControll : MonoBehaviour
{
    public static CheckConnectedControll instance;

    [SerializeField] string controlName;
    
    [Header("Current Scenes")]
    [SerializeField] ControllerIcons menusController;
    [SerializeField] ControllerIcons sceneController;

    [Header("Controller")]
    [SerializeField] Sprite BButton;
    [SerializeField] Sprite AButton;
    [SerializeField] Sprite XButton;
    [SerializeField] Sprite YButton;
    [SerializeField] Sprite RbButton;
    [SerializeField] Sprite LbButton;
    [SerializeField] Sprite RArrowButton;
    [SerializeField] Sprite LArrowButton;
    [SerializeField] Sprite UpArrowButton;
    [SerializeField] Sprite DoArrowButton;
    [SerializeField] Sprite Empty;

    bool firstFrame;

    public Sprite BButtonSprite { get => BButton; }
    public Sprite AButtonSprite { get => AButton; }
    public Sprite XButtonSprite { get => XButton; }
    public Sprite YButtonSprite { get => YButton; }
    public Sprite RbButtonSprite { get => RbButton; }
    public Sprite LbButtonSprite { get => LbButton; }
    public Sprite RArrowButtonSprite { get => RArrowButton; }
    public Sprite LArrowButtonSprite { get => LArrowButton; }
    public Sprite UpArrowButtonSprite { get => UpArrowButton; }
    public Sprite DoArrowButtonSprite { get => DoArrowButton; }
    public Sprite EmptySprite { get => Empty; }
    public ControllerIcons MenusController { get => menusController; }
    public ControllerIcons SceneController { get => sceneController;}

    public bool HasChanged { get; private set; }
    public bool ControlConnected { get; private set; }
    public bool HasGet { get; set; }

    /// <summary>
    /// Singleton
    /// </summary>
    void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Ja existe um CheckConnectedControll");
            Destroy(gameObject);
        }
        instance = this;
    }

    void Start()
    {
        HasGet = TryGetJoystick();

        HasChanged = MenusController.LoadIcons(HasGet, true);
        HasChanged = sceneController.LoadIcons(HasGet, true);
    }

    void Update()
    {
        if (firstFrame == false)
        {
            firstFrame = true;
            return;
        }

        HasGet = TryGetJoystick();

        if (HasChanged != HasGet)
        {
            HasChanged = MenusController.LoadIcons(HasGet, false);
            HasChanged = sceneController.LoadIcons(HasGet, false);
        }
    }

    public void GetJoystick()
    {
        HasChanged = sceneController.LoadIcons(HasGet, true);
    }

    /// <summary>
    /// Tries Get a connected joystick
    /// </summary>
    /// <returns>True if find a joystick</returns>
    public bool TryGetJoystick()
    {
        if (Input.GetJoystickNames() != null && Input.GetJoystickNames().Length > 0)
        {
            if (Input.GetJoystickNames()[0] != "")
            {
                controlName = Input.GetJoystickNames()[0];
                return ControlConnected = true;
            }
            else
            {
                return ControlConnected = false;
            }
        }
        else
        {
            return ControlConnected = false;
        }
    }

    /// <summary>
    /// Set the current Icon Controller in Scene
    /// </summary>
    /// <param name="newController">The Controller</param>
    public void SetScene(ControllerIcons newController)
    {
        sceneController = newController;
    }
}
