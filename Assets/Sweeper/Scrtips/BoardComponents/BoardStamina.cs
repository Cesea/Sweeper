using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardStamina : MonoBehaviour
{
    public int _maxStamina;
    protected int _currentStamina;

    public bool CanDoCommand
    {
        get { return _currentStamina > 0; }
    }
    private void Start()
    {
        ResetStaminaToMax();
    }

    public void ResetStaminaToMax()
    {
        _currentStamina = _maxStamina;
    }

    public bool Consume(int amount)
    {
        _currentStamina -= amount;
        if (_currentStamina < 0)
        {
            _currentStamina += amount;
            return false;
        }
        return true;
    }

}
