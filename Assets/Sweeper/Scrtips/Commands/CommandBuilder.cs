using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Foundation;

public static class CommandBuilder 
{
    public static List<Command> BuildCommands(Node targetNode, Side side)
    {
        List<Command> result = new List<Command>();

        result.Add(new InspectCommand());

        Vector3Int objectCellPosition = new Vector3Int(targetNode.X, targetNode.Y, targetNode.Z);
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
            result.Add(new JumpCommand(-deltaGridToPlayer.x, -deltaGridToPlayer.y, -deltaGridToPlayer.y));
        }

        //In build range

        if (deltaGridToPlayer.x >= -1 && deltaGridToPlayer.x <= 1 &&
           deltaGridToPlayer.y >= -1 && deltaGridToPlayer.y <= 1)
        {
            if (targetNode.GetInstalledObjectAt(side))
            {
                result.Add(new InstallObjectCommand(GameStateManager.Instance._dangerSignPrefab, objectCellPosition.x, objectCellPosition.y));
            }
        }
        return result;
    }
}
