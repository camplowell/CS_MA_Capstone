
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class PrototypeBaker : MonoBehaviour
{
    private string path = "Assets/Resources/Tiles/TileInfo.json";
    private Dictionary<string, PrototypeBuilder> protos;
    // Start is called before the first frame update
    public void BakePrototypes() {
        InitializeProtos();
        CacheValidNeighbors();
        SaveToJson();
    }

    void InitializeProtos() {
        this.protos = new Dictionary<string, PrototypeBuilder>();
        GameObject[] tiles = Resources.LoadAll<GameObject>("Tiles");
        foreach(GameObject tile in tiles) {
            string protoName = tile.name;
            for (int rot = 0; rot <= 270; rot += 90) {
                PrototypeBuilder builder = new PrototypeBuilder(tile, rot);
                protos.Add(builder.name, builder);
            }
        }
    }

    void CacheValidNeighbors() {
        foreach (PrototypeBuilder prototype in protos.Values) {
            prototype.AddNeighbors(protos);
        }
    }

    void SaveToJson() {
        PrototypeCollection collection = new PrototypeCollection();
        foreach(PrototypeBuilder builder in protos.Values) {
            collection.Add(builder.build());
        }
        
        string str = JsonConvert.SerializeObject(collection, Formatting.Indented);
        using (FileStream fs = new FileStream(path, FileMode.Create)) {
            using (StreamWriter writer = new StreamWriter(fs)) {
                writer.Write(str);
                Debug.Log("Done");
            }
        }
    }
}

