using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoardHealth : MonoBehaviour
{
    public int _maxHealth;
    protected int _currentHealth;
    public int CurrentHealth { get { return _currentHealth; } }

    private void Start()
    {
        ResetHealthToMax();
    }

    public bool Alive { get { return _currentHealth > 0; } }

    public bool ReceiveDamage(int amount)
    {
        _currentHealth -= amount;
        if (Alive)
        {
            return true;
        }
        return false;
    }

    public void ResetHealthToMax()
    {
        _currentHealth = _maxHealth;
    }

}
