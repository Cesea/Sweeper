using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Foundation;

public static class CommandBuilder 
{
    public static List<Command> BuildCommands(GameObject obj)
    {
        List<Command> result = new List<Command>();

        result.Add(new InspectCommand());


        Vector3Int objectCellPosition = obj.transform.position.ToVector3Int();
        Vector3Int deltaGridToPlayer = GameStateManager.Instance.Player.transform.position.ToVector3Int() - objectCellPosition;

        if ((deltaGridToPlayer.x == 0 && Mathf.Abs(deltaGridToPlayer.z) == 1) ||
            (Mathf.Abs(deltaGridToPlayer.x) == 1 && deltaGridToPlayer.z == 0))
        {
            result.Add(new WalkCommand(-deltaGridToPlayer.x, -deltaGridToPlayer.z));
        }

        //In jump range
        if (deltaGridToPlayer.x >= -2 && deltaGridToPlayer.x <= 2 &&
           deltaGridToPlayer.z >= -2 && deltaGridToPlayer.z <= 2)
        {
            //Cell cellAtObject = GameStateManager.Instance.BoardManager.CurrentBoard.GetCellAt((int)obj.transform.position.x, (int)obj.transform.position.z);
            //if (cellAtObject.InstalledObject == null)
            //{
            //}
            result.Add(new JumpCommand(-deltaGridToPlayer.x, -deltaGridToPlayer.y, -deltaGridToPlayer.y));
        }

        //In build range

        //if (deltaGridToPlayer.x >= -1 && deltaGridToPlayer.x <= 1 &&
        //   deltaGridToPlayer.y >= -1 && deltaGridToPlayer.y <= 1)
        //{
        //    Node cellAtObject = GameStateManager.Instance.BoardManager.CurrentBoard.GetNodeAt((int)obj.transform.position.x, (int)obj.transform.position.z);
        //    if (cellAtObject.InstalledObject == null)
        //    {
        //        result.Add(new InstallObjectCommand(GameStateManager.Instance._dangerSignPrefab, objectCellPosition.x, objectCellPosition.y));
        //    }
        //}


        return result;
    }
}
