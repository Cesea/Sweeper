using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static Vector2Int WorldPosToCellPos(this Transform trans)
    {
        return new Vector2Int((int)trans.position.x, (int)trans.position.z);
    }

}
