using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Foundation;

[System.Serializable]
public class InstallObjectCommand : Command
{
    public GameObject _installObjectPrefab;

    public int X { get; set; }
    public int Z { get; set; }

    public InstallObjectCommand(GameObject prefab, int x, int z)
    {
        _installObjectPrefab = prefab;
        X = x;
        Z = Z;
    }

    public override void Execute(GameObject target)
    {
        GameObject instantiaedObject = GameObject.Instantiate(_installObjectPrefab);
        instantiaedObject.name = _installObjectPrefab.name;
        instantiaedObject.transform.position = new Vector3(X, 0, Z);
        instantiaedObject.transform.SetParent(target.transform);
    }

}
