using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Matfv
{
    public static Vector3 Bezier(Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float oneMinusT = 1.0f - t;
        Vector3 result = (p1 * oneMinusT * oneMinusT) +
                            (2.0f * p2 * oneMinusT * t) +
                            (p3 * t * t * t);
        return result;
    }

}
