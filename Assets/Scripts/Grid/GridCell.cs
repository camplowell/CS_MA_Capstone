using System;
using System.Collections.Generic;

public class GridCell {
    public string current => _current;
    public bool collapsed => _collapsed;
    public float entropy => _entropy;
    public HashSet<string> superposition = new HashSet<string>();

    private PrototypeDict prototypes;
    private HashSet<string> pZ = new HashSet<string>();
    private HashSet<string> nZ = new HashSet<string>();
    private HashSet<string> pX = new HashSet<string>();
    private HashSet<string> nX = new HashSet<string>();
    private float _entropy;
    private string _current = null;
    private bool _collapsed = false;

    public GridCell(PrototypeDict prototypes, string current = null) {
        this.prototypes = prototypes;
        this._current = current;
        Reset(false);
    }

    public void Collapse(string to) {
        this.superposition.IntersectWith(new string[] {to});
        this._collapsed = true;
        this._current = to;
        Update();
    }

    public void Collapse(Random random) {
        List<float> cumulativeWeights = new List<float>();
        List<string> positions = new List<string>();
        float totalWeight = 0.0f;

        foreach (string key in this.superposition) {
            Prototype proto = this.prototypes[key];
            totalWeight += proto.weight;

            positions.Add(key);
            cumulativeWeights.Add(totalWeight);
        }

        int choice = cumulativeWeights.BinarySearch((float)random.NextDouble() * totalWeight);
        if (choice < 0) {
            choice = ~choice;
        }

        Collapse(positions[choice]);
    }

    public bool Intersect(IEnumerable<string> possibilities) {
        int prevPositions = superposition.Count;
        this.superposition.IntersectWith(possibilities);
        if (this.superposition.Count == prevPositions) {
            return false;
        }
        Update();
        return true;
    }

    public bool IsInvalid() {
        return this.superposition.Count == 0;
    }

    public HashSet<string> ValidNeighborValues(Direction dir) {
        return  (dir == Direction.pZ) ? this.pZ :
                (dir == Direction.nZ) ? this.nZ :
                (dir == Direction.pX) ? this.pX :
                this.nX;
    }

    public void Reset(bool clearCurrent = true) {
        if (clearCurrent) {
            this._current = null;
        }
        this._collapsed = false;
        foreach (Prototype proto in prototypes) {
            this.superposition.Add(proto.key);
        }
        Update();
    }

    public Prototype Prototype() {
        if (this._current == null) {
            return null;
        }
        return this.prototypes[this._current];
    }

    private void Update() {
        this.pZ.Clear();
        this.nZ.Clear();
        this.pX.Clear();
        this.nX.Clear();

        foreach (string position in this.superposition) {
            Prototype proto = prototypes[position];
            this.pZ.UnionWith(proto.pZ);
            this.nZ.UnionWith(proto.nZ);
            this.pX.UnionWith(proto.pX);
            this.nX.UnionWith(proto.nX);
        }

        this._entropy = this.superposition.Count;
    }
}