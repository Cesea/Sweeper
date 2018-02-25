using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Level
{
    public class LevelCursor : MonoBehaviour
    {
        void Start()
        {

        }

        void Update()
        {
            Quaternion rotation = transform.rotation;
            Vector3 worldPosition = transform.position;

            NodeSideInfo info = new NodeSideInfo();
            if (BoardManager.GetNodeSideInfoAtMouse(ref info))
            {
                //TODO;
                //RadialMenu.Show(Input.mousePosition, _selectingNode);
                if (info._node == null )
                {
                    Debug.Log("aaaaa");
                }
                worldPosition = info.GetWorldPosition();
                rotation = BoardManager.SideToRotation(info._side);
            }
            transform.position = worldPosition;
            transform.rotation = rotation;
        }

        void UpdatePosition()
        {

        }
    }
}
