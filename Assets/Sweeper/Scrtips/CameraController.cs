using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Utils;

public class CameraController : MonoBehaviour
{
    public Transform _cameraTransform;

    public float _verticalOffset;
    public float _horizontalOffset;

    Timer _timer;
    public float _speed;

    public float _rotateAngleGap = 90.0f;

    private float _currentRotation = 0.0f;
    private float _targetRotation = 0.0f;

	void Start ()
    {
		
	}
	
	void Update ()
    {

        Vector2 inputDelta = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxis("Vertical"));
        Move(inputDelta);

        if (Input.GetKeyDown(KeyCode.Q))
        {
            RotateCamera(-_rotateAngleGap);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            RotateCamera(_rotateAngleGap);
        }

        Rotate();
	}

    public void RotateCamera(float angle)
    {
        _targetRotation += angle;
        if (_targetRotation >= 360.0f)
        {
            _targetRotation -= 360.0f;
        }
        if (_targetRotation < 0.0f)
        {
            _targetRotation += 360.0f;
        }
    }

    private void Rotate()
    {
        Quaternion target = Quaternion.Euler(_currentRotation, _targetRotation, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, 0.3f);
    }

    public void Move(Vector2 inputDelta)
    {
        inputDelta *= _speed * Time.deltaTime;
        _cameraTransform.position += transform.right * inputDelta.x + transform.forward * inputDelta.y;
    }
}
