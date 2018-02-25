﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Foundation;

[System.Serializable]
public class InstallObjectCommand : Command
{
    public GameObject _installObjectPrefab;

    public int X { get; set; }
    public int Z { get; set; }

    public InstallObjectCommand(GameObject prefab, NodeSideInfo info)
    {
        _installObjectPrefab = prefab;
        X = x;
        Z = z;

        _cost = 2;
    }

    public override void Execute(GameObject target)
    {
        base.Execute(target);
        GameObject instantiaedObject = GameObject.Instantiate(_installObjectPrefab);
        instantiaedObject.name = _installObjectPrefab.name;
        instantiaedObject.transform.position = new Vector3(X, 0, Z);
        //instantiaedObject.transform.SetParent(target.transform);
    }

}
