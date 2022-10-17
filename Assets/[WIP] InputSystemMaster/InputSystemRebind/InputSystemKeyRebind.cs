using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[Serializable]
public struct RebinderSchema
{
    public string schema;
    public List<Rebinder> rebinders;
}
[Serializable]
public struct Rebinder
{
    public string action;
    public RebindHolder bind;
}
[Serializable]
public class RebindHolder
{
    public TextMeshProUGUI bindText;
    public TextMeshProUGUI inputNameText;
    public Button button;
    public int bindIndex;
    public int compositionIndex;
    public string schema;
    public string path;
    internal InputAction inputAction;
}

public class InputSystemKeyRebind : MonoBehaviour
{
    [InputControl(layout = "Button")]
    public string keyboardCancelInput;
    [InputControl(layout = "Button")]
    public string controllerCancelInput;

    public PlayerInput playerInput;
    public RebinderSchema[] rebindReceiver;
    public string SAVE_BINDINGS_PATH;

    public Button[] inputs;

    RebindHolder currentRebinding = new RebindHolder();
    InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    void OnValidate()
    {
        ReadBindings();
    }

    void ReadBindings()
    {
        if (playerInput && playerInput.actions)
        {
            int compositeCount = 0;
            int bindCount = 0;
            string lastAction = "";

            var controlSchemes = playerInput.actions.controlSchemes.ToArray();
            rebindReceiver = new RebinderSchema[controlSchemes.Length];

            for (int i = 0; i < controlSchemes.Length; i++)
            {
                rebindReceiver[i].schema = controlSchemes[i].name;
                rebindReceiver[i].rebinders = new List<Rebinder>();
            }

            List<InputBinding> allInputs = new List<InputBinding>();
            foreach (InputBinding item in playerInput.actions.bindings)
            {
                if (string.IsNullOrEmpty(item.effectivePath) == false && item.isComposite == false)
                {
                    allInputs.Add(item);
                }
            };

            for (int i = 0; i < allInputs.Count; i++)
            {
                compositeCount = allInputs[i].isPartOfComposite ? compositeCount + 1 : 0;

                bindCount = allInputs[i].action == lastAction && allInputs[i].isPartOfComposite == false ? bindCount + 1 : 0;

                lastAction = allInputs[i].action;

                RebindHolder rebindHolder = new RebindHolder
                {
                    inputAction = playerInput.actions[allInputs[i].action],
                    path = allInputs[i].effectivePath,
                    schema = allInputs[i].groups,
                    compositionIndex = compositeCount,
                    bindIndex = bindCount,
                    button = inputs[i],
                    bindText = inputs[i].GetComponentsInChildren<TextMeshProUGUI>()[0],
                    inputNameText = inputs[i].GetComponentsInChildren<TextMeshProUGUI>()[1]
                };

                inputs[i].onClick.AddListener(() => StartRebindingAction(rebindHolder));

                for (int j = 0; j < rebindReceiver.Length; j++)
                {
                    if (rebindReceiver[j].schema == allInputs[i].groups)
                    {
                        if (allInputs[i].isPartOfComposite)
                            lastAction += "/" + allInputs[i].name;

                        rebindReceiver[j].rebinders.Add(new Rebinder() { action = lastAction, bind = rebindHolder });
                        break;
                    }
                }
            }
        }
    }

    void Start()
    {
        ShowBindingsInText();
    }

