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
        public CursorManager _cursorManager;
        public LevelCreateCursor _createCursor;

        private GameObject _previewObject;

        public List<GameObject> _installObjectPrefabs;

        private int _selectingIndex = 0;
        public int SelectingIndex { get { return _selectingIndex; } }
        private int _prevIndex = 0; 

        public bool _canBuild = false;

        private float _yRotation = 0;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (!LevelCreateMenu.Opened)
                {
                    LevelCreateMenu.Show();
                    _cursorManager.ChangeState(CursorManager.CursorState.Create);
                }
                else
                {
                    LevelCreateMenu.Shut();
                    _cursorManager.ChangeState(CursorManager.CursorState.Select);
                }
            }
        }

        private void OnEnable()
        {
            EventManager.Instance.AddListener<Events.LevelCreatorMenuEvent>(OnLevelCreatorMenuEvent);
        }

        private void OnDisable()
        {
            EventManager.Instance.RemoveListener<Events.LevelCreatorMenuEvent>(OnLevelCreatorMenuEvent);
        }

        private void LateUpdate()
        {
            if (_canBuild)
            {
                if (_selectingIndex != _prevIndex)
                {
                    RecreateObjectToInstall();
                }
                MakeObjectToInstallFollowCursor();
                _prevIndex = _selectingIndex;
            }
        }

        //레벨을 위함이 아니다...
        public GameObject CreateObjectAtNode(NodeSideInfo info, GameObject prefab)
        {
            if (!Object.ReferenceEquals(info, null))
            {
                //오브젝트를 설치하고 노드의 속성을 업데이트 한다.
                Quaternion rot = Quaternion.Euler(0, _yRotation, 0);
                GameObject go = Instantiate(prefab, info.GetWorldPosition(), rot);
                return go;
            }
            return null;
        }


        public void InstallObjectAtNode(NodeSideInfo info, int prefabIndex = -1)
        {
            if (prefabIndex == -1)
            {
                prefabIndex = _selectingIndex;
            }

            if (prefabIndex > _installObjectPrefabs.Count - 1)
            {
                return;
            }
            if (!Object.ReferenceEquals(info, null))
            {
                if (info.InstalledObject != null)
                {
                    //설치하려는 자리에 이미 노드가 있다.... 어떻게 처리 할까??
                }
                else
                {
                    //오브젝트를 설치하고 노드의 속성을 업데이트 한다.
                    Quaternion rot = Quaternion.Euler(0, _yRotation, 0);
                    GameObject go = Instantiate(_installObjectPrefabs[prefabIndex], info.GetWorldPosition(), rot);
                    LevelObject levelObject = go.GetComponent<LevelObject>();
                    levelObject._prefabIndex = prefabIndex;
                    info.InstalledObject = go;
                }
            }
        }

        public void DestroyObjectAtNode(NodeSideInfo info)
        {
            if (!Object.ReferenceEquals(info, null)&&
                info.InstalledObject != null)
            {
                //LevelObject levelObject = info._node.GetInstalledObjectAt(info._side).GetComponent<LevelObject>();
                Destroy(info.InstalledObject);
                info.InstalledObject = null;
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
                _previewObject.transform.position = _createCursor.transform.position;
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

        public void OnLevelCreatorMenuEvent(Events.LevelCreatorMenuEvent e)
        {
            if (e.Opened)
            {
                RecreateObjectToInstall();
                _canBuild = true;
                //_levelCursor.ChangeState(LevelCursor.CursorState.Create);
            }
            else
            {
                GameObject.Destroy(_previewObject);
                _canBuild = false;
                //_levelCursor.ChangeState(LevelCursor.CursorState.Select);
            }
        }
    }
}
