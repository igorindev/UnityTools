using UnityEngine;

public partial struct MathExtension
{
    //Useful for pitch, scale, zoom. 
    //time = Random.value (0, 1)
    //a = 0.8f, b = 1.2f
    public static float Eerp(float a, float b, float time)
    {
        return a * Mathf.Exp(time * Mathf.Log(b / a));
    }
}
