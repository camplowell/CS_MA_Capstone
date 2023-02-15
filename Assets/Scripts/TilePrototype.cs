using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class TilePrototype
{
    public string protoID;
    public string objName;
    public int rotation;
    [JsonConverter(typeof(StringSetConverter))] public StringSet neighbor_pZ;
    [JsonConverter(typeof(StringSetConverter))] public StringSet neighbor_pX;
    [JsonConverter(typeof(StringSetConverter))] public StringSet neighbor_nZ;
    [JsonConverter(typeof(StringSetConverter))] public StringSet neighbor_nX;

    public TilePrototype() {
        this.neighbor_pZ = new StringSet();
        this.neighbor_pX = new StringSet();
        this.neighbor_nZ = new StringSet();
        this.neighbor_nX = new StringSet();
    }
}
