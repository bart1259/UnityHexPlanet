using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityHexPlanet {

    [CustomEditor(typeof(HexPlanetManager))]
    public class PlanetGeneratorEditor : Editor
    {

        private Editor _planetEditor;
        private Editor _generatorEditor;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            HexPlanetManager t = (HexPlanetManager)target;

            // Draw Planet GUI
            if(t.hexPlanet != null) {
                EditorGUI.indentLevel++;
                if(_planetEditor == null) {
                    _planetEditor = Editor.CreateEditor(t.hexPlanet);
                }
                _planetEditor.DrawDefaultInspector();

                // Draw Terrain generator GUI
                if(t.hexPlanet.terrainGenerator != null) {
                    EditorGUI.indentLevel++;
                    if(_generatorEditor == null) {
                        _generatorEditor = Editor.CreateEditor(t.hexPlanet.terrainGenerator);
                    }
                    _generatorEditor.DrawDefaultInspector();
                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;

                if(GUILayout.Button("Regenerate")) {
                    t.UpdateRenderObjects();
                }
            } else {
                EditorStyles.label.wordWrap = true;
                EditorGUILayout.LabelField("Attach a hexPlanet instance above to be able to generate planet geometry. One can be made by right-clicking in the project explorer -> Create -> HexPlanet -> HexPlanet", GUILayout.Height(50));
            }
        }
    }
}