using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Foundation;

[System.Serializable]
public class InstallObjectCommand : Command
{
    public GameObject _installObjectPrefab;

    public NodeSideInfo _nodeInfo;

    public InstallObjectCommand(GameObject prefab, NodeSideInfo info)
    {
        _installObjectPrefab = prefab;
        _nodeInfo = info;
        _cost = 2;
    }

    public override void Execute(GameObject target)
    {
        base.Execute(target);
        GameObject instantiaedObject = GameObject.Instantiate(_installObjectPrefab);
        instantiaedObject.name = _installObjectPrefab.name;
        instantiaedObject.transform.position = _nodeInfo.GetWorldPosition();
        //instantiaedObject.transform.SetParent(target.transform);
    }

}
