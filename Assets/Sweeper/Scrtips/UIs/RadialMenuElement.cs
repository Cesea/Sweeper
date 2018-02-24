using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RadialMenuElement : MonoBehaviour
{
    [HideInInspector]
    public RectTransform _rectTransform;
    [HideInInspector]
    public RadialMenu _parentMenu;

    [Tooltip("Each radial element needs a button. This is generally a child one level below this primary radial element game object.")]
    public Button _button;

    [Tooltip("This is the text label that will appear in the center of the radial menu when this option is moused over. Best to keep it short.")]
    public string _label;

    [HideInInspector]
    public float _minAngle, _maxAngle;

    [HideInInspector]
    public float _angleOffset;

    [HideInInspector]
    public bool _active = false;

    [HideInInspector]
    public int _assignedIndex = 0;
    // Use this for initialization

    private CanvasGroup cg;

    private void Awake()
    {
        _rectTransform = gameObject.GetComponent<RectTransform>();

        if (gameObject.GetComponent<CanvasGroup>() == null)
        {
            cg = gameObject.AddComponent<CanvasGroup>();
        }
        else
        {
            cg = gameObject.GetComponent<CanvasGroup>();
        }

        if (_rectTransform == null)
        {
            Debug.LogError("Radial Menu: Rect Transform for radial element " + gameObject.name + " could not be found. Please ensure this is an object parented to a canvas.");
        }
        if (_button == null)
        {
            Debug.LogError("Radial Menu: No button attached to " + gameObject.name + "!");
        }
    }

    void Start ()
    {
        _rectTransform.rotation = Quaternion.Euler(0, 0, -_angleOffset); //Apply rotation determined by the parent radial menu.

        //If we're using lazy selection, we don't want our normal mouse-over effects interfering, so we turn raycasts off.
        if (_parentMenu._useLazySelection)
        {
            cg.blocksRaycasts = false;
        }
        else
        {
            EventTrigger t;

            if (_button.GetComponent<EventTrigger>() == null)
            {
                t = _button.gameObject.AddComponent<EventTrigger>();
                t.triggers = new System.Collections.Generic.List<EventTrigger.Entry>();
            }
            else
            {
                t = _button.GetComponent<EventTrigger>();
            }

            EventTrigger.Entry enter = new EventTrigger.Entry();
            enter.eventID = EventTriggerType.PointerEnter;
            enter.callback.AddListener((eventData) => { SetParentMenuLable(_label); });


            EventTrigger.Entry exit = new EventTrigger.Entry();
            exit.eventID = EventTriggerType.PointerExit;
            exit.callback.AddListener((eventData) => { SetParentMenuLable(""); });

            t.triggers.Add(enter);
            t.triggers.Add(exit);

        }

	}

    //Used by the parent radial menu to set up all the approprate angles. Affects master Z rotation and the active angles for lazy selection.
    public void SetAllAngles(float offset, float baseOffset)
    {
        _angleOffset = offset;
        _minAngle = offset - (baseOffset / 2f);
        _maxAngle = offset + (baseOffset / 2f);
    }

    //Highlights this button. Unity's default button wasn't really meant to be controlled through code so event handlers are necessary here.
    //I would highly recommend not messing with this stuff unless you know what you're doing, if one event handler is wrong then the whole thing can break.
    public void HighlightThisElement(PointerEventData p)
    {

        ExecuteEvents.Execute(_button.gameObject, p, ExecuteEvents.selectHandler);
        _active = true;
        SetParentMenuLable(_label);

    }

    //Sets the label of the parent menu. Is set to public so you can call this elsewhere if you need to show a special label for something.
    public void SetParentMenuLable(string l)
    {

        if (_parentMenu._textLabel != null)
        {
            _parentMenu._textLabel.text = l;
        }
    }


    //Unhighlights the button, and if lazy selection is off, will reset the menu's label.
    public void UnHighlightThisElement(PointerEventData p)
    {
        ExecuteEvents.Execute(_button.gameObject, p, ExecuteEvents.deselectHandler);
        _active = false;

        if (!_parentMenu._useLazySelection)
        {
            SetParentMenuLable(" ");
        }
    }

    //Just a quick little test you can run to ensure things are working properly.
    public void ClickMeTest()
    {
        Debug.Log(_assignedIndex);
    }


}
