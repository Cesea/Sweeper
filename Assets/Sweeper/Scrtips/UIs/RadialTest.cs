using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RadialTest : MonoBehaviour
{
    [Header("Elements")]
    public List<RadialElementTest> _elements = new List<RadialElementTest>();
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

    private void Awake()
    {
        _pointerEventData = new PointerEventData(EventSystem.current);

        _elementAngleGap = 360.0f / _elements.Count;
        _rectTransform = GetComponent<RectTransform>();

        for (int i = 0; i < _elements.Count; ++i)
        {
            if (_elements[i] != null)
            {
                _elements[i]._parent = this;
                _elements[i]._assignedIndex = i;
            }
        }
    }

    void Start ()
    {
        SetElementsRotation();
	}
	
	void Update ()
    {
        float rawAngle = Mathf.Atan2(Input.mousePosition.y - _rectTransform.position.y,
                                     Input.mousePosition.x - _rectTransform.position.x) * Mathf.Rad2Deg;
        _currentAngle = NormalizeAngle(rawAngle + 180.0f);

        _selectingIndex = (int)(_currentAngle / _elementAngleGap);
        if (_elements[_selectingIndex] != null)
        {
            SelectButton(_selectingIndex);

            //If we click or press a "submit" button (Button on joystick, enter, or spacebar), then we'll execut the OnClick() function for the button.
            if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Submit"))
            {
                ExecuteEvents.Execute(_elements[_selectingIndex]._button.gameObject, _pointerEventData, ExecuteEvents.submitHandler);
            }
        }

        _selectinFollowContainer.rotation = Quaternion.Euler(0, 0, rawAngle + 270.0f);
	}

    public void SetElementsRotation()
    {
        for (int i = 0; i < _elements.Count; ++i)
        {
            RectTransform trans = _elements[i].GetComponent<RectTransform>();
            trans.localRotation = Quaternion.Euler(0, 0, _elementAngleGap * i + _globalAngleOffset);

            Button currentButton = _elements[i]._button;
            Text currentText = _elements[i]._text;

            currentButton.GetComponent<Image>().fillAmount = (1.0f / (_elements.Count));
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
