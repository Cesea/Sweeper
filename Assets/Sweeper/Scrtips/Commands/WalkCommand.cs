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
        _cost = 1;
    } 

    public override void Execute(GameObject target)
    {
        base.Execute(target);

        BoardWalk move = target.GetComponent<BoardWalk>();
        move.MoveBy(DeltaX, 0, DeltaZ);
    }
}
