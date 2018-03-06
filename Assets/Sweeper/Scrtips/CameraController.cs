using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Utils;

public class CameraController : MonoBehaviour
{
    public Transform _cameraTransform;

    public float _verticalOffset;
    public float _horizontalOffset;

    private Timer _moveTimer;
    private Timer _rotateTimer;
    private float _speed = 4.0f;

    private bool _interpolating;
    private Vector3 _targetPosition;
    private Vector3 _startPosition;

    Quaternion _startRotation;
    Quaternion _targetRotation;

    public float _rotateAngleGap = 90.0f;
    float _yRotation;

    private CursorManager _cursorManager;

	void Start ()
    {
        _moveTimer = new Timer(0.5f);
        _rotateTimer = new Timer(0.5f);

        _cursorManager = CursorManager.Instance;
	}
	
	private void LateUpdate ()
    {
        //if (_cursorManager.SelectionValid &&
        //    _cursorManager.SelectingInfoChanged)
        //{
        //    NodeSideInfo current = _cursorManager.SelectingInfo;
        //    //NodeSideInfo prev = _cursorManager.PrevSelectingInfo;
        //    //transform.position = current.GetWorldPosition();
        //    MoveTo(current.GetWorldPosition());
        //}

        Vector2 inputDelta = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxis("Vertical"));
        Move(inputDelta);
        //Moving();

        if (_cursorManager.SelectionValid)
        {
            Vector3 target = _cursorManager.SelectingInfo.GetWorldPosition();

            if (Input.GetKeyDown(KeyCode.Q))
            {
                RotateCamera(_rotateAngleGap);
                StartCoroutine(LocateAndRotateRoutine(_targetRotation, target));
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                RotateCamera(-_rotateAngleGap);
                StartCoroutine(LocateAndRotateRoutine(_targetRotation, target));
            }
        }
	}

    public void RotateCamera(float angle)
    {
        _yRotation += angle;
        if (_yRotation > 360.0f)
        {
            _yRotation -= 360.0f;
        }
        if (_yRotation < 0)
        {
            _yRotation += 360.0f;
        }
        _targetRotation = Quaternion.Euler(0, _yRotation, 0);
    }

    IEnumerator LocateAndRotateRoutine(Quaternion targetRotation, Vector3 targetPosition)
    {
        _targetPosition = targetPosition;
        _startPosition = transform.position;

        _targetRotation = targetRotation;
        _startRotation = transform.rotation;

        yield return StartCoroutine(MoveTo());
        yield return StartCoroutine(RotateTo());
    }

    private IEnumerator MoveTo()
    {
        bool ticked = false;
        while (!ticked)
        {
            ticked = _moveTimer.Tick(Time.deltaTime);
            transform.position = Vector3.Lerp(_startPosition, _targetPosition, _moveTimer.Percent);
            if (ticked)
            {
                transform.position = _targetPosition;
            }
            yield return null;
        }
        _moveTimer.Reset();
    }

    private IEnumerator RotateTo()
    {
        bool ticked = false;
        while (!ticked)
        {
            ticked = _rotateTimer.Tick(Time.deltaTime);
            transform.rotation = Quaternion.Slerp(_startRotation, _targetRotation, _rotateTimer.Percent);
            if (ticked)
            {
                transform.rotation = _targetRotation;
            }
            yield return null;
        }
        _rotateTimer.Reset();
    }

    //private void Rotate()
    //{
    //    Quaternion target = Quaternion.Euler(_currentRotation, _targetRotation, 0);
    //    transform.rotation = Quaternion.Slerp(transform.rotation, target, 0.3f);
    //}

    public void Move(Vector2 inputDelta)
    {
        inputDelta *= _speed * Time.deltaTime;
        _cameraTransform.position += transform.right * inputDelta.x + transform.forward * inputDelta.y;
    }

    //private void MoveTo(Vector3 targetPos)
    //{
    //    //if (_interpolating)
    //    //{
    //    //    return;
    //    //}

    //    _startPosition = transform.position;
    //    _targetPosition = targetPos;

    //    _timer.Reset();

    //    _interpolating = true;
    //}

    //private void Moving()
    //{
    //    if (_interpolating)
    //    {
    //        float percent = _timer.Percent;
    //        Vector3 interPosition = Vector3.Lerp(_startPosition, _targetPosition, percent);

    //        if (_timer.Tick(Time.deltaTime))
    //        {
    //            _timer.Reset();
    //            interPosition = _targetPosition;
    //            _interpolating = false;
    //        }
    //        transform.position = interPosition;
    //    }
    //}
}
