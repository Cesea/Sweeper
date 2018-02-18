using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarSkill : Skill
{
    private Vector3 _origin;
    public Vector3 Origin
    {
        get { return _origin; }
        set { _origin = value; }
    }

    public RadarSkill(Vector3 origin)
    {
        _origin = origin;
    } 

    public override void Activate()
    {
        EventManager.Instance.TriggerEvent(new Events.RadarSkillEvent(_origin));
    }
}
