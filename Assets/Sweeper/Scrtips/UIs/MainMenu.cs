using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : SimpleMenu<MainMenu>
{
    public override void OnBackPressed()
    {
        Application.Quit();
    }

    public void OnStartButtonPressed()
    {
        SceneLoader.Instance.LoadScene(1);
    }

    public void OnOptionButtonPressed()
    {
        OptionMenu.Show();
    }

    public void OnExitButtonPressed()
    {
        Application.Quit();
    }
}
