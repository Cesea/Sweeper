using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Utils;

namespace Level
{
    public class LevelCursor : MonoBehaviour
    {
        public enum CursorState
        {
            Select,
            Move,
            Create
        }

        public static Vector2[,] _cursorUVs =
        { 
            { new Vector2(0.0f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.0f, 1.0f), new Vector2(0.5f,1.0f) },
            { new Vector2(0.5f, 0.5f), new Vector2(1.0f, 0.5f), new Vector2(0.5f, 1.0f), new Vector2(1.0f,1.0f) },
            { new Vector2(0.0f, 0.0f), new Vector2(0.5f, 0.0f), new Vector2(0.0f, 0.5f), new Vector2(0.5f,0.5f) },
            { new Vector2(0.5f, 0.0f), new Vector2(1.0f, 0.0f), new Vector2(0.5f, 0.5f), new Vector2(1.0f,0.5f) }
        };

        [HideInInspector]
        public NodeSideInfo _selectingInfo;
        private Timer _rightMouseClickTimer;

        private MeshRenderer _meshRenderer;

        private CursorState _state;

        private void Start()
        {
            _rightMouseClickTimer = new Timer(0.5f);

            _state = CursorState.Select;
        }

        private void Update()
        {
            LocateCursor();

            SelectUpdate();
            //switch (_state)
            //{
            //    case CursorState.Select:
            //        {
            //            SelectUpdate();
            //        } break;
            //    case CursorState.Move:
            //        {
            //            MoveUpdate();
            //        } break;
            //    case CursorState.Create:
            //        {
            //            CreateUpdate();
            //        } break;
            //}

        }

        private void SelectUpdate()
        {
            if (Input.GetMouseButton(0) &&
                Object.ReferenceEquals(_selectingInfo, null))
            {
                GameObject sittingObject = _selectingInfo._sittingObject;
                if (sittingObject != null)
                {
                    //GameStateManager.Instance._selectingObject  = sittingObject;
                }
            }

            if (Input.GetMouseButton(1) && _rightMouseClickTimer.Tick(Time.deltaTime))
            {
                if (!Object.ReferenceEquals(_selectingInfo, null) && 
                    !RadialMenu._opened &&
                    BoardManager.GetNodeSideInfoAtMouse(ref _selectingInfo))
                {
                    RadialMenu.Show(_selectingInfo);
                    _rightMouseClickTimer.Reset();
                }
            }

            if (RadialMenu._opened &&
                Input.GetMouseButtonUp(1))
            {
                RadialMenu.Shut();
            }
        }

        private void CreateUpdate()
        {
        }

        private void MoveUpdate()
        {

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
