using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using Foundation;

public class RadialMenu : MonoBehaviour
{
    public GameObject _centerCirclePrefab;
    public GameObject _radialItemPrefab;

    public LayerMask _cellMask;
    public LayerMask _objectMask;

    public List<GameObject> _menuItems = new List<GameObject>();

    private float _downTime = 0.0f;
    private float _menuTriggerTime = 0.5f;

    private bool _prevShowing = false;
    private bool _showing = false;

    private void Update()
    {
        if (!_showing)
        {
            if (Input.GetMouseButton(1))
            {
                GameObject underneathObject = GetObjectUnderneathMouse();
                if (underneathObject != null)
                {
                    _downTime += Time.deltaTime;
                    if (_downTime > _menuTriggerTime)
                    {
                        ShowRadialMenu(underneathObject);
                    }
                }
            }
        }

        if (_showing)
        {
            if (Input.GetMouseButtonUp(1))
            {
                CloseRadialMenu();
            }
        }

        _prevShowing = _showing;
    }

    //일단 오브젝트가 있는 땅이면 못간다....
    private GameObject GetObjectUnderneathMouse()
    {
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(camRay, out hitInfo, _objectMask))
            {
                if (hitInfo.collider.gameObject.GetComponent<Interactable>() != null)
                {
                    return hitInfo.collider.gameObject;
                }
            }
        }
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(camRay, out hitInfo, _cellMask))
            {
                if (hitInfo.collider.gameObject.GetComponent<Interactable>() != null)
                {
                    return hitInfo.collider.gameObject;
                }
            }
        }
        return null;
    } 

    void ShowRadialMenu(GameObject instance)
    {
        _showing = true;
        _downTime = 0.0f;
        GameObject go = Instantiate(_centerCirclePrefab, transform);
        go.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
        go.GetComponentInChildren<Text>().text = instance.name;

        Interactable interaction = instance.GetComponent<Interactable>();
        Command[] availableCommands = interaction.GetAvailableCommands();

        if (availableCommands.Length > 0)
        {
            float radius = 100;
            float angleGap = (Mathf.PI * 2) / availableCommands.Length;

            for (int i = 0; i < availableCommands.Length; ++i)
            {
                Command currentCommand = availableCommands[i];
                GameObject radialItem = Instantiate(_radialItemPrefab);
                radialItem.transform.SetParent(go.transform);
                Text text = radialItem.GetComponentInChildren<Text>();
                text.text = currentCommand.GetType().Name;
                radialItem.transform.position = go.transform.position;
                radialItem.transform.position += new Vector3(Mathf.Cos(angleGap * i) * radius, Mathf.Sin(angleGap * i) * radius, 0.0f);
                radialItem.transform.localScale = Vector3.one;

                if (currentCommand.GetType() == typeof(MoveCommand)) 
                {
                    radialItem.GetComponent<Button>().onClick.AddListener(GameStateManager.Instance.Player.MoveRight);
                }
                _menuItems.Add(radialItem);
            }
        }

        _menuItems.Add(go);
    }

    void CloseRadialMenu()
    {
        _showing = false;
        for (int i = _menuItems.Count - 1; i >= 0; --i)
        {
            Destroy(_menuItems[i]);
        }
        _menuItems.Clear();
    }


}
