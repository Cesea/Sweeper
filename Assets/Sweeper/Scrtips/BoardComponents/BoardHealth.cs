using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardHealth : MonoBehaviour
{
    public int _maxHealth;
    public int _currentHealth;

    public bool Alive { get { return _currentHealth > 0; } }

}
