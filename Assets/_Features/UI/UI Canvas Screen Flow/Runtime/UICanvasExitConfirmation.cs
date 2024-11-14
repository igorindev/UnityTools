using System;
using UnityEngine;
using UnityEngine.UI;

public class UICanvasExitConfirmation : MonoBehaviour
{
    private event Action<bool> onConfirmed;
    private event Action<bool> onCancelled;

    [SerializeField] Button continueButton;
    [SerializeField] Button backButton;

    public void Setup(Action<bool> confirm, Action<bool> cancel, string message, string optionOne, string optionTwo)
    {
        onConfirmed = confirm;
        onCancelled = cancel;

        continueButton.onClick.AddListener(Continue);
        backButton.onClick.AddListener(Back);
    }

    private void OnDestroy()
    {
        continueButton.onClick.RemoveListener(Continue);
        backButton.onClick.RemoveListener(Back);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void Continue()
    {
        onConfirmed?.Invoke(true);
        Close();
    }

    private void Back()
    {
        onConfirmed?.Invoke(false);
        Close();
    }
}
