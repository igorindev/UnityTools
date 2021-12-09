using UnityEngine;
using UnityEngine.Animations;

public class IKLimitation : MonoBehaviour
{
    public float visibilityAngle;
    public Transform target;
    public enum Type
    {
        LookAt,
        AimIk,
        IKControll,
    }
    public Type type;

    [Header("IKs")]
    public LookAtConstraint lookAtConstraint;
    public AimConstraint aimConstraint;
    public IKControll iKControll;

    // Update is called once per frame
    void Update()
    {
        if (type == Type.LookAt)
        {
            if (MathExtension.TargetDirection(target.position, transform.position, transform.forward, visibilityAngle))
            {
                lookAtConstraint.weight = Mathf.Lerp(lookAtConstraint.weight, 1, Time.deltaTime);
            }
            else
            {
                lookAtConstraint.weight = Mathf.Lerp(lookAtConstraint.weight, 0, Time.deltaTime);
            }
        }
        else if (type == Type.AimIk)
        {
            if (MathExtension.TargetDirection(target.position, transform.position, transform.forward, visibilityAngle))
            {
                aimConstraint.weight = Mathf.Lerp(aimConstraint.weight, 1, Time.deltaTime);
            }
            else
            {
                aimConstraint.weight = Mathf.Lerp(aimConstraint.weight, 0, Time.deltaTime);
            }
        }
        else if (type == Type.IKControll)
        {
            iKControll.enabled = MathExtension.TargetDirection(target.position, transform.position, transform.forward, visibilityAngle);
        }
        
    }
}
