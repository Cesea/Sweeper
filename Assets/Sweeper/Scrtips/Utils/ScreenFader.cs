using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using Utils;

[RequireComponent(typeof(MaskableGraphic))]
public class ScreenFader : MonoBehaviour
{
    public Color _solidColor = Color.white;
    public Color _clearColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);

    public float _timeToFade = 1.0f;

    private MaskableGraphic _graphic;

    private Timer _timer;
    private bool _fading = false;

    private int _fadingDirection = 0;

    private void Awake()
    {
        _graphic = GetComponent<MaskableGraphic>();
        _timer = new Timer(_timeToFade);
    }

    private void Update()
    {
        if (_fading)
        {
            bool ticked = _timer.Tick(Time.deltaTime);
            Color fromColor =  (_fadingDirection == 0) ? _solidColor : _clearColor;
            Color toColor =  (_fadingDirection == 0) ? _clearColor : _solidColor;
            _graphic.color = Color.Lerp(fromColor, toColor, _timer.Percent);
            Debug.Log(_timer.Percent);

            if (ticked)
            {
                _fading = false;
                _timer.Reset();
                gameObject.SetActive(false);
            }
        }
    }

    void UpdateColor(Color newColor)
    {
        _graphic.color = newColor;
    }

    public void FadeOff()
    {
        _fading = true;
        _fadingDirection = 0;
    }

    public void FadeOn()
    {
        _fading = true;
        _fadingDirection = 1;
    }
}
