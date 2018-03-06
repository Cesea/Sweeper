using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Level
{
    public class LevelObject : MonoBehaviour
    {
        [SerializeField]
        private bool _isHazard = true;
        [SerializeField]
        private bool _isWalkable = true;
        [SerializeField]
        private bool _canInstallSide = false;

        public bool IsHazard { get { return _isHazard; } set { _isHazard = value; } }
        public bool IsWalkable { get { return _isWalkable; } set { _isWalkable = value; } }
        public bool CanInstallSide { get { return _canInstallSide; } set { _canInstallSide = value; } }

        private NodeSideInfo _sittingInfo = null;
        public NodeSideInfo SittingInfo
        {
            get { return _sittingInfo; }
            set { _sittingInfo = value; }
        }

        private Vector3 _offset;
        public Vector3 Offset { get { return _offset; }  set { _offset = value; } }


        private float _rotation;
        public float Rotation { get { return _rotation; }  set { _rotation = value; } }

        [HideInInspector]
        public int _prefabIndex;

        public LevelObjectSaveData ToSaveData()
        {
            LevelObjectSaveData result = new LevelObjectSaveData();
            result._isHazard = _isHazard;
            result._isWalkable = _isWalkable;
            result._canInstallSide = _canInstallSide;
            result._prefabIndex = _prefabIndex;

            result._offsetX = Offset.x;
            result._offsetY = Offset.y;
            result._offsetZ = Offset.z;
            result._rotation = Rotation;

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

        public float _offsetX;
        public float _offsetY;
        public float _offsetZ;

        public Quaternion _rotation;

        public Side _installedSide;
        public bool _isHazard = true;
        public bool _isWalkable = true;
        public bool _canInstallSide = false;
    }
}
