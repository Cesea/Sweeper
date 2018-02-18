using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class OptionMenu : SimpleMenu<OptionMenu>
{
    public void OnBackButton()
    {
        OptionMenu.Shut();
    }
}
