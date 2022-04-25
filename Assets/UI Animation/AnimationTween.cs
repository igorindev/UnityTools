using UnityEngine;

public class AnimationTween : MonoBehaviour
{
    public bool executeOnStart;
    public float duration = 1;
    public bool loop;

    public AnimationAxis position;
    public AnimationAxis rotation;
    public AnimationAxis scale;

    [System.Serializable]
    public struct AnimationAxis
    {
        public Vector3 desiredVector;

        public bool xAxisActive;
        public AnimationCurve xAxis;

        public bool yAxisActive;
        public AnimationCurve yAxis;

        public bool zAxisActive;
        public AnimationCurve zAxis;
    }

    Vector3 originalPosition;
    Quaternion originalRotation;
    Vector3 originalScale;

    Vector3 finalPosition;
    Vector3 finalRotation;
    Vector3 finalScale;

    public bool updatePosition;
    public bool updateRotation;
    public bool updateScale;

    float timer;
    bool inverted;

    [ContextMenu("Play")]
    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalScale = transform.localScale;

        timer = 0;

        enabled = false;

        if (executeOnStart)
            Play(true);
    }

    public void Play(bool setActive)
    {
        inverted = setActive == false;
        enabled = true;
    }

    void Update()
    {
        timer += Time.deltaTime * (inverted ? -1 : 1) / duration;

        if (updatePosition)
            UpdatePosition();
        if (updateRotation)
            UpdateRotation();
        if (updateScale)
            UpdateScale();

        if (inverted && timer <= 0)
        {
            timer = 0;
            enabled = false;
        }
        else if (timer > 1)
        {
            timer = 1;
            enabled = false;
        }
    }

    void UpdatePosition()
    {
        if (position.xAxisActive)
            finalPosition.x = Mathf.Lerp(originalPosition.x, originalPosition.x - (position.xAxis.Evaluate(timer) * (originalPosition.x - position.desiredVector.x)), timer);
        if (position.yAxisActive)
            finalPosition.y = Mathf.Lerp(originalPosition.y, originalPosition.y - (position.yAxis.Evaluate(timer) * (originalPosition.y - position.desiredVector.y)), timer);
        if (position.zAxisActive)
            finalPosition.z = Mathf.Lerp(originalPosition.z, originalPosition.z - (position.zAxis.Evaluate(timer) * (originalPosition.z - position.desiredVector.z)), timer);

        transform.position = finalPosition;
    }
    void UpdateRotation()
    {
        if (rotation.xAxisActive)
            finalRotation.x = Mathf.Lerp(originalRotation.eulerAngles.x, rotation.xAxis.Evaluate(timer) * rotation.desiredVector.x, timer);
        if (rotation.xAxisActive)
            finalRotation.y = Mathf.Lerp(originalRotation.eulerAngles.y, rotation.yAxis.Evaluate(timer) * rotation.desiredVector.y, timer);
        if (rotation.xAxisActive)
            finalRotation.z = Mathf.Lerp(originalRotation.eulerAngles.z, rotation.zAxis.Evaluate(timer) * rotation.desiredVector.z, timer);

        transform.rotation = Quaternion.Euler(finalRotation);
    }
    void UpdateScale()
    {
        if (scale.xAxisActive)
            finalScale.x = Mathf.Lerp(originalScale.x, scale.yAxis.Evaluate(timer) * scale.desiredVector.x, timer);
        if (scale.yAxisActive)
            finalScale.y = Mathf.Lerp(originalScale.y, scale.yAxis.Evaluate(timer) * scale.desiredVector.y, timer);
        if (scale.zAxisActive)
            finalScale.z = Mathf.Lerp(originalScale.z, scale.yAxis.Evaluate(timer) * scale.desiredVector.z, timer);

        transform.localScale = finalScale;
    }
}
