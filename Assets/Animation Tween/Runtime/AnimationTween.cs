using UnityEngine;

public class AnimationTween : MonoBehaviour
{
    public bool executeOnStart;
    public float duration = 1;
    public bool loop;
    public bool testMode;
    public bool playAgain = true;
    [SerializeField] bool active;

    bool currentActive = false;

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

    [SerializeField] Vector3 originalPosition;
    [SerializeField] Quaternion originalRotation;
    [SerializeField] Vector3 originalScale;

    [SerializeField] Vector3 finalPosition;
    [SerializeField] Vector3 finalRotation;
    [SerializeField] Vector3 finalScale;

    public bool updatePosition;
    public bool updateRotation;
    public bool updateScale;

    [SerializeField] float timer;
    [SerializeField] bool inverted;

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
            Play();
    }
    public void Play()
    {
        if (playAgain == false && currentActive) return;

        finalPosition = transform.localPosition;
        finalRotation = transform.localEulerAngles;
        finalScale = transform.localScale;
        timer = 0;
        if (testMode) active = true; else enabled = true;

        currentActive = true;
    }
    public void ToggleOnlyOn(bool value)
    {
        if (value == false)
        {
            currentActive = false;
            return;
        }
        if (playAgain == false && currentActive) return;

        finalPosition = transform.localPosition;
        finalRotation = transform.localEulerAngles;
        finalScale = transform.localScale;
        timer = 0;
        if (testMode) active = true; else enabled = true;

        currentActive = true;
    }
    public void Toggle(bool setActive)
    {
        if (playAgain == false && currentActive) return;

        finalPosition = transform.localPosition;
        finalRotation = transform.localEulerAngles;
        finalScale = transform.localScale;

        inverted = !setActive;

        if (testMode) active = true; else enabled = true;

        currentActive = true;
    }

    void Update()
    {
        if (testMode)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Toggle(true);
            }
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                Toggle(false);
            }
        }

        if (testMode && active == false) return;

        timer += Time.deltaTime * (inverted ? -1 : 1) / duration;
        timer = Mathf.Clamp01(timer);

        if (updatePosition)
            UpdatePosition();
        if (updateRotation)
            UpdateRotation();
        if (updateScale)
            UpdateScale();

        if (inverted && timer <= 0)
        {
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
        else if (!inverted && timer >= 1)
        {
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
            finalPosition.y = originalPosition.y + ((position.desiredVector.y - originalPosition.y) * position.yAxis.Evaluate(timer));
        if (position.zAxisActive)
            finalPosition.z = originalPosition.z + ((position.desiredVector.z - originalPosition.z) * position.zAxis.Evaluate(timer));

        transform.localPosition = finalPosition;
    }
    void UpdateRotation()
    {
        if (rotation.xAxisActive)
            finalRotation.x = originalRotation.eulerAngles.x + ((rotation.desiredVector.x - originalRotation.eulerAngles.x) * rotation.xAxis.Evaluate(timer));
        if (rotation.yAxisActive)
            finalRotation.y = originalRotation.eulerAngles.y + ((rotation.desiredVector.y - originalRotation.eulerAngles.y) * rotation.yAxis.Evaluate(timer));
        if (rotation.zAxisActive)
            finalRotation.z = originalRotation.eulerAngles.z + ((rotation.desiredVector.z - originalRotation.eulerAngles.z) * rotation.zAxis.Evaluate(timer));

        transform.localRotation = Quaternion.Euler(finalRotation);
    }
    void UpdateScale()
    {
        if (scale.xAxisActive)
            finalScale.x = originalScale.x + ((scale.desiredVector.x - originalScale.x) * scale.xAxis.Evaluate(timer));
        if (scale.yAxisActive)
            finalScale.y = originalScale.y + ((scale.desiredVector.y - originalScale.y) * scale.yAxis.Evaluate(timer));
        if (scale.zAxisActive)
            finalScale.z = originalScale.z + ((scale.desiredVector.z - originalScale.z) * scale.zAxis.Evaluate(timer));

        transform.localScale = finalScale;
    }
}
