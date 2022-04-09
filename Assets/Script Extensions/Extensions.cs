using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Extensions
{
    public static Color ToColor(this string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out Color newCol))
        {
            return newCol;
        }
        else
        {
            Debug.LogError("Error: Can not convert color");
            return Color.white;
        }
    }
    public static string ToHex(this Color color)
    {
        return string.Format("#{0:X2}{1:X2}{2:X2}", ToByte(color.r), ToByte(color.g), ToByte(color.b));
    }
    static byte ToByte(float f)
    {
        f = Mathf.Clamp01(f);
        return (byte)(f * 255);
    }

    public static string AddColor(this string text, Color color)
    {
        return string.Format("<color={0}>{1}</color>", color.ToHex(), text);
    }

    public static T GetRandom<T>(this T[] array)
    {
        return array[Random.Range(0, array.Length)];
    }
    public static T GetRandom<T>(this List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }
    public static T GetRandomAndRemove<T>(this List<T> list)
    {
        T item = list[Random.Range(0, list.Count)];
        list.Remove(item);
        return item;
    }
    public static float GetRandomInRange(this Vector2 range)
    {
        return Random.Range(range.x, range.y);
    }
    public static int GetRandomInRange(this Vector2Int range)
    {
        return Random.Range(range.x, range.y + 1);
    }
    public static int GetRandomInRangeExclusive(this Vector2Int range)
    {
        return Random.Range(range.x, range.y);
    }

    public static float RandomPositiveOrNegative(this float value)
    {
        return value * (Random.Range(0, 2) == 0 ? 1 : -1);
    }
    public static int RandomPositiveOrNegative(this int value)
    {
        return value * (Random.Range(0, 2) == 0 ? 1 : -1);
    }

    public static float Float2Db(this float value)
    {
        return Mathf.Log10(value) * 20;
    }
    public static float DbToFloat(this float value)
    {
        value *= 0.05f;
        return Mathf.Pow(10, value);
    }

    public static Vector3 ZeroX(this Vector3 vector)
    {
        vector.x = 0;
        return vector;
    }
    public static Vector3 ZeroY(this Vector3 vector)
    {
        vector.y = 0;
        return vector;
    }
    public static Vector3 ZeroZ(this Vector3 vector)
    {
        vector.z = 0;
        return vector;
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        for (var i = list.Count - 1; i > 1; i--)
        {
            var j = Random.Range(0, i + 1);
            var value = list[j];
            list[j] = list[i];
            list[i] = value;
        }
    }

    public static void DestroyAllChildren(this Transform transform)
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            UnityEngine.Object.Destroy(transform.GetChild(i).gameObject);
        }
    }

    public static bool CompareLists<T>(this List<T> list, List<T> list2)
    {
        return list == list2;
    }
    public static bool CompareArray<T>(this T[] array, T[] array2)
    {
        return array == array2;
    }
    public static void AddUnique<T>(this List<T> list, T item)
    {
        if (!list.Contains(item))
            list.Add(item);
    }
    public static void AddUnique<T, J>(this Dictionary<T, J> dictionary, T key, J value)
    {
        if (!dictionary.ContainsKey(key))
            dictionary.Add(key, value);
    }

    public static string ToTimer(this float time)
    {
        TimeSpan ts = TimeSpan.FromSeconds(time);
        return string.Format("{0:00}:{1:00}", (int)ts.TotalMinutes, (int)ts.Seconds);
    }
}