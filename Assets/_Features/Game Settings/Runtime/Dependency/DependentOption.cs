using UnityEngine;
using UnityEngine.UI;

public abstract class DependentOption<TDependsOn, TDesiredType> : MonoBehaviour where TDependsOn : Selectable
{
    [SerializeField] protected RectTransform _contentView;
    [SerializeField] protected TDependsOn _dependsOn;
    [SerializeField] protected TDesiredType _desiredState;

    protected void Rebuild()
    {
        LayoutRebuilder.MarkLayoutForRebuild(_contentView);
    }
}
