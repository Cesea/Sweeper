using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Foundation;

[System.Serializable]
public class WalkCommand : Command
{
    public int DeltaX { get; set; }
    public int DeltaZ { get; set; }

    public WalkCommand(int deltaX, int deltaZ)
    {
        DeltaX = deltaX;
        DeltaZ = deltaZ;
    } 

    public override void Execute(GameObject target)
    {
        BoardWalk move = target.GetComponent<BoardWalk>();
        move.MoveBy(DeltaX, DeltaZ);
    }
}
