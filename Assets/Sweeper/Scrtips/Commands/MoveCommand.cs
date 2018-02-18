using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Foundation;

[System.Serializable]
public class MoveCommand : Command
{
    public int DeltaX { get; set; }
    public int DeltaZ { get; set; }

    public MoveCommand(int deltaX, int deltaZ)
    {
        DeltaX = deltaX;
        DeltaZ = deltaZ;
    } 

    public override void Execute(GameObject target)
    {
        BoardMover move = target.GetComponent<BoardMover>();
        move.MoveBy(DeltaX, DeltaZ);
    }
}
