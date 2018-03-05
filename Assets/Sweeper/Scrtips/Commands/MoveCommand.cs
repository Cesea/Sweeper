using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Foundation;

[System.Serializable]
public class MoveCommand : Command
{
    private List<NodeSideInfo> _targetPaths;

    public MoveCommand(List<NodeSideInfo> targets)
    {
        _targetPaths = targets;
        _cost = _targetPaths.Count;
    } 

    public override bool Execute(GameObject target)
    {
        if (base.Execute(target) && _targetPaths.Count != 0)
        {
            BoardMovementManager manager = target.GetComponent<BoardMovementManager>();
            if (_targetPaths.Count == 1)
            {
                manager.StartFindPath(_targetPaths[0]);
            }
            else
            {
                manager.GiveAndFollowPathData(_targetPaths.ToArray());
            }
            _targetPaths.Clear();
            return true;
        }
        else
        {
            _targetPaths.Clear();
            return false;
        }
    }
}
