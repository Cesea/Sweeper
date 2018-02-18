using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Foundation;

public class Interactable : MonoBehaviour
{
    public Command[] GetAvailableCommands()
    {
        return CommandBuilder.BuildCommands(gameObject);
    }

}
