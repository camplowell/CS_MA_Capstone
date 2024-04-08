#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class PrototypeBaker : MonoBehaviour {
    public static PrototypeBaker instance;
    public int height;
    public string path = "Tiles/TilePrototypes";

    void OnValidate()
    {
        instance = this;
    }
}


[CustomEditor(typeof(PrototypeBaker))]
public class PrototypeBakerGUI : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Bake Tile Adjacencies")) {
            PrototypeBaker generator = (PrototypeBaker)target;
            
            var builders = InitializeBuilders(generator.height);
            BakeAdjacencies(builders);
            var prototypes = MakePrototypes(builders);
            SavePrototypes(prototypes, generator.path);
        }
    }

    List<PrototypeBuilder> InitializeBuilders(int height) {
        var builders = new List<PrototypeBuilder>();
        var prefabs = Resources.LoadAll<GameObject>("Tiles");
        float maxHeight = 0;
        foreach (var tile in prefabs) {
            maxHeight = Mathf.Max(maxHeight, GetHeight(tile));
            var tileParams = tile.GetComponent<TileParams>();
            int tileElevations = height - tileParams.MaxHeight;
            for (int elevation = 0; elevation <= tileElevations; elevation++) {
                for (int rot = 0; rot < Direction.Values.Count; rot++) {
                    builders.Add(new PrototypeBuilder(tileParams, rot, elevation));
                }
            }
        }
        Debug.Log("Initialized " + builders.Count + " tiles from " + prefabs.Length + " prefabs");
        return builders;
    }

    void BakeAdjacencies(List<PrototypeBuilder> builders) {
        foreach (var builder in builders) {
            builder.FindValidNeighbors(builders);
        }
        Debug.Log("Found valid adjacencies");
    }

    PrototypeDict MakePrototypes(List<PrototypeBuilder> builders) {
        var dict = new PrototypeDict();
        foreach (var builder in builders) {
            dict.Add(builder.Build());
        }
        Debug.Log("Made dictionary");
        return dict;
    }

    void SavePrototypes(PrototypeDict prototypes, string path) {
        string serialized = JsonConvert.SerializeObject(prototypes, Formatting.Indented);
        using (var fs = new FileStream("Assets/Resources/" + path + ".json", FileMode.Create)) {
            using (var writer = new StreamWriter(fs)) {
                writer.Write(serialized);
                Debug.Log("Done");
            }
        }
    }

    float GetHeight(GameObject prefab) {
        Bounds bounds = new Bounds();
        foreach (Renderer renderer in prefab.GetComponentsInChildren<Renderer>()) {
            bounds.Encapsulate(renderer.bounds);
        }
        return bounds.extents.y;
    }
}

#endif