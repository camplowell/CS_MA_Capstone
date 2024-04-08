using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

public class Prototype {
    [JsonProperty] public readonly string key;
    [JsonProperty] public readonly string prefab;
    [JsonProperty] public readonly int rot;
    [JsonProperty] public readonly int elevation;

    [JsonProperty] public readonly float weight;

    [JsonProperty] public readonly ReadOnlyCollection<string> north;
    [JsonProperty] public readonly ReadOnlyCollection<string> south;
    [JsonProperty] public readonly ReadOnlyCollection<string> east;
    [JsonProperty] public readonly ReadOnlyCollection<string> west;

    public Prototype(
        string key,
        string prefab,
        int rot,
        int elevation,
        float weight,
        List<string> north,
        List<string> south,
        List<string> east,
        List<string> west
    ) {
        this.key = key;
        this.prefab = prefab;
        this.rot = rot;
        this.elevation = elevation;
        this.weight = weight;
        this.north = new ReadOnlyCollection<string>(north);
        this.south = new ReadOnlyCollection<string>(south);
        this.east = new ReadOnlyCollection<string>(east);
        this.west = new ReadOnlyCollection<string>(west);
    }

    public ReadOnlyCollection<string> ValidNeighbors(Direction dir) {
        if (dir == Direction.north) { return this.north; }
        if (dir == Direction.south) { return this.south; }
        if (dir == Direction.east) { return this.east; }
        if (dir == Direction.west) { return this.west; }
        throw new ArgumentException("Unknown Direction " + dir);
    }
}