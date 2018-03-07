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

        [SerializeField]
        private List<GameObject> _installPrefabList;
        public List<GameObject> InstallPrefabList { get { return _installPrefabList; } }

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

        //레벨을 위함이 아니다...
        public GameObject CreateObjectAtNode(NodeSideInfo info, GameObject prefab, Vector3 offset, Quaternion rotation)
        {
            if (!Object.ReferenceEquals(info, null))
            {
                //오브젝트를 설치하고 노드의 속성을 업데이트 한다.
                GameObject go = Instantiate(prefab, info.GetWorldPosition() + offset, rotation);
                return go;
            }
            return null;
        }

        public void InstallObjectAtNode(NodeSideInfo info, int prefabIndex, Vector3 offset, Quaternion rotation)
        {
            if (prefabIndex > _installPrefabList.Count)
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
                    GameObject go = Instantiate(_installPrefabList[prefabIndex], info.GetWorldPosition() + offset, rotation);
                    LevelObject levelObject = go.GetComponent<LevelObject>();
                    levelObject.Offset = offset;
                    levelObject.Rotation = rotation;
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

    }
}
