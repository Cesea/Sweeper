using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Level
{
    public class LevelObject : MonoBehaviour
    {
        public bool _isHazard = true;
        public bool _isWalkable = true;
        public bool _canInstallSide = false;

        [HideInInspector]
        public Node _sittingNode = null;
        [HideInInspector]
        public Side _installedSide;

        [HideInInspector]
        public int _prefabIndex;

        public LevelObjectSaveData ToSaveData()
        {
            LevelObjectSaveData result = new LevelObjectSaveData();
            result._isHazard = _isHazard;
            result._isWalkable = _isWalkable;
            result._prefabIndex = _prefabIndex;
            result._installedSide = _installedSide;

            if (_sittingNode != null)
            {
                result._boardX = _sittingNode.X;
                result._boardY = _sittingNode.Y;
                result._boardZ = _sittingNode.Z;

            }
            return result;
        }
    }

    [System.Serializable]
    public class LevelObjectSaveData
    {
        public int _prefabIndex;
        public int _boardX;
        public int _boardY;
        public int _boardZ;

        public Side _installedSide;
        public bool _isHazard = true;
        public bool _isWalkable = true;
    }
}
