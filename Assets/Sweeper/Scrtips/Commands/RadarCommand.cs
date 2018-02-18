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
    } 

    public override void Execute(GameObject target)
    {
        //Instantiate particle prefab;
    }
}
