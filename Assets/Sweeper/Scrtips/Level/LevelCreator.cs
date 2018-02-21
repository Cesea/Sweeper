using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCreator : MonoBehaviour
{
    public BoardObject _player;

    public List<GameObject> _installObjects;
    public int _selectingIndex = 0;

    public bool CanBuild = false;

    private float _yRotation = 0;

    private void Update()
    {
        HandleInput();
        
    }

    private void HandleInput()
    {
        if (CanBuild)
        {
            if (Input.GetMouseButtonDown(0))
            {
                InstallObject(_selectingIndex);
            }

            if (Input.GetMouseButton(1))
            {
                AddRotation();
            }
        }
    }

    public void InstallObject(int prefabIndex)
    {
        if (prefabIndex > _installObjects.Count - 1)
        {
            return;
        }
        GameObject objectUnder = GameStateManager.GetObjectUnderneathMouse();
        Node node = GameStateManager.Instance.CurrentBoard.GetNodeAt(objectUnder.transform.position);
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

    public void DestroyObject()
    {

    }

    private void AddRotation()
    {
        _yRotation += 90.0f;
        if (_yRotation >= 360.0f)
        {
            _yRotation -= 360.0f;
        }
    }

}
