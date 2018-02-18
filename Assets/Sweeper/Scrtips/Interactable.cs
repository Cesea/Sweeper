using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Foundation;

public class Interactable : MonoBehaviour
{
    private List<Command> _commandBuffer = new List<Command>();

    private void OnEnable()
    {
        EventManager.Instance.AddListener<Events.RadialShutEvent>(ClearCommandBuffer);
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveListener<Events.RadialShutEvent>(ClearCommandBuffer);
    }

    public Command[] GetAvailableCommands()
    {
        _commandBuffer.AddRange(CommandBuilder.BuildCommands(gameObject));
        return _commandBuffer.ToArray();
    }

    public void DoCommand(int index)
    {
        if (index > _commandBuffer.Count - 1)
        {
            Debug.Log("Command index out of range");
            return;
        }
        _commandBuffer[index].Execute(GameStateManager.Instance.Player.gameObject);
        _commandBuffer.Clear();
    }


    public void ClearCommandBuffer(Events.RadialShutEvent e)
    {
        _commandBuffer.Clear();
    }

}
