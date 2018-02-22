using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Foundation;

[System.Serializable]
public class RadarCommand : Command
{

    public int X { get; set; }
    public int Z { get; set; }

    public RadarCommand(int x, int z)
    {
        X = x;
        Z = z;
        _cost = 1;
    } 

    public override void Execute(GameObject target)
    {
        base.Execute(target);
        //Instantiate particle prefab;
    }
}
