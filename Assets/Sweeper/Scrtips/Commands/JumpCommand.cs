using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Foundation;

[System.Serializable]
public class JumpCommand : Command
{
    public override void Execute(GameObject target)
    {
        SkillManager skillManager = target.GetComponent<SkillManager>();
    }
}
