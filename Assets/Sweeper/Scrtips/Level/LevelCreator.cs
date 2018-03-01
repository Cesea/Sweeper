using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;

using Foundation;
using Utils;

namespace Level
{
    public class LevelCreator : SingletonBase<LevelCreator>
    {
        public BoardObject _player;
        public LevelCursor _buildCursor;

        private GameObject _previewObject;

        public List<GameObject> _installObjectPrefabs;

        public int _selectingIndex = 0;
        private int _prevIndex = 0; 
        
        public bool _canBuild = false;

        private float _yRotation = 0;

        private void Start()
        {
        }

        private void Update()
        {
            _canBuild = LevelCreateMenu._opened;

            if (!_canBuild)
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    LevelCreateMenu.Show();
                    RecreateObjectToInstall();
                    return;
                }
            }

            if (_canBuild)
            {

                HandleInput();

                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    LevelCreateMenu.Shut();
                    GameObject.Destroy(_previewObject);
                    return;
                }

                if (_selectingIndex != _prevIndex)
                {
                    RecreateObjectToInstall();
                }

                MakeObjectToInstallFollowCursor();

                _prevIndex = _selectingIndex;
            }
        }

        private void HandleInput()
        {
            if (_canBuild)
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }

                if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftShift))
                {
                    DestroyObjectAtNode(_buildCursor._selectingInfo._node, _buildCursor._selectingInfo._side);
                    return;
                }

                if (Input.GetMouseButtonDown(0))
                {
                    InstallObjectAtNode(_buildCursor._selectingInfo._node, _buildCursor._selectingInfo._side, _selectingIndex);
                    return;
                }
            }
        }

        public void InstallObjectAtNode(Node node, Side side, int prefabIndex)
        {
            if (prefabIndex > _installObjectPrefabs.Count - 1)
            {
                return;
            }
            if (node != null)
            {
                if (node.GetInstalledObjectAt(side) != null)
                {
                    //설치하려는 자리에 이미 노드가 있다.... 어떻게 처리 할까??
                }
                else
                {
                    //오브젝트를 설치하고 노드의 속성을 업데이트 한다.
                    Quaternion rot = Quaternion.Euler(0, _yRotation, 0);
                    GameObject go = Instantiate(_installObjectPrefabs[prefabIndex], node.GetWorldPositionBySide(side), rot);
                    LevelObject levelObject = go.GetComponent<LevelObject>();
                    levelObject._prefabIndex = prefabIndex;
                    levelObject._sittingNode = node;
                    levelObject._installedSide = side;
                    if (levelObject._isHazard)
                    {
                        node.IsHazard = true;
                    }
                    if (!levelObject._isWalkable)
                    {
                        node.IsPassable = false;
                    }
                    node.SetInstalledObjectAt(side, go);
                }
            }
        }

        public void DestroyObjectAtNode(Node node, Side side)
        {
            if (node != null &&
                node.GetInstalledObjectAt(side) != null)
            {
                //LevelObject levelObject = info._node.GetInstalledObjectAt(info._side).GetComponent<LevelObject>();

                Destroy(node.GetInstalledObjectAt(side));
                node.SetInstalledObjectAt(side, null);
            }
        }

        private void RecreateObjectToInstall()
        {
            if (_previewObject != null)
            {
                Destroy(_previewObject);
                _previewObject = null;
            }

            if (_previewObject == null)
            {
                _previewObject = Instantiate(_installObjectPrefabs[_selectingIndex]);
            }
        }

        private void MakeObjectToInstallFollowCursor()
        {
            if (_previewObject != null)
            {
                _previewObject.transform.position = _buildCursor.transform.position;
            }
        }

        public void SetSelectingIndex(int i)
        {
            if (i >= _installObjectPrefabs.Count)
            {
                return;
            }
            _selectingIndex = i;
        }
    }
}
