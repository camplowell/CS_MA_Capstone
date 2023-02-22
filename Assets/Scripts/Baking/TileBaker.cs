using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class TileBaker : MonoBehaviour
{
    public static string path = "Assets/Resources/Tiles/TilePrototypes.json";
    public void Bake() {
        var builders = InitializeBuilders();
        var prototypes = MakePrototypes(builders);
        SavePrototypes(prototypes);
    }
    private List<PrototypeBuilder> InitializeBuilders() {
        List<PrototypeBuilder> builders = new List<PrototypeBuilder>();
        foreach (GameObject tile in Resources.LoadAll<GameObject>("Tiles")) {
            for (int rot = 0; rot < 360; rot += 90) {
                var tileParams = tile.GetComponent<TileParams>();
                builders.Add(new PrototypeBuilder(tileParams, tile.name, rot, 0));
            }
            Debug.Log("Initialized prototype builders: " + tile.name);

        }
        return builders;
    }
    private PrototypeDict MakePrototypes(List<PrototypeBuilder> builders) {
        PrototypeDict prototypes = new PrototypeDict();
        foreach (PrototypeBuilder builder in builders) {
            builder.FindValidNeighbors(builders);
            Debug.Log("Added " + (builder.nX.Count + builder.nZ.Count + builder.pX.Count + builder.pZ.Count) + " neighbors: " + builder.key);
            prototypes.Add(builder.Build());
        }
        return prototypes;
    }

    private void SavePrototypes(PrototypeDict prototypes) {
        string str = JsonConvert.SerializeObject(prototypes, Formatting.Indented);
        using (FileStream fs = new FileStream(path, FileMode.Create)) {
            using (StreamWriter writer = new StreamWriter(fs)) {
                writer.Write(str);
                Debug.Log("Done");
            }
        }
    }
}