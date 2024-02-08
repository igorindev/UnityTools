using UnityEngine;

public struct MathExtension
{
    /// <summary>
    /// Return true if the target is in distance and the direction passed is in the angle
    /// </summary>
    /// <param name="me"></param>
    /// <param name="target"></param>
    /// <param name="origin"></param>
    /// <param name="direction"></param>
    /// <param name="interactAngle"></param>
    /// <param name="interactDistance"></param>
    /// <returns></returns>
    public static bool InRange(Vector3 me, Vector3 target, Vector3 origin, Vector3 direction, float interactAngle, float interactDistance)
    {
        if (Vector3.Distance(me, target) <= interactDistance)
        {
            float cosAngle = Vector3.Dot((target - origin).normalized, direction);
            float angle = Mathf.Acos(cosAngle) * Mathf.Rad2Deg;

            if (angle < interactAngle)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Return true if the target is inside the angle
    /// </summary>
    /// <param name="targetPos"></param>
    /// <param name="origin"></param>
    /// <param name="direction"></param>
    /// <param name="interactAngle"></param>
    /// <returns></returns>
    public static bool TargetDirection(Vector3 targetPos, Vector3 origin, Vector3 direction, float interactAngle)
    {
        float cosAngle = Vector3.Dot((targetPos - origin).normalized, direction);
        float angle = Mathf.Acos(cosAngle) * Mathf.Rad2Deg;

        if (angle < interactAngle)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// Convert a value from 2 limits to new 2 limits - Ex: 0 - 1 to 2 - 3;
    /// </summary>
    /// <param name="value"></param>
    /// <param name="oldMax"></param>
    /// <param name="oldMin"></param>
    /// <param name="newMax"></param>
    /// <param name="newMin"></param>
    /// <returns></returns>
    public static float ConvertRanges(float value, float oldMax, float oldMin, float newMax, float newMin)
    {
        return ((value - oldMin) * (newMax - newMin) / (oldMax - oldMin)) + newMin;
    }
    /// <summary>
    /// Convert a value from 2 limits to new 2 limits - Ex: 0 - 1 to 2 - 3;
    /// </summary>
    /// <param name="value"></param>
    /// <param name="oldMax"></param>
    /// <param name="oldMin"></param>
    /// <param name="newMax"></param>
    /// <param name="newMin"></param>
    /// <returns></returns>
    public static float ConvertRanges(float value, Vector2 oldValues, Vector2 newValues)
    {
        return ((value - oldValues.x) * (newValues.y - newValues.x) / (oldValues.y - oldValues.x)) + newValues.x;
    }
    /// <summary>
    /// Return a Random point inside a circle projected in horizontal
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="minRadius"></param>
    /// <param name="maxRadius"></param>
    /// <returns></returns>
    public static Vector3 RandomPointInCircle(Vector3 origin, float minRadius, float maxRadius)
    {
        Vector2 randomPos = Random.insideUnitCircle.normalized * Random.Range(minRadius, maxRadius);
        return origin + new Vector3(randomPos.x, 0, randomPos.y);
    }

    public static bool Probability(int chance)
    {
        return Random.Range(1, 101) <= chance;
    }
    public static bool Probability(int chance, out int value)
    {
        value = Random.Range(1, 101);
        return value <= chance;
    }

    /// <summary>
    /// Return a value between 0 and 100
    /// </summary>
    /// <returns></returns>
    public static float GetRandomIntPercentage()
    {
        return Random.Range(0, 101);
    }
    /// <summary>
    /// Return a value between 0 and 100
    /// </summary>
    /// <returns></returns>
    public static float GetRandomFloatPercentage()
    {
        return Random.Range(0f, 100f);
    }

    public static float WrapAngle(float angle)
    {
        angle %= 360;
        if (angle > 180)
            return angle - 360;

        return angle;
    }
    public static float UnwrapAngle(float angle)
    {
        if (angle >= 0)
            return angle;

        angle = -angle % 360;

        return 360 - angle;
    }

    public static float Pulse(float time, float maxFrequency = 5, float frequencyMultiplier = 1)
    {
        float pi = 3.14f;
        maxFrequency *= frequencyMultiplier; // Frequency in Hz
        return 0.5f * (1 + Mathf.Sin(2 * pi * maxFrequency * time));
    }

    public static Vector3 SampleParabola(Vector3 start, Vector3 end, Vector3 dir, float height, float t)
    {
        float parabolicT = t * 2 - 1;
        Vector3 travelDirection = end - start;
        Vector3 result = start + t * travelDirection;
        result += dir * (-parabolicT * parabolicT + 1) * height;
        return result;
    }

    // Use this method to know the progress of an element between two points taking into account the direction of the movement
    // Returns 0 at the start position, 1 at the end position, and negative numbers if the movements goes in the opposite direction
    public static float PathProgress(Vector2 startPosition, Vector2 endPosition, Vector2 currentPosition)
    {
        Vector2 totalDisplacement = endPosition - startPosition;
        Vector2 currentDisplacement = currentPosition - startPosition;

        return Vector2.Dot(currentDisplacement, totalDisplacement) / totalDisplacement.sqrMagnitude;
    }

    public static float GetTargetAngle(Transform self, Vector3 incomingDir)
    {
        // Flatten to plane
        var otherDir = new Vector3(-incomingDir.x, 0f, -incomingDir.z);
        var playerFwd = Vector3.ProjectOnPlane(self.forward, Vector3.up);

        // Direction between player fwd and incoming object
        var angle = Vector3.SignedAngle(playerFwd, otherDir, Vector3.up);

        return angle;
    }

    public static void RandomVector(out Vector3 vector, Vector3 min, Vector3 max)
    {
        vector = new Vector3(Random.Range(min.x, max.x),
                             Random.Range(min.y, max.y),
                             Random.Range(min.z, max.z));
    }
}