    [ContextMenu("Save Bindings")]
    public void SaveUserRebinds()
    {
        var rebinds = playerInput.actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);
    }
    [ContextMenu("Load Bindings")]
    public void LoadUserRebinds()
    {
        var rebinds = PlayerPrefs.GetString("rebinds");
        playerInput.actions.LoadBindingOverridesFromJson(rebinds);
    }
    [ContextMenu("Reset Bindings")]
    public void ResetUserRebinds()
    {
        playerInput.actions.RemoveAllBindingOverrides();

        ShowBindingsInText();
    }

    void ShowBindingsInText()
    {
        for (int i = 0; i < rebindReceiver.Length; i++)
        {
            Color c = Random.ColorHSV();
            for (int j = 0; j < rebindReceiver[i].rebinders.Count; j++)
            {
                rebindReceiver[i].rebinders[j].bind.bindText.text = ConvertToTextSprite(rebindReceiver[i].rebinders[j].bind.path);
                rebindReceiver[i].rebinders[j].bind.inputNameText.text = rebindReceiver[i].rebinders[j].action;
                rebindReceiver[i].rebinders[j].bind.inputNameText.color = c;
            }
        }
    }

    void StartRebindingAction(RebindHolder action)
    {
        if (currentRebinding.button != null) return;

        action.bindText.text = "Waiting Input";

        action.inputAction.Disable();

        currentRebinding = action;

        //ALOW SAME KEY IN TWO PLACES? THROW ALERT
        //COMPOSITION NOT ALLOW REPETITION (none move)

        if (action.compositionIndex > 0)
            rebindingOperation = action.inputAction.PerformInteractiveRebinding(action.compositionIndex);
        else
            rebindingOperation = action.inputAction.PerformInteractiveRebinding(action.bindIndex);

        rebindingOperation.WithControlsExcluding("Mouse");
        rebindingOperation.WithControlsExcluding("Gamepad");
        rebindingOperation.WithControlsExcluding(keyboardCancelInput);
        rebindingOperation.WithCancelingThrough(keyboardCancelInput);
        rebindingOperation.OnMatchWaitForAnother(0.1f);
        rebindingOperation.OnCancel(RebindEndedAsCompletedOrCanceled);
        rebindingOperation.OnComplete(RebindEndedAsCompletedOrCanceled);
        rebindingOperation.Start();
    }
    void RebindEndedAsCompletedOrCanceled(InputActionRebindingExtensions.RebindingOperation obj)
    {
        rebindingOperation.Dispose();
        currentRebinding.inputAction.Enable();

        int bindingIndex;
        if (currentRebinding.compositionIndex > 0)
            bindingIndex = currentRebinding.compositionIndex;
        else
            bindingIndex = currentRebinding.bindIndex;

        currentRebinding.bindText.text = ConvertToTextSprite(currentRebinding.inputAction.bindings[bindingIndex].effectivePath);
        currentRebinding.path = currentRebinding.inputAction.bindings[bindingIndex].effectivePath;

        Dictionary<string, int> paths = new Dictionary<string, int>();

        int schemaIndex = 0;
        for (int i = 0; i < rebindReceiver.Length; i++)
        {
            if (rebindReceiver[i].schema == currentRebinding.schema)
            {
                schemaIndex = i;
                for (int j = 0; j < rebindReceiver[i].rebinders.Count; j++)
                {
                    string path = rebindReceiver[i].rebinders[j].bind.path;
                    if (paths.ContainsKey(path))
                    {
                        paths[path] += 1;
                    }
                    else
                    {
                        paths[path] = 1;
                    }
                }
                break;
            }
        }

        for (int j = 0; j < rebindReceiver[schemaIndex].rebinders.Count; j++)
        {
            string path = rebindReceiver[schemaIndex].rebinders[j].bind.path;
            Button b = rebindReceiver[schemaIndex].rebinders[j].bind.button;
            if (paths.ContainsKey(path))
            {
                if (paths[path] == 1)
                {
                    b.GetComponent<Image>().color = Color.white;
                }
                else
                {
                    b.GetComponent<Image>().color = Color.red;
                }
            }
            else
            {
                b.GetComponent<Image>().color = Color.white;
            }
        }

        currentRebinding = new RebindHolder();
        PlayerInputController.Instance.inputSystemCheckDevice.UpdateAllPaths();
    }

    string ConvertToTextSprite(string path)
    {
        path = InputControlPath.ToHumanReadableString(path, InputControlPath.HumanReadableStringOptions.OmitDevice | InputControlPath.HumanReadableStringOptions.UseShortNames);
        return "<sprite name=" + path + ">";
    }
}
