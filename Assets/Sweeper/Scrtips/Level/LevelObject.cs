using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Level
{
    public class LevelObject : MonoBehaviour
    {

        private bool _isHazard = true;
        private bool _isWalkable = true;
        private bool _canInstallSide = false;

        public bool IsHazard { get; set; }
        public bool IsWalkable { get; set; }
        public bool CanInstallSide { get; set; }

        [HideInInspector]
        private NodeSideInfo _sittingInfo = null;
        public NodeSideInfo SittingInfo
        {
            get { return _sittingInfo; }
            set { _sittingInfo = value; }
        }

        [HideInInspector]
        public int _prefabIndex;

        public LevelObjectSaveData ToSaveData()
        {
            LevelObjectSaveData result = new LevelObjectSaveData();
            result._isHazard = _isHazard;
            result._isWalkable = _isWalkable;
            result._prefabIndex = _prefabIndex;

            if (!Object.ReferenceEquals(_sittingInfo, null))
            {
                result._installedSide = _sittingInfo._side;
                result._boardX = _sittingInfo._node.X;
                result._boardY = _sittingInfo._node.Y;
                result._boardZ = _sittingInfo._node.Z;

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
