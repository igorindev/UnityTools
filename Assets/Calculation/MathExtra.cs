using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathExtra
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
}



