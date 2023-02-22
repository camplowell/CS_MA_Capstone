using System;
using System.Collections.Generic;

class PrototypeBuilder {
    public string key;
    public string model;
    public int rot;
    public int elevation;

    public float weight;

    public List<string> nX = new List<string>();
    public List<string> pX = new List<string>();
    public List<string> nZ = new List<string>();
    public List<string> pZ = new List<string>();

    public Dictionary<Corner, CornerParams> cornerParams = new Dictionary<Corner, CornerParams>();
    public Dictionary<Border, string> borderParams = new Dictionary<Border, string>();

    public PrototypeBuilder(TileParams param, string model, int rot, int elevation) {
        this.rot = rot;
        this.elevation = elevation;
        this.model = model;
        this.weight = param.weight;

        foreach (Corner corner in Enum.GetValues(typeof(Corner))) {
            CornerParams cornerParam = param.GetCorner(corner.Rotate(rot));
            this.cornerParams.Add(corner, new CornerParams(cornerParam.biome, cornerParam.height + elevation));
        }
        foreach (Border border in Enum.GetValues(typeof(Border))) {
            this.borderParams.Add(border, param.GetBorder(border.Rotate(rot)));
        }

        this.key = GenerateKey();
    }

    public void FindValidNeighbors(IEnumerable<PrototypeBuilder> builders) {
        foreach (PrototypeBuilder neighbor in builders) {
            if (CanBorder(neighbor, Border.pZ)) {
                this.pZ.Add(neighbor.key);
            }
            if (CanBorder(neighbor, Border.nX)) {
                this.nX.Add(neighbor.key);
            }
            if (CanBorder(neighbor, Border.nZ)) {
                this.nZ.Add(neighbor.key);
            }
            if (CanBorder(neighbor, Border.pX)) {
                this.pX.Add(neighbor.key);
            }
        }
    }

    public Prototype Build() {
        return new Prototype (
            key,
            model,
            rot,
            elevation,
            weight,
            nX,
            pX,
            nZ,
            pZ
        );
    }

    public CornerParams GetCornerParams(Corner corner) {
        return this.cornerParams[corner];
    }

    public string GetBorderFeature(Border border) {
        return this.borderParams[border];
    }

    public bool CanBorder(PrototypeBuilder other, Border border) {
        Border opposite = border.Opposite();
        var adjacent = border.GetAdjacent();
        var oppositeAdjacent = opposite.GetAdjacent();
        bool sameFeatures = this.GetBorderFeature(border) == other.GetBorderFeature(opposite);
        bool lMatch = this.GetCornerParams(adjacent.l).Equals(other.GetCornerParams(oppositeAdjacent.r));
        bool rMatch = this.GetCornerParams(adjacent.r).Equals(other.GetCornerParams(oppositeAdjacent.l));

        return sameFeatures && lMatch && rMatch;
    }

    private string GenerateKey() {
        return model + "_r" + rot + "-" + elevation;
    }
}