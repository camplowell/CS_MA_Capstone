using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

public class Prototype {
    [JsonProperty] public readonly string key;
    [JsonProperty] public readonly string model;
    [JsonProperty] public readonly int rot;
    [JsonProperty] public readonly int elevation;

    [JsonProperty] public readonly float weight;

    [JsonProperty] public readonly ReadOnlyCollection<string> nX;
    [JsonProperty] public readonly ReadOnlyCollection<string> pX;
    [JsonProperty] public readonly ReadOnlyCollection<string> nZ;
    [JsonProperty] public readonly ReadOnlyCollection<string> pZ;

    public Prototype(
        string key,
        string model,
        int rot,
        int elevation,
        float weight,
        List<string> nX,
        List<string> pX,
        List<string> nZ,
        List<string> pZ
    ) {
        this.key = key;
        this.model = model;
        this.rot = rot;
        this.elevation = elevation;
        this.weight = weight;
        this.nX = new ReadOnlyCollection<string>(nX);
        this.pX = new ReadOnlyCollection<string>(pX);
        this.nZ = new ReadOnlyCollection<string>(nZ);
        this.pZ = new ReadOnlyCollection<string>(pZ);
    }
}