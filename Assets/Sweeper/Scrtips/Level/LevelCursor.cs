using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Utils;

namespace Level
{
    public class LevelCursor : MonoBehaviour
    {
        private NodeSideInfo _selectingInfo;
        private Timer _rightMouseClickTimer;

        private bool _showMenu;

        private void Start()
        {
            _rightMouseClickTimer = new Timer(0.5f); 
        }

        private void OnEnable()
        {
            EventManager.Instance.AddListener<Events.RadialShutEvent>(OnRadialShutEvent); 
        }
        private void OnDisable()
        {
            EventManager.Instance.RemoveListener<Events.RadialShutEvent>(OnRadialShutEvent); 
        }
        public void OnRadialShutEvent(Events.RadialShutEvent e)
        {
            _showMenu = false;
        }

        void Update()
        {
            LocateCursor();

            if (Input.GetMouseButton(1) && _rightMouseClickTimer.Tick(Time.deltaTime))
            {
                if (_selectingInfo != null && 
                    !_showMenu &&
                    BoardManager.GetNodeSideInfoAtMouse(ref _selectingInfo))
                {
                    _showMenu = true;
                    RadialMenu.Show(_selectingInfo);
                }
            }

            if (_showMenu &&
                Input.GetMouseButtonUp(1))
            {
                RadialMenu.Shut();
            }
        }

        void LocateCursor()
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
