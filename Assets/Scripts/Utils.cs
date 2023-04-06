using UnityEngine;
using System.Collections;

public static class Utils
{
    public static float AngleCalculator(float angleCalculatorX, float angleCalculatorY, bool usingNormals = false)
    {
        float angleCalculatorResult;

        if (usingNormals)
        {
            angleCalculatorResult = Mathf.Atan2(angleCalculatorX, angleCalculatorY) * -Mathf.Rad2Deg;
        }
        else
        {
            angleCalculatorResult = Mathf.Atan2(angleCalculatorY, angleCalculatorX) * Mathf.Rad2Deg;
        }

        return (720f + angleCalculatorResult) % 360f;
    }
}
