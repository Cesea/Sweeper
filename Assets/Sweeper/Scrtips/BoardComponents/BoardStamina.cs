using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardStamina : MonoBehaviour
{
    public int MaxStamina;
    public int CurrentStamina;

    public bool CanDoCommand
    {
        get { return CurrentStamina > 0; }
    }


    private void Start()
    {
        CurrentStamina = MaxStamina;
    }

    public void ResetStaminaToMax()
    {
        CurrentStamina = MaxStamina;
    }



}
