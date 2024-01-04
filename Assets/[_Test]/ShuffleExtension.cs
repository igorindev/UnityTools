using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

// Shuffle using the Fisher-Yates shuffle algorithm: https://www.dotnetperls.com/fisher-yates-shuffle
public static class ShuffleExtension
{
    public static void Shuffle<T>(this T[] array)
    {
        int n = array.Length;

        for (int i = 0; i < n - 1; i++)
        {
            int swapIndex = i + Random.Range(0, n - i);

            (array[i], array[swapIndex]) = (array[swapIndex], array[i]);
        }
    }

    public static void Shuffle<T>(this List<T> array)
    {
        int n = array.Count;

        for (int i = 0; i < n - 1; i++)
        {
            int swapIndex = i + Random.Range(0, n - i);

            (array[i], array[swapIndex]) = (array[swapIndex], array[i]);
        }
    }

    public static void Shuffle<T>(this Span<T> array)
    {
        int n = array.Length;

        for (int i = 0; i < n - 1; i++)
        {
            int swapIndex = i + Random.Range(0, n - i);

            (array[i], array[swapIndex]) = (array[swapIndex], array[i]);
        }
    }
}
