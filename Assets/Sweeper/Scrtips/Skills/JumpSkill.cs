using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class JumpSkill : Skill
{
    public Vector3 JumpStartPos { get; set; }
    public Vector3 JumpEndPos { get; set; }

    public JumpSkill(Vector3 jumpStart, Vector3 jumpEnd)
    {
        JumpStartPos = jumpStart;
        JumpEndPos = jumpEnd;
    }

    public override void Activate(GameObject instance)
    {
    }
}
