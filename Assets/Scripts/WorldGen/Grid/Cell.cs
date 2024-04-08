using System;
using System.Collections.Generic;

public class Cell
{
    public string current { get; private set; }
    public bool collapsed { get; private set; }
    public float entropy { get; private set; }
    public bool IsInvalid => superposition.Count == 0;
    public Prototype prototype => current == null ? null : this.prototypes[current];

    HashSet<string> superposition = new HashSet<string>();
    Dictionary<Direction, HashSet<string>> validNeighbors;
    public readonly PrototypeDict prototypes;


    public Cell(PrototypeDict prototypes, string current = null) {
        this.prototypes = prototypes;
        this.validNeighbors = new Dictionary<Direction, HashSet<string>>();
        foreach (var dir in Direction.Values) {
            validNeighbors[dir] = new HashSet<string>();
        }
        Reset();
    }

    public Cell(Cell copy) {
        this.current = copy.current;
        this.collapsed = copy.collapsed;
        this.prototypes = copy.prototypes;
        this.superposition = new HashSet<string>(copy.superposition);
        this.validNeighbors = new Dictionary<Direction, HashSet<string>>();
        foreach (var dir in Direction.Values) {
            validNeighbors[dir] = new HashSet<string>();
        }
        Update();
    }
    
    public void Collapse(string to) {
        this.superposition.IntersectWith(new string[] {to});
        this.collapsed = true;
        this.current = to;
        Update();
    }

    public void Collapse(Random random) {

        if (IsInvalid) {
            throw new InvalidOperationException("Calling Collapse on an invalid cell");
        }
        var cumulativeWeights = new List<float>();
        var positions = new List<string>();
        float totalWeight = 0f;

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
        try {
            Collapse(positions[choice]);
        } catch (ArgumentOutOfRangeException) {
            throw new ArgumentOutOfRangeException(string.Format("Choice: {0}; positions.Count: {1}", choice, positions.Count));
        }
    }

    public bool Intersect(Cell other, Direction toOther) {
        var possibilities = other.validNeighbors[toOther];
        int prevPositions = superposition.Count;
        this.superposition.IntersectWith(possibilities);
        if (this.superposition.Count == prevPositions) { return false; }
        Update();
        return true;
    }

    public void Reset() {
        this.current = null;
        this.collapsed = false;

        this.superposition.Clear();
        foreach (var prototype in prototypes) {
            this.superposition.Add(prototype.key);
        }

        Update();
    }

    void Update() {
        foreach (var set in this.validNeighbors.Values) {
            set.Clear();
        }

        float totalWeight = 0;
        float maxWeight = 0;
        foreach (var state in this.superposition) {
            var proto = this.prototypes[state];
            totalWeight += proto.weight;
            maxWeight = MathF.Max(maxWeight, proto.weight);
            foreach (var dir in Direction.Values) {
                this.validNeighbors[dir].UnionWith(proto.ValidNeighbors(dir));
            }
        }
        this.entropy = totalWeight < 0.0001 ? 0 : 1 - (maxWeight / totalWeight);
    }
}