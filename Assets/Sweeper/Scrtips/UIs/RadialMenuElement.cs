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
    public RadialMenu _parent;

    public Button _button;
    public Text _text;

    [HideInInspector]
    public float _minAngle, _maxAngle;
    [HideInInspector]
    public float _angleOffset;
    [HideInInspector]
    public bool _active = false;
    [HideInInspector]
    public int _assignedIndex = 0;

    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        if (GetComponent<CanvasGroup>() == null)
        {
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        else
        {
            _canvasGroup = gameObject.GetComponent<CanvasGroup>();
        }
    }

    void Start ()
    {
        _canvasGroup.blocksRaycasts = false;
	}

    public void SetAngles(float offset, float baseOffset)
    {
        _angleOffset = offset;
        _minAngle = offset - (baseOffset / 2f);
        _maxAngle = offset + (baseOffset / 2f);
    }

    public bool IsInAngle(float angle)
    {
        if (angle > _minAngle && angle < _maxAngle)
        {
            return true;
        }
        return false;
    }

    public void Highlight(PointerEventData p)
    {
        ExecuteEvents.Execute(_button.gameObject, p, ExecuteEvents.selectHandler);
        _active = true;
        //SetParentMenuLable(_label);
    }

    public void UnHighlight(PointerEventData p)
    {
        ExecuteEvents.Execute(_button.gameObject, p, ExecuteEvents.deselectHandler);
        _active = false;
    }

    //public void SetParentMenuLabel(string text)
    //{
    //    _parent.useGUILayout
    //}

}
