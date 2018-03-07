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

    private bool _interpolating;
    public bool Interpolating { get { return _interpolating; } }
    private float _startAngle;
    private float _currentAngle;
    private float _targetAngle;
    private Utils.Timer _timer;

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

        _timer = new Utils.Timer(2f);
    }

    void Start ()
    {
        _canvasGroup.blocksRaycasts = false;
	}

    private void Update()
    {
        if (_interpolating)
        {
            bool ticked = _timer.Tick(Time.deltaTime);
            float angleDiff = _startAngle + (_targetAngle - _startAngle) * (_timer.Percent * _timer.Percent);
            Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, angleDiff);
            if (ticked)
            {
                rotation = Quaternion.Euler(0.0f, 0.0f, _targetAngle);
                _interpolating = false;
                _startAngle = _targetAngle;
            }
            transform.localRotation = rotation;
        }
    }

    public void SetAngles(float offset, float baseOffset)
    {
        _angleOffset = offset;
        _minAngle = offset - (baseOffset / 2f);
        _maxAngle = offset + (baseOffset / 2f);

        _minAngle = _parent.NormalizeAngle(_minAngle);
        _maxAngle = _parent.NormalizeAngle(_maxAngle);
    }

    public void RotateTo(float targetAngle, float time)
    {
        _timer.TargetTime = time;

        _targetAngle = targetAngle;
        _interpolating = true;
    }

    public bool IsInAngle(float angle)
    {
        //4, 1, 사분면에 걸쳐 있는 경우
        if (_minAngle > _maxAngle)
        {
            if (angle > _minAngle || angle < _maxAngle)
            {
                return true;
            }
        }
        else
        {
            if (angle > _minAngle && angle < _maxAngle)
            {
                return true;
            }
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
