using System.Collections.Generic;
using UnityEngine;

public class Cell {
    private PrototypeCollection prototypes;
    private StringSet superposition;
    public string current;

    public float entropy;

    public StringSet N = new StringSet();
    public StringSet S = new StringSet();
    public StringSet E = new StringSet();
    public StringSet W = new StringSet();

    public Cell(PrototypeCollection allStates) {
        this.superposition = new StringSet();
        this.prototypes = allStates;
        Reset();
    }

    public StringSet getSuperposition() {
        return new StringSet(this.superposition);
    }

    public bool CanBe(string key) {
        return this.superposition.Contains(key);
    }

    public bool Intersect(StringSet other) {
        bool changed = this.superposition.IntersectWith(other);
        UpdateEntropy();
        UpdateNeighbors();
        
        return changed;
    }

    public bool isResolved() {
        return this.current != null; //.Count == 0;
    }

    public void Reset() {
        this.superposition.UnionWith(prototypes.Keys);
        this.current = null;
        UpdateEntropy();
        UpdateNeighbors();
    }

    public void Collapse() {
        int index = (int)Random.Range(0, superposition.Count);
        this.current = superposition.ElementAt(index);

        this.superposition.Clear();
        this.superposition.Add(this.current);

        UpdateNeighbors();
        UpdateEntropy();
    }

    private void UpdateNeighbors() {
        this.N.Clear();
        this.S.Clear();
        this.E.Clear();
        this.W.Clear();

        foreach (string possibility in superposition) {
            TilePrototype proto = prototypes[possibility];
            this.N.UnionWith(proto.neighbor_pZ);
            this.S.UnionWith(proto.neighbor_nZ);
            this.E.UnionWith(proto.neighbor_pX);
            this.W.UnionWith(proto.neighbor_nX);
        }
    }

    private void UpdateEntropy() {
        this.entropy = this.superposition.Count;
    }
}