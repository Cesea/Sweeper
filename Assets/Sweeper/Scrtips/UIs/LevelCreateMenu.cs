using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using Level;

public class LevelCreateMenu : Menu<LevelCreateMenu>
{
    private LevelCreator _levelCreator;

    public GameObject _buttonPanel;
    public GameObject _buttonPrefab;


    public static void Show()
    {
        Open();
        Instance._levelCreator = LevelCreator.Instance;

        for (int i = 0; i < Instance._levelCreator._installObjectPrefabs.Count; ++i)
        {
            GameObject currentPrefab = Instance._levelCreator._installObjectPrefabs[i];

            GameObject go = GameObject.Instantiate(Instance._buttonPrefab, Instance._buttonPanel.transform);
            go.GetComponentInChildren<Text>().text = currentPrefab.name;

            int localI = i;
            go.GetComponent<Button>().onClick.AddListener(() => Instance._levelCreator.SetSelectingIndex(localI));
        }
    }

    public static void Shut()
    {
        Close();
    }
}
