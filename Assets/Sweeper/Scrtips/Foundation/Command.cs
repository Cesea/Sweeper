﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foundation
{
    [System.Serializable]
    public class Command
    {
        protected int _cost;
        public int Cost { get { return _cost; } }

        public virtual bool Execute(GameObject target)
        {
            BoardStamina stamina = target.GetComponent<BoardStamina>();
            if (stamina != null && stamina.Consume(_cost))
            {
                return true;
            }
            return false;
        }
    }
}
