﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public class GameEvent
    {
    }

    public class RadialShutEvent : GameEvent
    {
    }

    public class RadarSkillEvent : GameEvent
    {
        public Vector3 Origin { get; set; }
        public RadarSkillEvent(Vector3 origin)
        {
            Origin = origin;
        }
    }
}
