using System;
using System.Collections.Generic;
using UnityEngine;

public class PrototypeBuilder {
    public string key;
    public string prefab;
    public int rot;
    public int elevation;
    public float weight;

    public Dictionary<Direction, Edge> edges;
    public Dictionary<Direction, List<string>> validNeighbors;


    public PrototypeBuilder(TileParams tileParams, int rot, int elevation) {
        this.prefab = tileParams.gameObject.name;
        this.rot = rot;
        this.elevation = elevation;
        this.weight = tileParams.Weight;
        this.key = GenerateKey();

        this.validNeighbors = new Dictionary<Direction, List<string>>();
        this.edges = new Dictionary<Direction, Edge>();
        foreach (Direction dir in Direction.Values) {
            validNeighbors.Add(dir, new List<string>());
            edges.Add(dir, tileParams.edges[dir.Rotate(rot)]);
        }

    }
    public void FindValidNeighbors(IEnumerable<PrototypeBuilder> builders) {
        foreach (var builder in builders) {
            AddAdjacencies(builder);
        }
    }
    public Prototype Build() {
        return new Prototype(
            key: this.key,
            prefab: this.prefab,
            rot: this.rot,
            elevation: this.elevation,
            weight: this.weight,
            north: this.validNeighbors[Direction.north],
            south: this.validNeighbors[Direction.south],
            east: this.validNeighbors[Direction.east],
            west: this.validNeighbors[Direction.west]
        );
    }

    void AddAdjacencies(PrototypeBuilder other) {
        foreach (Direction dir in Direction.Values) {
            if (Edge.Matches(this.edges[dir], this.elevation, other.edges[dir.Opposite], other.elevation)) {
                this.validNeighbors[dir].Add(other.key);
            }
        }
    }

    string GenerateKey() {
        return prefab + "_r" + rot + "-" + elevation;
    }
}