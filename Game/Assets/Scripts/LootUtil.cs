using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootUtil
{

    public static float[] getRandomWeights(int bins, float sumOfWeights)
    {
        if (sumOfWeights > bins) sumOfWeights = (float)bins;
        var weights = new float[bins];
        var sum = 0.0f;
        for (int i = 0; i < bins; i++)
        {
            var weight = Random.Range(sumOfWeights / bins, 1.0f);
            weights[i] = weight;
            sum += weight;
        }
        for (int i = 0; i < bins; i++)
        {
            weights[i] = weights[i] / sum * sumOfWeights;
        }
        return weights;
    }
}
