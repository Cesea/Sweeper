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

        GUIContent visualize = new GUIContent("Visualize Arrangement", "Press this to preview what the radial menu will look like ingame.");
        GUIContent reset = new GUIContent("Reset Arrangement", "Press this to reset all elements to a 0 rotation for easy editing.");

        if (!Application.isPlaying)
        {
            if (GUILayout.Button(visualize))
            {
                ArrangeElementsInEditor(_target, false);

            }

            if (GUILayout.Button(reset))
            {
                ArrangeElementsInEditor(_target, true);
            }
        }
    }

    public void ArrangeElementsInEditor(RadialMenu menu, bool reset)
    {
        if (reset)
        {
            for (int i = 0; i < menu._elements.Count; i++)
            {
                if (menu._elements[i] == null)
                {
                    Debug.LogError("Radial Menu: element " + i.ToString() + " in the radial menu " + menu.gameObject.name + " is null!");
                    continue;
                }
                RectTransform rectTransform = menu._elements[i].GetComponent<RectTransform>();
                rectTransform.rotation = Quaternion.Euler(0, 0, 0);
                RectTransform textTransform = menu._elements[i]._text.GetComponent<RectTransform>();
                textTransform.rotation = Quaternion.Euler(0, 0, 0);
            }
            return;
        }

        for (int i = 0; i < menu._elements.Count; ++i)
        {
            if (menu._elements[i] == null)
            {
                Debug.LogError("Radial Menu: element " + i.ToString() + " in the radial menu " + menu.gameObject.name + " is null!");
                continue;
            }
            RectTransform rectTransform = menu._elements[i].GetComponent<RectTransform>();
            rectTransform.rotation = Quaternion.Euler(0, 0, -((360f / (float)menu._elements.Count) * i) - menu._globalOffset);

            RectTransform textTransform = menu._elements[i]._text.GetComponent<RectTransform>();
            textTransform.localRotation = Quaternion.Euler(0, 0, ((360f / (float)menu._elements.Count) * i) - menu._globalOffset);

            int a = 0;
        }

    }



}
