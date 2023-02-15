using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(PrototypeBaker))]
public class BakerGui : Editor
{
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        if (GUILayout.Button("Bake Tile Constraints")) {
            PrototypeBaker baker = (PrototypeBaker)target;
            baker.BakePrototypes();
        }
    }
}