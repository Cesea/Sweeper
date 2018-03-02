using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class InGameHUD : Menu<InGameHUD>
{
    public Text _healthText;
    public Text _staminaText;

    public Button _endTurnButton;

    public Text _turnLabel;

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
        EventManager.Instance.AddListener<Events.PlayerTurnEvent>(OnPlayerTurnEvent);
        EventManager.Instance.AddListener<Events.EnemyTurnEvent>(OnEnemyTurnEvent);
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveListener<Events.PlayerHealthChanged>(OnHealthChanged);
        EventManager.Instance.RemoveListener<Events.PlayerStaminaChanged>(OnStaminaChanged);
        EventManager.Instance.AddListener<Events.PlayerTurnEvent>(OnPlayerTurnEvent);
        EventManager.Instance.AddListener<Events.EnemyTurnEvent>(OnEnemyTurnEvent);
    }

    public void OnHealthChanged(Events.PlayerHealthChanged e)
    {
        _healthText.text = e._value.ToString();
    }

    public void OnStaminaChanged(Events.PlayerStaminaChanged e)
    {
        _staminaText.text = e._value.ToString();
    }

    public void OnPlayerTurnEvent(Events.PlayerTurnEvent e)
    {
        _turnLabel.text = "Player Turn";
    }

    public void OnEnemyTurnEvent(Events.EnemyTurnEvent e)
    {
        _turnLabel.text = "Enemy Turn";
    }

    public void OnEndTurnButtonPressed()
    {
        GameStateManager.Instance.TurnChanged = true;
    }
}
