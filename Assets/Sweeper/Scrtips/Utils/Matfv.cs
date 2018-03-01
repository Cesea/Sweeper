using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Matfv
{
    public static Vector3 Bezier(Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float oneMinusT = 1.0f - t;
        Vector3 result = (p1 * oneMinusT * oneMinusT) +
                            (2.0f * p2 * oneMinusT * t) +
                            (p3 * t * t * t);
        return result;
    }

    public static Vector3 Vector3Round(Vector3 v)
    {
        return new Vector3(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z));
    }

    public static Vector2 Vector2Round(Vector2 v)
    {
        return new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));
    }

}
