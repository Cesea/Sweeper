using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RadialMenu : Menu<RadialMenu>
{
    [Header("Elements")]
    public List<RadialMenuElement> _elements = new List<RadialMenuElement>();
    [Header("Follower")]
    public RectTransform _selectinFollowContainer;

    public float _globalAngleOffset = 0.0f;

    private float _elementAngleGap;
    private float _currentAngle;

    private int _selectingIndex = 0;
    private int _prevSelectingIndex = 0;

    private PointerEventData _pointerEventData;

    [HideInInspector]
    public RectTransform _rectTransform;

    public static void Show()
    {
        Open();
    }

    public static void Shut()
    {
        Close();
    }

    void Start ()
    {
        _pointerEventData = new PointerEventData(EventSystem.current);


        SetElementsRotation();
	}
	
	void Update ()
    {
        float rawAngle = Mathf.Atan2(Input.mousePosition.y - _rectTransform.position.y,
                                     Input.mousePosition.x - _rectTransform.position.x) * Mathf.Rad2Deg;
        _currentAngle = NormalizeAngle(rawAngle);

        Debug.Log(_currentAngle);

        foreach (var e in _elements)
        {
            if (e.IsInAngle(_currentAngle))
            {
                _selectingIndex = e._assignedIndex;
                break;
            }
        }

        SelectButton(_selectingIndex);
        //If we click or press a "submit" button (Button on joystick, enter, or spacebar), then we'll execut the OnClick() function for the button.
        if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Submit"))
        {
            ExecuteEvents.Execute(_elements[_selectingIndex]._button.gameObject, _pointerEventData, ExecuteEvents.submitHandler);
        }

        _selectinFollowContainer.rotation = Quaternion.Euler(0, 0, rawAngle + 270.0f);
	}

    public void SetElementsRotation()
    {
        _rectTransform = GetComponent<RectTransform>();
        _elementAngleGap = 360.0f / _elements.Count;

        for (int i = 0; i < _elements.Count; ++i)
        {
            RadialMenuElement currentElement = _elements[i];
            currentElement._assignedIndex = i;
            currentElement._parent = this;

            RectTransform trans = currentElement.GetComponent<RectTransform>();
            trans.localRotation = Quaternion.Euler(0, 0, _elementAngleGap * i + _globalAngleOffset);

            Button currentButton = currentElement._button;
            Text currentText = currentElement._text;

            currentButton.GetComponent<Image>().fillAmount = (1.0f / (_elements.Count));

            float diffAngle = (_elementAngleGap * i + 180 + _elementAngleGap * 0.5f + _globalAngleOffset) * Mathf.Deg2Rad;
            currentElement.SetAngles(NormalizeAngle(diffAngle * Mathf.Rad2Deg), _elementAngleGap);

            Vector3 offset = new Vector3(Mathf.Cos(diffAngle), Mathf.Sin(diffAngle), 0) * (_rectTransform.rect.width + 30);
            currentElement._text.GetComponent<RectTransform>().position = trans.position + offset;
            currentElement._text.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, -(_elementAngleGap * i + _globalAngleOffset));

        }
    }

    private void SelectButton(int i)
    {
        if (_elements[i]._active == false)
        {
            _elements[i].Highlight(_pointerEventData); //Select this one

            if (_prevSelectingIndex != i)
            {
                _elements[_prevSelectingIndex].UnHighlight(_pointerEventData); //Deselect the last one.
            }
        }
        _prevSelectingIndex = i;
    } 


    private float NormalizeAngle(float angle)
    {
        if (angle > 360.0f)
        {
            angle -= 360.0f;
        }
        if (angle < 0.0f)
        {
            angle += 360.0f;
        }
        return angle;
    }



}
