using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Drawer))]
public class DrawerEditor : Editor {

    Drawer drawer;

    private void OnEnable()
    {
        drawer = (Drawer)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Update"))
        {
            drawer.Show();
        }
    }

}
