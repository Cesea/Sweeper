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

    public override void Execute(CommandInterpreter target)
    {
        Vector3 newPosition = new Vector3(
            target.transform.position.x + DeltaX, 
            target.transform.position.y, 
            target.transform.position.z + DeltaZ);
        target.transform.position = newPosition;
    }

}
