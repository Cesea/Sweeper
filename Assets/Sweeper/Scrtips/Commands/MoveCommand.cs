using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Foundation;

[System.Serializable]
public class MoveCommand : Command
{
    private NodeSideInfo _targetNode;

    public MoveCommand(NodeSideInfo target)
    {
        _targetNode = target;
        _cost = 1;
    } 

    public override bool Execute(GameObject target)
    {
        if (base.Execute(target))
        {
            BoardMovementManager manager = target.GetComponent<BoardMovementManager>();
            manager.StartFindPath(_targetNode);
            return true;
        }
        else
        {
            return false;
        }
    }
}
