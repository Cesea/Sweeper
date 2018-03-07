using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static Vector3Int ToVector3Int(this Vector3 position)
    {
        return new Vector3Int((int)position.x, (int)position.y, (int)position.z);
    }

    public static Vector3 ToVector3(this Vector3Int position)
    {
        return new Vector3(position.x, position.y, position.z);
    }

    public static Vector3 ClampXYZ(this Vector3 v, float min, float max)
    {
        return new Vector3(Mathf.Clamp(v.x, min, max),
                            Mathf.Clamp(v.y, min, max),
                            Mathf.Clamp(v.z, min, max) );
    }

}
