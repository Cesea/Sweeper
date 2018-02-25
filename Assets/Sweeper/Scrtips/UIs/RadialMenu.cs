using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using Foundation;

public class RadialMenu : MonoBehaviour
{
    //public GameObject _centerCirclePrefab;
    //public GameObject _radialMenuItemPrefab;

    //public List<GameObject> _menuItems = new List<GameObject>();

    //public static void Show(Vector3 mousePosition, NodeSideInfo info)
    //{
    //    Open();
    //    Instance.ShowRadialMenu(info); 
    //}

    //public static void Shut()
    //{
    //    //GameStateManager.Instance.SelectingObject = null;
    //    Close();
    //}

    //private void Update()
    //{
    //    if (Input.GetMouseButtonUp(1))
    //    {
    //        CloseRadialMenu();
    //        RadialMenu.Shut();
    //        EventManager.Instance.TriggerEvent(new Events.RadialShutEvent());
    //    }
    //}

    //public void CallCommand(int index)
    //{
    //    Debug.Log(index);
    //    if (_menuItems.Count > 1)
    //    {
    //        //Interactable interactable = GameStateManager.Instance.SelectingObject.GetComponent<Interactable>();
    //        //interactable.DoCommand(index);
    //    }
    //} 

    //void ShowRadialMenu(NodeSideInfo nodeInfo)
    //{
    //    GameObject InstalledObject = nodeInfo.Node.GetInstalledObjectAt(nodeInfo.Side);

    //    if (InstalledObject != null)
    //    {
    //        GameObject centerRadialObject = Instantiate(_centerCirclePrefab, transform);
    //        centerRadialObject.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
    //        centerRadialObject.GetComponentInChildren<Text>().text = InstalledObject.name;

    //        Interactable interactable = InstalledObject.GetComponent<Interactable>();
    //        Command[] availableCommands = interactable.GetAvailableCommands();

    //        if (availableCommands.Length > 0)
    //        {
    //            float radius = 80;
    //            float angleGap = (Mathf.PI * 2) / availableCommands.Length;

    //            for (int i = 0; i < availableCommands.Length; ++i)
    //            {
    //                Command currentCommand = availableCommands[i];
    //                GameObject radialItem = Instantiate(_radialMenuItemPrefab);
    //                radialItem.transform.SetParent(centerRadialObject.transform);
    //                Text text = radialItem.GetComponentInChildren<Text>();
    //                text.text = currentCommand.GetType().Name;
    //                radialItem.transform.position = centerRadialObject.transform.position;
    //                radialItem.transform.position += new Vector3(Mathf.Cos(angleGap * i) * radius, Mathf.Sin(angleGap * i) * radius, 0.0f);
    //                radialItem.transform.localScale = Vector3.one;

    //                int localCount = i;

    //                radialItem.GetComponent<Button>().onClick.AddListener(() => CallCommand(localCount));
    //                _menuItems.Add(radialItem);
    //            }
    //        }
    //        _menuItems.Add(centerRadialObject);
    //    }
    //}

    //void CloseRadialMenu()
    //{
    //    for (int i = _menuItems.Count - 1; i >= 0; --i)
    //    {
    //        Destroy(_menuItems[i]);
    //    }
    //    _menuItems.Clear();
    //}

    [HideInInspector]
    public RectTransform _rectTransform;
    //public RectTransform baseCircleRT;
    //public Image selectionFollowerImage;

    [Tooltip("Adjusts the radial menu for use with a gamepad or joystick. You might need to edit this script if you're not using the default horizontal and vertical input axes.")]
    public bool _useGamepad = false;

    [Tooltip("With lazy selection, you only have to point your mouse (or joystick) in the direction of an element to select it, rather than be moused over the element entirely.")]
    public bool _useLazySelection = true;

    [Tooltip("If set to true, a pointer with a graphic of your choosing will aim in the direction of your mouse. You will need to specify the container for the selection follower.")]
    public bool _useSelectionFollower = true;

    [Tooltip("If using the selection follower, this must point to the rect transform of the selection follower's container.")]
    public RectTransform _selectionFollowerContainer;

    [Tooltip("This is the text object that will display the labels of the radial elements when they are being hovered over. If you don't want a label, leave this blank.")]
    public Text _textLabel;

    [Tooltip("This is the list of radial menu elements. This is order-dependent. The first element in the list will be the first element created, and so on.")]
    public List<RadialMenuElement> _elements = new List<RadialMenuElement>();

    [Tooltip("Controls the total angle offset for all elements. For example, if set to 45, all elements will be shifted +45 degrees. Good values are generally 45, 90, or 180")]
    public float _globalOffset = 0f;

