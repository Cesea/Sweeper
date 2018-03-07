using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObjectCommand : Foundation.Command
{
    public NodeSideInfo _nodeInfo;

    public DestroyObjectCommand(NodeSideInfo info)
    {
        _nodeInfo = info;
    }

    public override bool Execute(GameObject target)
    {
        if (base.Execute(target))
        {
            Level.LevelCreator.Instance.DestroyObjectAtNode(_nodeInfo);
            return true;
        }
        else
        {
            return false;
        }
    }
}
