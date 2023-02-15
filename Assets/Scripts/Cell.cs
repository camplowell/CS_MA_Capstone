using System.Collections.Generic;
using UnityEngine;

public class Cell {
    private PrototypeCollection prototypes;
    private StringSet superposition;
    public string current;

    public float entropy;

    public StringSet N;
    public StringSet S;
    public StringSet E;
    public StringSet W;

    public Cell(PrototypeCollection allStates) {
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

        this.entropy = this.superposition.Count;
        
        return changed;
    }

    public bool isResolved() {
        return this.superposition.Count == 0;
    }

    public void Reset() {
        this.superposition.UnionWith(prototypes.Keys);
        this.current = null;
    }

    public void Collapse() {
        int index = (int)Random.Range(0, superposition.Count);
        this.current = superposition.ElementAt(index);
    }
}