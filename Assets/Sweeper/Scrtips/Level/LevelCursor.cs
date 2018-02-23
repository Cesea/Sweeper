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
            Vector3 hitPos = Vector3.zero;
            Node node = BoardManager.GetNodeAtMouse(ref hitSide, ref hitPos);

            Vector3 worldPosition = transform.position;
            if (node != null)
            {
                //TODO;
                //RadialMenu.Show(Input.mousePosition, _selectingNode);
                worldPosition = hitPos;
            }
            transform.position = worldPosition;


        }

        void UpdatePosition()
        {

        }
    }
}
