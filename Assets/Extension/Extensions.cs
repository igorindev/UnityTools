using System.Collections.Generic;
using UnityEngine;

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
}
