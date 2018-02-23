using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Level
{
    public class LevelObject : MonoBehaviour
    {
        public bool IsHazard = true;
        public bool IsWalkable = true;
        public bool CanInstallVertical = false;

        [HideInInspector]
        public Node SittingNode = null;
        [HideInInspector]
        public Side InstalledSide;

        [HideInInspector]
        public int PrefabIndex;

        public LevelObjectSaveData ToSaveData()
        {
            LevelObjectSaveData result = new LevelObjectSaveData();
            result.IsHazard = IsHazard;
            result.IsWalkable = IsWalkable;
            result.PrefabIndex = PrefabIndex;
            result.InstalledSide = InstalledSide;

            if (SittingNode != null)
            {
                result.BoardX = SittingNode.X;
                result.BoardY = SittingNode.Y;
                result.BoardZ = SittingNode.Z;

                result.YRotation = transform.rotation.eulerAngles.y;
            }
            return result;
        }
    }

    [System.Serializable]
    public class LevelObjectSaveData
    {
        public int PrefabIndex;
        public int BoardX;
        public int BoardY;
        public int BoardZ;

        public float YRotation;

        public Side InstalledSide;
        public bool IsHazard = true;
        public bool IsWalkable = true;
    }
}
