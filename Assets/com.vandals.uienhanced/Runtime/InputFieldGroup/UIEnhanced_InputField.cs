using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIEnhanced_InputField : TMP_InputField
{
    [SerializeField] bool m_reSelectAfterSubmit;
    UIEnhanced_InputFieldGroup m_group;

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        if (m_group)
            m_group.SelectInputField(this);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        if (m_group)
            m_group.SetFieldDisabled(this);
    }

    public void SetupGroup(UIEnhanced_InputFieldGroup group)
    {
        m_group = group;
        m_reSelectAfterSubmit = true;
    }

    public override void OnUpdateSelected(BaseEventData eventData)
    {
        base.OnUpdateSelected(eventData);

        if (m_reSelectAfterSubmit && isFocused == false)
        {
            ActivateInputField();
        }
    }
}
