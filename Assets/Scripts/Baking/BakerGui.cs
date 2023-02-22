using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(TileBaker))]
public class BakerGui : Editor
{
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        if (GUILayout.Button("Bake Tile Constraints")) {
            TileBaker baker = (TileBaker)target;
            baker.Bake();
        }
    }
}