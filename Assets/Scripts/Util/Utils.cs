using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static float SignedAngle(Vector3 a, Vector3 b)
    {
        float angle = Vector3.Angle(a, b);
        Vector3 cross = Vector3.Cross(a, b);
        if (cross.y < 0) angle = -angle;

        return angle;
    }

    public static int WeightedRandom(int[] probabilities)
    {
        int index = 0;
        int sum = 0;
        for (int i = 0; i < probabilities.Length; i++)
        {
            sum += probabilities[i];
        }

        int rand = UnityEngine.Random.Range(1, sum);
        //Debug.LogFormat("Sum: {0}, Chance: {1}", sum, rand);

        int top = 0;
        for (int j = 0; j < probabilities.Length; j++)
        {
            top += probabilities[j];
            //Debug.LogFormat("> Top: {0}", top);
            if (rand < top)
            {
                index = j;
                //Debug.LogFormat("Index is {0}", j);
                break;
            }
        }

        return index;
    }
}
