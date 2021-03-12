using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ControllerIcons : MonoBehaviour
{
    [SerializeField] Image[] BButtons = new Image[0];
    [SerializeField] Image[] AButtons = new Image[0];
    [SerializeField] Image[] XButtons = new Image[0];
    [SerializeField] Image[] YButtons = new Image[0];
    [SerializeField] Image[] RbButtons = new Image[0];
    [SerializeField] Image[] LbButtons = new Image[0];
    [SerializeField] Image[] RightArrowButtons = new Image[0];
    [SerializeField] Image[] LeftArrowButtons = new Image[0];
    [SerializeField] Image[] UpArrowButtons = new Image[0];
    [SerializeField] Image[] DownArrowButtons = new Image[0];
    [SerializeField] Image[] SpritesToDisable = new Image[0];

    Sprite BSprites = null;
    Sprite ASprites = null;
    Sprite XSprites = null;
    Sprite YSprites = null;
    Sprite RbSprites = null;
    Sprite LbSprites = null;
    Sprite RightArrowSprites = null;
    Sprite LeftArrowSprites = null;
    Sprite UpArrowSprites = null;
    Sprite DownArrowSprites = null;

    /// <summary>
    /// Set the current scene Icon Controller
    /// </summary>
    void Awake()
    {
        if (CheckConnectedControll.instance != null)
        {
            CheckConnectedControll.instance.SetScene(this);
            CheckConnectedControll.instance.GetJoystick();
        }
    }

    /// <summary>
    /// Load the icons
    /// </summary>
    /// <param name="hasGet"></param>
    /// <param name="force"></param>
    /// <returns></returns>
    public bool LoadIcons(bool hasGet, bool force)
    {
        if (hasGet)
        {
            BSprites = CheckConnectedControll.instance.BButtonSprite;
            ASprites = CheckConnectedControll.instance.AButtonSprite;
            XSprites = CheckConnectedControll.instance.XButtonSprite;
            YSprites = CheckConnectedControll.instance.YButtonSprite;
            RbSprites = CheckConnectedControll.instance.RbButtonSprite;
            LbSprites = CheckConnectedControll.instance.LbButtonSprite;
            RightArrowSprites = CheckConnectedControll.instance.RArrowButtonSprite;
            LeftArrowSprites = CheckConnectedControll.instance.LArrowButtonSprite;
            UpArrowSprites = CheckConnectedControll.instance.UpArrowButtonSprite;
            DownArrowSprites = CheckConnectedControll.instance.DoArrowButtonSprite;
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(null);

            BSprites = CheckConnectedControll.instance.EmptySprite;
            ASprites = CheckConnectedControll.instance.EmptySprite;
            XSprites = CheckConnectedControll.instance.EmptySprite;
            YSprites = CheckConnectedControll.instance.EmptySprite;
            RbSprites = CheckConnectedControll.instance.EmptySprite;
            LbSprites = CheckConnectedControll.instance.EmptySprite;
            RightArrowSprites = CheckConnectedControll.instance.EmptySprite;
            LeftArrowSprites = CheckConnectedControll.instance.EmptySprite;
            UpArrowSprites = CheckConnectedControll.instance.EmptySprite;
            DownArrowSprites = CheckConnectedControll.instance.EmptySprite;
        }

        if (force == false)
        {
            
        }

        if (CameraManager.instance != null)
        {
            CameraManager.instance.CursorActive(!hasGet);
        }
        else
        {
            Cursor.visible = !hasGet;
            if (Cursor.visible)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        for (int i = 0; i < BButtons.Length; i++)
        {
            BButtons[i].sprite = BSprites;
        }

        for (int i = 0; i < AButtons.Length; i++)
        {
            AButtons[i].sprite = ASprites;
        }

        for (int i = 0; i < XButtons.Length; i++)
        {
            XButtons[i].sprite = XSprites;
        }

        for (int i = 0; i < YButtons.Length; i++)
        {
            YButtons[i].sprite = YSprites;
        }

        for (int i = 0; i < RbButtons.Length; i++)
        {
            RbButtons[i].sprite = RbSprites;
        }

        for (int i = 0; i < LbButtons.Length; i++)
        {
            LbButtons[i].sprite = LbSprites;
        }

        for (int i = 0; i < RightArrowButtons.Length; i++)
        {
            RightArrowButtons[i].sprite = RightArrowSprites;
        }

        for (int i = 0; i < LeftArrowButtons.Length; i++)
        {
            LeftArrowButtons[i].sprite = LeftArrowSprites;
        }

        for (int i = 0; i < UpArrowButtons.Length; i++)
        {
            UpArrowButtons[i].sprite = UpArrowSprites;
        }

        for (int i = 0; i < DownArrowButtons.Length; i++)
        {
            DownArrowButtons[i].sprite = DownArrowSprites;
        }

        for (int i = 0; i < SpritesToDisable.Length; i++)
        {
            SpritesToDisable[i].enabled = !hasGet;
        }
        
        return hasGet;
    }
}
