using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foundation
{
    [System.Serializable]
    public class Command
    {
        protected int _cost;

        public virtual void Execute(GameObject target)
        {
            BoardStamina stamina = target.GetComponent<BoardStamina>();
            stamina.CurrentStamina -= _cost;
        }
    }
}
