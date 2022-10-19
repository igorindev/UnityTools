using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScreen : MonoBehaviour
{
    public void FillOneCell(Image imageToFill)
    {
        imageToFill.fillAmount += 0.1f;
    }

    public void MinusOneCell(Image imageToFill)
    {
        imageToFill.fillAmount -= 0.1f;
    }
}
