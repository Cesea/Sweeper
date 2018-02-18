using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Foundation;

public class CommandBuilder : MonoBehaviour
{
    public Command[] BuildCommands()
    {
        List<Command> result = new List<Command>();

        Vector3 deltaToPlayer = GameStateManager.Instance.Player.transform.position - transform.position;
        Vector2Int deltaGridToPlayer = new Vector2Int((int)deltaToPlayer.x, (int)deltaToPlayer.z);
        if ((deltaGridToPlayer.x == 0 && Mathf.Abs(deltaGridToPlayer.y) == 1) ||
            (Mathf.Abs(deltaGridToPlayer.x) == 1 && deltaGridToPlayer.y == 0))
        {
            result.Add(new MoveCommand(deltaGridToPlayer.x, deltaGridToPlayer.y));
        }
        return result.ToArray();
    }
}
