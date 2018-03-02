using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Foundation;

[System.Serializable]
public class InstallObjectCommand : Command
{
    public NodeSideInfo _nodeInfo;
    public int PrefabIndex { get; set; }

    public InstallObjectCommand(NodeSideInfo info)
    {
        _nodeInfo = info;
        _cost = 2;
    }

    public override bool Execute(GameObject target)
    {
        if (base.Execute(target))
        {
            Level.LevelCreator.Instance.InstallObjectAtNode(_nodeInfo, PrefabIndex);
            return true;
        }
        else
        {
            return false;
        }
    }

}
