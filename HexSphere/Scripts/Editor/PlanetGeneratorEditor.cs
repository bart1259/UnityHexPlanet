using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HexPlanetRenderer))]
public class PlanetGeneratorEditor : Editor
{

    private Editor _editor;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        HexPlanetRenderer t = (HexPlanetRenderer)target;

        EditorGUI.indentLevel++;
        serializedObject.Update();
        var hexPlanetAsset = serializedObject.FindProperty("hexPlanet");
        CreateCachedEditor(hexPlanetAsset.objectReferenceValue, null, ref _editor);
        _editor.OnInspectorGUI();
        serializedObject.ApplyModifiedProperties();
        EditorGUI.indentLevel--;

        if(GUILayout.Button("Regenerate")) {
            t.UpdateRenderObjects();
        }
    }
}
