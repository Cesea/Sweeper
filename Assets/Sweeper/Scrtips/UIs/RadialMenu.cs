using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.Events;

using Foundation;

public class RadialMenu : Menu<RadialMenu>
{
    public GameObject _centerCirclePrefab;
    public GameObject _radialMenuItemPrefab;

    public List<GameObject> _menuItems = new List<GameObject>();

    int _itemCount = 0;

    public static void Show(Vector3 mousePosition, GameObject selectingObject)
    {
        Open();
        Instance.ShowRadialMenu(selectingObject); 
    }

    public static void Shut()
    {
        GameStateManager.Instance.SelectingObject = null;
        Close();
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            CloseRadialMenu();
            RadialMenu.Shut();
            EventManager.Instance.TriggerEvent(new Events.RadialShutEvent());
        }
    }

    public void CallCommand(int index)
    {
        Debug.Log(index);
        if (_menuItems.Count > 1)
        {
            Interactable interactable = GameStateManager.Instance.SelectingObject.GetComponent<Interactable>();
            interactable.DoCommand(index);
        }
    } 

    void ShowRadialMenu(GameObject instance)
    {
        GameObject centerRadialObject = Instantiate(_centerCirclePrefab, transform);
        centerRadialObject.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
        centerRadialObject.GetComponentInChildren<Text>().text = instance.name;

        Interactable interactable = instance.GetComponent<Interactable>();
        Command[] availableCommands = interactable.GetAvailableCommands();

        if (availableCommands.Length > 0)
        {
            float radius = 80;
            float angleGap = (Mathf.PI * 2) / availableCommands.Length;

            for (int i = 0; i < availableCommands.Length; ++i)
            {
                Command currentCommand = availableCommands[i];
                GameObject radialItem = Instantiate(_radialMenuItemPrefab);
                radialItem.transform.SetParent(centerRadialObject.transform);
                Text text = radialItem.GetComponentInChildren<Text>();
                text.text = currentCommand.GetType().Name;
                radialItem.transform.position = centerRadialObject.transform.position;
                radialItem.transform.position += new Vector3(Mathf.Cos(angleGap * i) * radius, Mathf.Sin(angleGap * i) * radius, 0.0f);
                radialItem.transform.localScale = Vector3.one;

                int localCount = i;

                radialItem.GetComponent<Button>().onClick.AddListener(() => CallCommand(localCount));
                _menuItems.Add(radialItem);
            }
        }
        _menuItems.Add(centerRadialObject);
    }

    void CloseRadialMenu()
    {
        for (int i = _menuItems.Count - 1; i >= 0; --i)
        {
            Destroy(_menuItems[i]);
        }
        _menuItems.Clear();
    }

}
