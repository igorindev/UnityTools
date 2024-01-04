using TMPro;
using UnityEngine;

public class IntVariableSetter : MonoBehaviour
{
    public TextMeshProUGUI TMP;
    public Variable<int> variable;

    private void OnEnable()
    {
        variable.onValueChange += UpdateTMP;
        UpdateTMP(variable.value);
    }

    private void OnDisable()
    {
        variable.onValueChange -= UpdateTMP;
    }

    public void UpdateTMP(int value)
    {
        TMP.text = value.ToString();
    }
}