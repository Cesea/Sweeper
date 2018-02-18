using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public class GameEvent
    {
    }

    public class GameStart : GameEvent
    {
    }


    public class ChangeScene : GameEvent
    {
        public int SceneIndex { get; set; }
        public bool LoadAsync { get; set; }

        public ChangeScene(int index, bool async)
        {
            SceneIndex = index;
            LoadAsync = async;
        }
    }

    public class BuildVisuals : GameEvent
    {
    }

    public class NextFloor : GameEvent
    {
    }

}

