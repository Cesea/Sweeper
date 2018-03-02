using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class InGameHUD : Menu<InGameHUD>
{
    public Text _healthText;
    public Text _staminaText;

    public static void Show(PlayerBoardObject boardObject)
    {
        Open();

        Instance._healthText.text =  boardObject.GetComponent<BoardHealth>().CurrentHealth.ToString();
        Instance._staminaText.text = boardObject.GetComponent<BoardStamina>().CurrentStamina.ToString();
    }

    public static void Shut()
    {
        Close();
    }

    private void OnEnable()
    {
        EventManager.Instance.AddListener<Events.PlayerHealthChanged>(OnHealthChanged);
        EventManager.Instance.AddListener<Events.PlayerStaminaChanged>(OnStaminaChanged);
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveListener<Events.PlayerHealthChanged>(OnHealthChanged);
        EventManager.Instance.RemoveListener<Events.PlayerStaminaChanged>(OnStaminaChanged);
    }

    public void OnHealthChanged(Events.PlayerHealthChanged e)
    {
        _healthText.text = e._value.ToString();
    }

    public void OnStaminaChanged(Events.PlayerStaminaChanged e)
    {
        _staminaText.text = e._value.ToString();
    }

}
