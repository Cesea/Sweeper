using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using Level;

public class LevelCreateMenu : Menu<LevelCreateMenu>
{
    public LevelCreateCursor _levelCreateCursor;

    public GameObject _buttonPanel;
    public GameObject _buttonPrefab;

    [SerializeField]
    private Text _offsetText;
    public Text OffsetText { get { return _offsetText; } }
    [SerializeField]
    private Text _rotationText;
    public Text RotationText { get { return _rotationText; } }

    public static void Show()
    {
        Open();
        Instance._levelCreateCursor = CursorManager.Instance.CreateCursor;

        for (int i = 0; i < Instance._levelCreateCursor.InstallPrefabList.Count; ++i)
        {
            GameObject currentPrefab = Instance._levelCreateCursor.InstallPrefabList[i];

            GameObject go = GameObject.Instantiate(Instance._buttonPrefab, Instance._buttonPanel.transform);
            go.GetComponentInChildren<Text>().text = currentPrefab.name;

            int localI = i;
            go.GetComponent<Button>().onClick.AddListener(() => Instance._levelCreateCursor.SetSelectingIndex(localI));
        }

        EventManager.Instance.TriggerEvent(new Events.LevelCreatorMenuEvent(true));
    }

    public static void Shut()
    {
        EventManager.Instance.TriggerEvent(new Events.LevelCreatorMenuEvent(false));
        Close();
    }
}
