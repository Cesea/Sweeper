using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Foundation;

public static class CommandBuilder 
{
    public static List<Command> BuildCommands(BoardObject subject, NodeSideInfo target)
    {
        List<Command> result = new List<Command>();

        result.Add(new InspectCommand());

        BoardMovementManager movementManager = subject.GetComponent<BoardMovementManager>();
        Vector3Int deltaGridToPlayer = movementManager._sittingNodeInfo._node.BoardPosition - target._node.BoardPosition;
        if (movementManager != null)
        {
            result.Add(new MoveCommand(target));
        }

        if (deltaGridToPlayer.x >= -1 && deltaGridToPlayer.x <= 1 &&
           deltaGridToPlayer.y >= -1 && deltaGridToPlayer.y <= 1 )
        {
            if (target._node.GetInstalledObjectAt(target._side) != null)
            {
                result.Add(new InstallObjectCommand(GameStateManager.Instance._dangerSignPrefab, target));
            }
        }
        subject._commandBuffer = result;
        return result;
    }
}
