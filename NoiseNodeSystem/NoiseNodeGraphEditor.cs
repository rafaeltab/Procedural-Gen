using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NoiseNodeGraphWditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorUtility.SetDirty(target);
    }
}
