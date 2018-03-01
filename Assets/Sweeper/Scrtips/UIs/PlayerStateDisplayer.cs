using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class PlayerStateDisplayer : Menu<PlayerStateDisplayer>
{
    private PlayerBoardObject _player;
    private BoardHealth _playerHealth;
    private BoardStamina _playerStamina;

    public static void Show()
    {
        Open();
    }

    public static void Shut()
    {
        Close();
    }

    public void OnHealthChanged(int currentHealth)
    {

    }

    public void OnStaminaChanged(int currentStamina)
    {
    }

    public void OnActionPointChanged(int currentActionPoint)
    {

    }

}
