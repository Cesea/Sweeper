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

    public override void Execute(GameObject target)
    {
        base.Execute(target);
        Level.LevelCreator.Instance.InstallObjectAtNode(_nodeInfo, PrefabIndex);
    }

}
