using UnityEngine;

public class AnimationTween : MonoBehaviour
{
    public bool executeOnStart;
    public float duration = 1;
    public bool loop;
    public bool testMode;
    bool active;

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

    public AnimationAxis position;
    public AnimationAxis rotation;
    public AnimationAxis scale;

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

    void Reset()
    {
        position = new AnimationAxis()
        {
            desiredVector = Vector3.zero,
            xAxisActive = false,
            yAxisActive = false,
            zAxisActive = false,
            xAxis = new AnimationCurve(new Keyframe[2]
            {
                new Keyframe(0, 0), new Keyframe(1, 1),
            }),
            yAxis = new AnimationCurve(new Keyframe[2]
            {
                new Keyframe(0, 0), new Keyframe(1, 1),
            }),
            zAxis = new AnimationCurve(new Keyframe[2]
            {
                new Keyframe(0, 0), new Keyframe(1, 1),
            })
        };
        rotation = new AnimationAxis()
        {
            desiredVector = Vector3.zero,
            xAxisActive = false,
            yAxisActive = false,
            zAxisActive = false,
            xAxis = new AnimationCurve(new Keyframe[2]
            {
                new Keyframe(0, 0), new Keyframe(1, 1),
            }),
            yAxis = new AnimationCurve(new Keyframe[2]
            {
                new Keyframe(0, 0), new Keyframe(1, 1),
            }),
            zAxis = new AnimationCurve(new Keyframe[2]
            {
                new Keyframe(0, 0), new Keyframe(1, 1),
            })
        };

        scale = new AnimationAxis()
        {
            desiredVector = Vector3.zero,
            xAxisActive = false,
            yAxisActive = false,
            zAxisActive = false,
            xAxis = new AnimationCurve(new Keyframe[2]
            {
                new Keyframe(0, 0), new Keyframe(1, 1)
            }),
            yAxis = new AnimationCurve(new Keyframe[2]
            {
                new Keyframe(0, 0), new Keyframe(1, 1),
            }),
            zAxis = new AnimationCurve(new Keyframe[2]
            {
                new Keyframe(0, 0), new Keyframe(1, 1),
            })
        };
    }

    void Start()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
        originalScale = transform.localScale;

        timer = 0;

        if (testMode) active = false; else enabled = false;

        if (executeOnStart)
            Play(true);
    }

    public void Play(bool setActive)
    {
        finalPosition = transform.localPosition;
        finalRotation = transform.localEulerAngles;
        finalScale = transform.localScale;

        inverted = setActive == false;
        if (testMode) active = true; else enabled = true;
    }

    void Update()
    {
        if (testMode)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Play(true);
            }
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                Play(false);
            }
        }

        if (testMode && active == false) return;

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
            if (loop)
                inverted = !inverted;
            else
            {
                if (testMode)
                    active = false;
                else
                    enabled = false;
            }
        }
        else if (!inverted && timer > 1)
        {
            timer = 1;
            if (loop)
                inverted = !inverted;
            else
            {
                if (testMode) 
                    active = false; 
                else 
                    enabled = false;
            }
        }
    }

    void UpdatePosition()
    {
        if (position.xAxisActive)
            finalPosition.x = originalPosition.x + ((position.desiredVector.x - originalPosition.x) * position.xAxis.Evaluate(timer));
        if (position.yAxisActive)
            finalPosition.y = originalPosition.y + ((position.desiredVector.y - originalPosition.y) * position.xAxis.Evaluate(timer));
        if (position.zAxisActive)
            finalPosition.z = originalPosition.z + ((position.desiredVector.z - originalPosition.z) * position.xAxis.Evaluate(timer));

        transform.localPosition = finalPosition;
    }
    void UpdateRotation()
    {
        if (rotation.xAxisActive)
            finalRotation.x = originalRotation.eulerAngles.x + ((rotation.desiredVector.x - originalRotation.eulerAngles.x) * rotation.yAxis.Evaluate(timer));
        if (rotation.yAxisActive)
            finalRotation.y = originalRotation.eulerAngles.y + ((rotation.desiredVector.y - originalRotation.eulerAngles.y) * rotation.yAxis.Evaluate(timer));
        if (rotation.zAxisActive)
            finalRotation.z = originalRotation.eulerAngles.z + ((rotation.desiredVector.z - originalRotation.eulerAngles.z) * rotation.yAxis.Evaluate(timer));

        transform.localRotation = Quaternion.Euler(finalRotation);
    }
    void UpdateScale()
    {
        if (scale.xAxisActive)
            finalScale.x = originalScale.x + ((scale.desiredVector.x - originalScale.x) * scale.xAxis.Evaluate(timer));
        if (scale.yAxisActive)
            finalScale.y = originalScale.y + ((scale.desiredVector.y - originalScale.y) * scale.xAxis.Evaluate(timer));
        if (scale.zAxisActive)
            finalScale.z = originalScale.z + ((scale.desiredVector.z - originalScale.z) * scale.xAxis.Evaluate(timer));

        transform.localScale = finalScale;
    }
}
