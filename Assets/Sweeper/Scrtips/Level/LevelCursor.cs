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
            Side hitSide = Side.Top;
            Quaternion rotation = transform.rotation;
            Node node = BoardManager.GetNodeAtMouse(ref hitSide);

            Vector3 worldPosition = transform.position;
            if (node != null)
            {
                //TODO;
                //RadialMenu.Show(Input.mousePosition, _selectingNode);
                worldPosition = node.GetWorldPositionBySide(hitSide);
                //if (hitSide != Side.Top && hitSide != Side.Bottom)
                //{
                //    worldPosition += BoardManager.SideToVector3Offset(hitSide);
                //}
                rotation = BoardManager.SideToRotation(hitSide);
                Debug.Log(rotation);
            }
            transform.position = worldPosition;
            transform.rotation = rotation;


        }

        void UpdatePosition()
        {

        }
    }
}
