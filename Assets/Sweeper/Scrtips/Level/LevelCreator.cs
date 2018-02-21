using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Foundation;

namespace Level
{
    public class LevelCreator : SingletonBase<LevelCreator>
    {
        public BoardObject _player;
        public LevelCursor _buildCursor;

        private GameObject _objectToInstall;

        public List<GameObject> _installObjects;

        public int _selectingIndex = 0;
        private int _prevIndex = 0; 
        

        public bool CanBuild = false;

        private float _yRotation = 0;

        private void Start()
        {
            RecreateObjectToInstall();
        }

        private void Update()
        {
            if (_selectingIndex != _prevIndex)
            {
                RecreateObjectToInstall();
            }

            MakeObjectToInstallFollowCursor();

            HandleInput();

            _prevIndex = _selectingIndex;
        }

        private void HandleInput()
        {
            if (CanBuild)
            {
                if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftShift))
                {
                    DestroyObjectAtMousePosition();
                    return;
                }

                if (Input.GetMouseButtonDown(0))
                {
                    InstallObjectAtMousePosition(_selectingIndex);
                    return;
                }

                if (Input.GetMouseButton(1))
                {
                    AddRotation();
                    return;
                }
            }
        }

        public void InstallObjectAtBoardPosition(int x, int y, int prefabIndex)
        {
            if (prefabIndex > _installObjects.Count - 1)
            {
                return;
            }
            Node node = GameStateManager.Instance.CurrentBoard.GetNodeAt(x, y);
            if (node != null)
            {
                if (node.InstalledObject != null)
                {
                    //설치하려는 자리에 이미 노드가 있다.... 어떻게 처리 할까??
                }
                else
                {
                    //오브젝트를 설치하고 노드의 속성을 업데이트 한다.
                    Quaternion rot = Quaternion.Euler(0, _yRotation, 0);
                    GameObject go = Instantiate(_installObjects[prefabIndex], node.WorldPosition, rot);
                    LevelObject levelObject = go.GetComponent<LevelObject>();
                    if (levelObject.IsHazard)
                    {
                        node.IsHazard = true;
                    }
                    if (!levelObject.IsWalkable)
                    {
                        node.IsWalkable = false;
                    }

                    node.InstalledObject = go;
                }
            }
        }

        public void DestroyObjectAtBoardPosition(int x, int y)
        {
            Node node = GameStateManager.Instance.CurrentBoard.GetNodeAt(x, y);
            if (node != null &&
                node.InstalledObject != null)
            {
                LevelObject levelObject = node.InstalledObject.GetComponent<LevelObject>();
                Destroy(node.InstalledObject);
                node.IsHazard = false;
            }
        }

        public void InstallObjectAtMousePosition(int prefabIndex)
        {
            if (prefabIndex > _installObjects.Count - 1)
            {
                return;
            }
            GameObject objectUnder = GameStateManager.GetObjectUnderneathMouse();
            Vector2Int boardPos = BoardManager.WorldPosToBoardPos(objectUnder.transform.position);
            InstallObjectAtBoardPosition(boardPos.x, boardPos.y, prefabIndex);
        }

        public void DestroyObjectAtMousePosition()
        {
            GameObject objectUnder = GameStateManager.GetObjectUnderneathMouse();
            Vector2Int boardPos = BoardManager.WorldPosToBoardPos(objectUnder.transform.position);
            DestroyObjectAtBoardPosition(boardPos.x, boardPos.y);
        }

        private void AddRotation()
        {
            _yRotation += 90.0f;
            if (_yRotation >= 360.0f)
            {
                _yRotation -= 360.0f;
            }
        }

        private void RecreateObjectToInstall()
        {
            if (_objectToInstall != null)
            {
                Destroy(_objectToInstall);
                _objectToInstall = null;
            }

            if (_objectToInstall == null)
            {
                _objectToInstall = Instantiate(_installObjects[_selectingIndex]);
            }
        }

        private void MakeObjectToInstallFollowCursor()
        {
            if (_objectToInstall != null)
            {
                _objectToInstall.transform.position = _buildCursor.transform.position;
            }
        }

    }
}
