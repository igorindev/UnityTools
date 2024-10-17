using UnityEngine;

public class AudioMinMaxSliderAttribute : PropertyAttribute
{

    public float min;
    public float max;

    public AudioMinMaxSliderAttribute(float min, float max)
    {
        this.min = min;
        this.max = max;
    }
}