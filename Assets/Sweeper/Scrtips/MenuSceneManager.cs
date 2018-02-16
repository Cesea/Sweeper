using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class MenuSceneManager : MonoBehaviour
{

    public void StartGame()
    {
        SceneLoader.Instance.LoadScene(1);
    }

    public void LoadStatus()
    {
        //Load Scene
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
