using UnityEngine;

public class SledFollow : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Object that pulls the sled.")]
    Transform _leader;

    [SerializeField]
    [Tooltip("Position on the sled's local z+ axis where its tow rope is attached")]
    float _attachmentDistance = 1;

    [SerializeField]
    [Tooltip("How far from the attachment point can the leader go before pulling?")]
    float _ropeLength = 1;


    Vector3 _attachmentPosition;

    void Start()
    {
        Vector3 localAttachment = new Vector3(0, 0, _attachmentDistance);
        _attachmentPosition = transform.TransformPoint(localAttachment);
    }

    void LateUpdate()
    {
        // When the leader moves further from the attachment point
        // than the length of the rope, move the attachment point toward
        // the leader until the rope is perfectly taut, not over-stretched.
        Vector3 displacement = _attachmentPosition - _leader.position;
        Vector3 rope = Vector3.ClampMagnitude(displacement, _ropeLength);
        _attachmentPosition = _leader.position + rope;

        // If you never scale the sled, you can skip this line.
        float leverLength = _attachmentDistance * transform.localScale.z;

        // When the attachment point moves, move this object as though
        // connected to it by a rigid lever that can't lengthen OR contract.
        displacement = transform.position - _attachmentPosition;
        Vector3 lever = displacement.normalized * leverLength;
        transform.position = _attachmentPosition + lever;

        // Rotate so the attachment point is along our forward vector.
        transform.rotation = Quaternion.LookRotation(-displacement);
    }
}