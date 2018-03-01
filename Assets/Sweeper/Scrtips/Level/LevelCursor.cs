using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Utils;

namespace Level
{
    public class LevelCursor : MonoBehaviour
    {
        [HideInInspector]
        public NodeSideInfo _selectingInfo;
        private Timer _rightMouseClickTimer;

        private void Start()
        {
            _rightMouseClickTimer = new Timer(0.5f); 
        }


        private void Update()
        {
            LocateCursor();

            /////////////////
            if (Input.GetMouseButton(1) && _rightMouseClickTimer.Tick(Time.deltaTime))
            {
                if (_selectingInfo != null && 
                    !RadialMenu._opened &&
                    BoardManager.GetNodeSideInfoAtMouse(ref _selectingInfo))
                {
                    RadialMenu.Show(_selectingInfo);
                }
            }

            if (RadialMenu._opened &&
                Input.GetMouseButtonUp(1))
            {
                RadialMenu.Shut();
            }
            /////////////////


        }

        private void LocateCursor()
        {
            Quaternion rotation = transform.rotation;
            Vector3 worldPosition = transform.position;

            if (BoardManager.GetNodeSideInfoAtMouse(ref _selectingInfo))
            {
                worldPosition = _selectingInfo.GetWorldPosition();
                rotation = BoardManager.SideToRotation(_selectingInfo._side);
            }
            transform.position = worldPosition;
            transform.rotation = rotation;
        }


    }
}
