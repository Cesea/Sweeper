using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorBase : MonoBehaviour
{
    protected NodeSideInfo _selectingInfo;
    protected NodeSideInfo _prevSelectingInfo;

    public virtual void LocateCursor()
    {
    }

    public virtual void HandleInput()
    {
    }

    public virtual void OnCursorEnabled()
    {
    }

    public virtual void UpdateSelectingInfos(NodeSideInfo selecting, NodeSideInfo prev)
    {
        _selectingInfo = selecting;
        _prevSelectingInfo = prev;
    }
}
