using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[CustomEditor(typeof(RadialMenu))]
public class RadialMenuEditor : Editor
{
    private RadialMenu _target;

    private void OnEnable()
    {
        _target = target as RadialMenu;
    }

    private void OnDisable()
    {
        _target = null;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }

}
