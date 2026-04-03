using System;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    public static float DirectionToAngle(Vector3 dir)
    {
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }

    public static float GetNormalAngle(float angle)
    {
        return (angle % 360f + 360f) % 360f;
    }

    public static bool IsVisibleFromCamera(Transform obj, Camera cam)
    {
        Vector3 viewportPos = cam.WorldToViewportPoint(obj.position);

        return  viewportPos.z > 0 &&
                viewportPos.x >= 0 && viewportPos.x <= 1 &&
                viewportPos.y >= 0 && viewportPos.y <= 1;
    }

    public static List<int> FisherYatesShuffle(int min, int max, int count)
    {
        if (count > (max - min))
            throw new ArgumentException("Error: Out of range");

        int[] numbers = new int[max - min];
        for (int i = 0; i < numbers.Length; i++)
        {
            numbers[i] = min + i;
        }

        for (int i = 0; i < count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, numbers.Length);

            int temp = numbers[i];
            numbers[i] = numbers[randomIndex];
            numbers[randomIndex] = temp;
        }

        List<int> result = new List<int>(count);

        for (int i = 0; i < count; i++)
        {
            result.Add(numbers[i]);
        }

        return result;
    }
}
