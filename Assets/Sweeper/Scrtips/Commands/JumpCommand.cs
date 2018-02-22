using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Foundation;

[System.Serializable]
public class JumpCommand : Command
{
    public int DeltaX { get; set; }
    public int DeltaY { get; set; }
    public int DeltaZ { get; set; }

    public JumpCommand(int deltaX, int deltaY, int deltaZ)
    {
        DeltaX = deltaX;
        DeltaY = deltaY;
        DeltaZ = deltaZ;

        _cost = 2;
    } 

    public override void Execute(GameObject target)
    {
        base.Execute(target);
        BoardJump jump = target.GetComponent<BoardJump>();
        jump.MoveBy(DeltaX, DeltaY, DeltaZ);
    }
}
