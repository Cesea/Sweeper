using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Foundation;

//public class SkillCommand : Command
//{
//    private int _skillIndex = -1;

//    public SkillCommand(int index, Vector3 pos  )
//    {
//    }

//    public override bool Execute(GameObject target)
//    {
//    }
//}
//public class System
//{
//    public virtual void Exeucte(Command command)
//    {

//    }
//}

//public class SkillSystem : System
//{
//    Skill[]  _skills = new 

//    public override void Exeucte(Command command)
//    {
//        SkillCommand castedCommand = command as SkillCommand;
//        castedCommand.
//    }

//}

[System.Serializable]
public class InstallObjectCommand : Command
{
    public NodeSideInfo _nodeInfo;
    public int PrefabIndex { get; set; }
    public Vector3 Offset { get; set; }

    public InstallObjectCommand(NodeSideInfo info)
    {
        _nodeInfo = info;
        _cost = 2;
        Offset = Vector3.zero;
    }

    public override bool Execute(GameObject target)
    {
        if (base.Execute(target))
        {
            Level.LevelCreator.Instance.InstallObjectAtNode(_nodeInfo, PrefabIndex, 
                Offset, 
                BoardManager.SideToRotation(_nodeInfo._side));
            return true;
        }
        else
        {
            return false;
        }
    }

}
