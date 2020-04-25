using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArrayUtilities
{

    /// <summary>
    /// Splits array into smaller arrays of given size.
    /// The split array count is rounded down
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="arr">Array to be split</param>
    /// <param name="newSize">Size of the array to be split</param>
    /// <returns></returns>
    public static T[][] SplitArray<T>(T[] arr, float newSize)
    {
        //How many sequences of length newSize can be created from observationSymbols array
        int lngth = UnityEngine.Mathf.FloorToInt(arr.Length / newSize);
        T[][] seq = new T[lngth][];

        List<T> temp = new List<T>();
        int seqArrIndex = 0;
        for (int i = 0; i < arr.Length; i++)
        {
            temp.Add(arr[i]);
            if (temp.Count >= newSize)
            {
                seq[seqArrIndex] = temp.ToArray();
                temp.Clear();
                seqArrIndex++;
            }
        }
        return seq;
    }
}
