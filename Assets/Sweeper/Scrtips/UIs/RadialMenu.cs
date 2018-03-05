using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;

using Foundation;

public class RadialMenu : Menu<RadialMenu>
{
    public GameObject _elementPrefab;
    public GameObject _background;
    public GameObject _elementsParent;

    public RectTransform _selectinFollowContainer;

    public float _globalAngleOffset = 0.0f;

    private float _elementAngleGap;
    private float _currentAngle;

    private int _selectingIndex = 0;
    private int _prevSelectingIndex = 0;

    private PointerEventData _pointerEventData;
    private List<RadialMenuElement> _elements = new List<RadialMenuElement>();

    [HideInInspector]
    public RectTransform _rectTransform;

    private bool _commandBuildDone = false;
    private bool _elementBuildDone = false;

    public static void Show(NodeSideInfo info)
    {
        Open();
        Instance.StartCoroutine(Instance.CreateAndShowRadialMenu(info));
    }

    public static void Shut()
    {
        EventManager.Instance.TriggerEvent(new Events.RadialShutEvent());
        Instance._commandBuildDone = false;
        Instance._elementBuildDone = false;

        Instance._selectinFollowContainer.gameObject.SetActive(false);
        Instance._background.gameObject.SetActive(false);

        GameStateManager.Instance.Player.CommandBuffer.Clear();
        Close();
    }

    private void OnCommandBuildDone()
    {
        _commandBuildDone = true;
    }
    private void OnRadialElementBuildDone()
    {
        _elementBuildDone = true;

        _selectinFollowContainer.gameObject.SetActive(true);
        _background.gameObject.SetActive(true);
    }

    private IEnumerator CreateAndShowRadialMenu(NodeSideInfo info)
    {
        _rectTransform = _background.GetComponent<RectTransform>();

        yield return StartCoroutine(CommandBuilder.BuildCommands(GameStateManager.Instance.Player, info, OnCommandBuildDone));
        while (!_commandBuildDone)
        {
            yield return null;
        }

        List<Command> commands = GameStateManager.Instance.Player.CommandBuffer;

        _elementAngleGap = 360.0f / commands.Count;

        for (int i = 0; i < commands.Count; ++i)
        {
            Command currentCommand = commands[i];

            GameObject go = Instantiate(_elementPrefab, _elementsParent.transform);

            RadialMenuElement element = go.GetComponent<RadialMenuElement>();
            element._assignedIndex = i;
            element._parent = this;

            RectTransform rectTrans = element.GetComponent<RectTransform>();
            rectTrans.localRotation = Quaternion.Euler(0, 0, -_elementAngleGap * i + _globalAngleOffset);

            Button currentButton = element._button;
            currentButton.GetComponent<Image>().fillAmount = (1.0f / (commands.Count));

            Text currentText = element._text;
            currentText.text = commands[i].ToString();

            float diffAngle = (270.0f - _elementAngleGap * i + _globalAngleOffset - (_elementAngleGap * 0.5f)) * Mathf.Deg2Rad;
            element.SetAngles(NormalizeAngle(diffAngle * Mathf.Rad2Deg), _elementAngleGap);

            Vector3 offset = new Vector3(Mathf.Cos(diffAngle), Mathf.Sin(diffAngle), 0) * _rectTransform.rect.size.x * 0.4f;
            currentText.GetComponent<RectTransform>().position = _rectTransform.position + offset;
            currentText.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, (_elementAngleGap * i + _globalAngleOffset));

            int localI = i;
            currentButton.onClick.AddListener(() => CallCommand(localI));

            _elements.Add(element);
        }
        OnRadialElementBuildDone();
    }

    public void CallCommand(int i)
    {
        GameStateManager.Instance.Player.DoCommand(i);
    }

    void Start ()
    {
        _pointerEventData = new PointerEventData(EventSystem.current);

	}
	
	void Update ()
    {
        float rawAngle = Mathf.Atan2(Input.mousePosition.y - _rectTransform.position.y,
                                     Input.mousePosition.x - _rectTransform.position.x) * Mathf.Rad2Deg;
        _currentAngle = NormalizeAngle(rawAngle);
        if (_commandBuildDone && _elementBuildDone)
        {

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
            if (Input.GetMouseButtonUp(0) || Input.GetButtonUp("Submit"))
            {
                ExecuteEvents.Execute(_elements[_selectingIndex]._button.gameObject, _pointerEventData, ExecuteEvents.submitHandler);
            }

        }
        _selectinFollowContainer.rotation = Quaternion.Euler(0, 0, rawAngle + 270.0f);
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


    public float NormalizeAngle(float angle)
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
