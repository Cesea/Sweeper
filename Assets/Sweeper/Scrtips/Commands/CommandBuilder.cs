using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Foundation;

public class CommandBuilder 
{
    private static bool _pathDone = false;
    private static List<NodeSideInfo> _paths = new List<NodeSideInfo>();

    public static IEnumerator BuildCommands(BoardObject subject, NodeSideInfo target, System.Action OnDoneMethod)
    {
        List<Command> buffer = subject.CommandBuffer;
        buffer.Add(new InspectCommand());

        BoardMovementManager movementManager = subject.GetComponent<BoardMovementManager>();
        Vector3Int deltaGridToPlayer = movementManager._sittingNodeInfo._node.BoardPosition - target._node.BoardPosition;
        if (movementManager != null)
        {
            PathRequestManager.RequestPath(subject.SittingNode, target, OnPathFound);
            while (!_pathDone)
            {
                yield return null;
            }
            buffer.Add(new MoveCommand(_paths));
        }

        if (deltaGridToPlayer.x >= -1 && deltaGridToPlayer.x <= 1 &&
           deltaGridToPlayer.y >= -1 && deltaGridToPlayer.y <= 1 )
        {
            if (target.InstalledObject != null)
            {
                buffer.Add(new InstallObjectCommand(target));
            }
        }

        _pathDone = false;

        OnDoneMethod();

        yield return null;
    }

    public static void OnPathFound(NodeSideInfo[] path, bool success)
    {
        if (success)
        {
            _paths.AddRange(path);
        }
        _pathDone = true;
    }
}
