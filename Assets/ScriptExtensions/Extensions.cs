using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Extensions
{
    public static string ToHex(this Color color)
    {
        return string.Format("#{0:X2}{1:X2}{2:X2}", ToByte(color.r), ToByte(color.g), ToByte(color.b));
    }
    static byte ToByte(float f)
    {
        f = Mathf.Clamp01(f);
        return (byte)(f * 255);
    }

    public static string Color(this string text, Color color)
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
    public static int GetRandomInRangeExclusive(this Vector2Int range)
    {
        return Random.Range(range.x, range.y);
    }
    public static int GetRandomInRange(this Vector2Int range)
    {
        return Random.Range(range.x, range.y + 1);
    }
    public static float GetRandomInRange(this Vector2 range)
    {
        return Random.Range(range.x, range.y);
    }

    public static float RandomNegateThisValue(this float value)
    {
        return value * (Random.Range(0, 2) == 0 ? 1 : -1);
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
    public static T AllocateFromPool<T>(this T[] array, ref int count)
    {
        count += 1;
        count %= array.Length;

        return array[count];
    }
    public static T AllocateFromPool<T>(this List<T> list, ref int count)
    {
        count += 1;
        count %= list.Count;

        return list[count];
    }

    public static string ToTimer(this float time)
    {
        TimeSpan ts = TimeSpan.FromSeconds(time);
        return string.Format("{0:00}:{1:00}", (int)ts.TotalMinutes, (int)ts.Seconds);
    }
}