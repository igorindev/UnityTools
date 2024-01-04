using TMPro;
using UnityEngine;

public class CurrentMaxIntVariableSetter : MonoBehaviour
{
    public string splitCharacter = "/";
    public TextMeshProUGUI TMP;
    public IntVariable current;
    public IntVariable max;

    private void OnEnable()
    {
        current.onValueChange += UpdateTMP;
        UpdateTMP(current.value);
    }

    private void OnDisable()
    {
        current.onValueChange -= UpdateTMP;
    }

    public void UpdateTMP(int value)
    {
        TMP.text = $"{value}{splitCharacter}{max.value}";
    }
}