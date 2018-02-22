using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static Vector3Int ToVector3Int(this Vector3 position, int xDelta = 0, int yDelta = 0, int  zDelta = 0)
    {
        return new Vector3Int((int)position.x + xDelta, (int)position.y + yDelta, (int)position.z + zDelta);
    }

}
