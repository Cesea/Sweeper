using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardDestroyable : MonoBehaviour
{
    [SerializeField]
    private float _destroySuccessPercent;
    public float DestroySuccessPercent { get { return _destroySuccessPercent; }  set { _destroySuccessPercent = value; } }

    public bool DoDestroy()
    {
        float random = Random.Range(0.0f, 1.0f);
        if (random < _destroySuccessPercent)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
