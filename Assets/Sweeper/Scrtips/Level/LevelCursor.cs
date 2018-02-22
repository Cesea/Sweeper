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
            //Vector3 worldPosition = transform.position;
            //Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            //RaycastHit hitInfo;
            //if (GameStateManager.Instance.MouseCollisionBox.Raycast(camRay, out hitInfo, 1000.0f))
            //{
            //    worldPosition = hitInfo.point;
            //    Vector3Int boardPos = BoardManager.WorldPosToBoardPos(worldPosition);
            //    worldPosition = BoardManager.BoardPosToWorldPos(boardPos);
            //}
            //transform.position = worldPosition;
        }

        void UpdatePosition()
        {

        }
    }
}
