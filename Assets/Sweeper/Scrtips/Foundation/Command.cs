using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foundation
{
    [System.Serializable]
    public class Command
    {
        public virtual void Execute(GameObject target)
        {
        }
    }
}
