using System.Collections;
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

    public class PlayerHealthChanged : GameEvent
    {
        public int _value; 

        public PlayerHealthChanged(int value)
        {
            _value = value;
        }
    }

    public class PlayerStaminaChanged : GameEvent
    {
        public int _value;

        public PlayerStaminaChanged(int value)
        {
            _value = value;
        }
    }

    public class LevelCreatorMenuEvent : GameEvent
    {
        public bool Opened { get; set; }
        public LevelCreatorMenuEvent(bool opened)
        {
            Opened = opened;
        }
    }

    #region Turn Events
    public class PlayerTurnEvent : GameEvent
    {
    }

    public class EnemyTurnEvent : GameEvent
    {
    }
    #endregion

    #region Enemy State Events
    public class EnemyChaseEvent : GameEvent
    {
        public int _enemyID;
        public EnemyChaseEvent(int id)
        {
            _enemyID = id;
        }
    }
    public class EnemyFleeEvent : GameEvent
    {
        public int _enemyID;
        public EnemyFleeEvent(int id)
        {
            _enemyID = id;
        }
    }
    public class EnemyIdleEvent : GameEvent
    {
        public int _enemyID;
        public EnemyIdleEvent(int id)
        {
            _enemyID = id;
        }
    }
    public class EnemyPatrolEvent : GameEvent
    {
        public int _enemyID;
        public EnemyPatrolEvent(int id)
        {
            _enemyID = id;
        }
    }
    #endregion

    public class PlayerPositionEvent : GameEvent
    {
        private NodeSideInfo _sittingInfo;
        public NodeSideInfo SittingInfo { get { return _sittingInfo; } }
        public PlayerPositionEvent(NodeSideInfo info)
        {
            _sittingInfo = info;
        }
    }

}

