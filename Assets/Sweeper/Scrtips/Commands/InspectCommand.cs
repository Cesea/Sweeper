using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Foundation;

public class InspectCommand : Command
{
    public string _name;

    public InspectCommand()
    {
        _cost = 1;
    }

    public override bool Execute(GameObject target)
    {
        DialogContent content = new DialogContent();
        content._content = "Objet Name : " + target.name;
        content._action = () => ModalDialog.Shut();

        //RadialMenu.Shut();

        ModalDialog.Show(content);

        return true;
    }
}
