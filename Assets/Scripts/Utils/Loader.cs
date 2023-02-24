using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
public class Loader {
    public static Dictionary<string, GameObject> LoadPrefabs() {
        var prefabs = new Dictionary<string, GameObject>();
        foreach (GameObject prefab in Resources.LoadAll<GameObject>("Tiles")) {
            prefabs.Add(prefab.name, prefab);
        }
        return prefabs;
    }

    public static PrototypeDict LoadPrototypes() {
        TextAsset protoText = Resources.Load<TextAsset>("Tiles/TilePrototypes");
        return JsonConvert.DeserializeObject<PrototypeDict>(protoText.text);
    }
}