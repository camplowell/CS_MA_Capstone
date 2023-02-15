using System.Collections.Generic;
using UnityEngine;

public class Cell {
    private PrototypeCollection prototypes;
    private StringSet superposition;
    public string current;

    public StringSet N;
    public StringSet S;
    public StringSet E;
    public StringSet W;

    public Cell(PrototypeCollection allStates) {
        prototypes = allStates;
        Reset();
    }

    public StringSet getSuperposition() {
        return new StringSet(superposition);
    }

    public bool CanBe(string key) {
        return superposition.Contains(key);
    }

    public bool Intersect(StringSet other) {
        bool changed = superposition.IntersectWith(other);
        N.Clear();
        S.Clear();
        E.Clear();
        W.Clear();

        foreach (string possibility in superposition) {
            TilePrototype proto = prototypes[possibility];
            N.UnionWith(proto.neighbor_pZ);
            S.UnionWith(proto.neighbor_nZ);
            E.UnionWith(proto.neighbor_pX);
            W.UnionWith(proto.neighbor_nX);
        }
        
        return changed;
    }

    public bool isResolved() {
        return superposition.Count == 0;
    }

    public void Reset() {
        superposition = new StringSet(prototypes.Keys);
    }

    public void Collapse() {
        int index = (int)Random.Range(0, superposition.Count);
        current = superposition.ElementAt(index);
    }
}