    [HideInInspector]
    public float _currentAngle = 0f; //Our current angle from the center of the radial menu.

    [HideInInspector]
    public int _index = 0; //The current index of the element we're pointing at.

    private int _elementCount;

    private float _angleOffset; //The base offset. For example, if there are 4 elements, then our offset is 360/4 = 90

    private int _previousActiveIndex = 0; //Used to determine which buttons to unhighlight in lazy selection.

    private PointerEventData _pointer;

    private void Awake()
    {
        _pointer = new PointerEventData(EventSystem.current);

        _rectTransform = GetComponent<RectTransform>();

        if (_rectTransform == null)
        {
            Debug.LogError("Radial Menu: Rect Transform for radial menu " + gameObject.name + " could not be found. Please ensure this is an object parented to a canvas.");
        }

        if (_useSelectionFollower && _selectionFollowerContainer == null)
        {
            Debug.LogError("Radial Menu: Selection follower container is unassigned on " + gameObject.name + ", which has the selection follower enabled.");
        }

        _elementCount = _elements.Count;

        _angleOffset = (360f / (float)_elementCount);

        //Loop through and set up the elements.
        for (int i = 0; i < _elementCount; i++)
        {
            if (_elements[i] == null)
            {
                Debug.LogError("Radial Menu: element " + i.ToString() + " in the radial menu " + gameObject.name + " is null!");
                continue;
            }
            _elements[i]._parentMenu = this;

            _elements[i].SetAllAngles((_angleOffset * i) + _globalOffset, _angleOffset);

            _elements[i]._assignedIndex = i;
        }
    }

    void Start()
    {
        if (_useGamepad)
        {
            EventSystem.current.SetSelectedGameObject(gameObject, null); //We'll make this the active object when we start it. Comment this line to set it manually from another script.
            if (_useSelectionFollower && _selectionFollowerContainer != null)
                _selectionFollowerContainer.rotation = Quaternion.Euler(0, 0, -_globalOffset); //Point the selection follower at the first element.
        }
    }

    void Update()
    {
        //If your gamepad uses different horizontal and vertical joystick inputs, change them here!
        //==============================================================================================
        bool joystickMoved = Input.GetAxis("Horizontal") != 0.0 || Input.GetAxis("Vertical") != 0.0;
        //==============================================================================================

        float rawAngle;

        if (!_useGamepad)
        {
            rawAngle = Mathf.Atan2(Input.mousePosition.y - _rectTransform.position.y, 
                Input.mousePosition.x - _rectTransform.position.x) * Mathf.Rad2Deg;
        }
        else
        {
            rawAngle = Mathf.Atan2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal")) * Mathf.Rad2Deg;
        }

        //If no gamepad, update the angle always. Otherwise, only update it if we've moved the joystick.
        if (!_useGamepad)
        {
            _currentAngle = NormalizeAngle(-rawAngle + 90 - _globalOffset + (_angleOffset / 2f));
        }
        else if (joystickMoved)
        {
            _currentAngle = NormalizeAngle(-rawAngle + 90 - _globalOffset + (_angleOffset / 2f));
        }

        //Handles lazy selection. Checks the current angle, matches it to the index of an element, and then highlights that element.
        if (_angleOffset != 0 && _useLazySelection) {

            //Current element index we're pointing at.
            _index = (int)(_currentAngle / _angleOffset);

            if (_elements[_index] != null) {

                //Select it.
                SelectButton(_index);

                //If we click or press a "submit" button (Button on joystick, enter, or spacebar), then we'll execut the OnClick() function for the button.
                if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Submit"))
                {
                    ExecuteEvents.Execute(_elements[_index]._button.gameObject, _pointer, ExecuteEvents.submitHandler);
                }
            }

        }

        //Updates the selection follower if we're using one.
        if (_useSelectionFollower && _selectionFollowerContainer != null)
        {
            if (!_useGamepad || joystickMoved)
                _selectionFollowerContainer.rotation = Quaternion.Euler(0, 0, rawAngle + 270);
        } 

    }


    private void SelectButton(int i)
    {
        if (_elements[i]._active == false)
        {
            _elements[i].HighlightThisElement(_pointer); //Select this one

            if (_previousActiveIndex != i)
            {
                _elements[_previousActiveIndex].UnHighlightThisElement(_pointer); //Deselect the last one.
            }
        }
        _previousActiveIndex = i;
    }

    private float NormalizeAngle(float angle)
    {
        angle = angle % 360f;

        if (angle < 0)
            angle += 360;

        return angle;
    }

}
