
public enum Border {
    
    pZ = 0,
    nX = 1,
    nZ = 2,
    pX = 3
}
public enum Corner {
    nn = 0,
    pn = 1,
    pp = 2,
    np = 3
}

public static class BoundaryExtensions {
    public static Border Rotate(this Border border, int rot) {
        return (Border)(((int)border + rot / 90) % 4);
    }

    public static Corner Rotate(this Corner corner, int rot) {
        return (Corner)(((int)corner + rot / 90) % 4);
    }

    public static (Corner l, Corner r) GetAdjacent(this Border border) {
        switch(border) {
            case Border.pZ:
                return (Corner.pp, Corner.np);
            case Border.nX:
                return (Corner.np, Corner.nn);
            case Border.nZ:
                return (Corner.nn, Corner.pn);
            default:
                return (Corner.pn, Corner.pp);
        }
    }

    public static Border Opposite(this Border border) {
        switch(border) {
            case Border.pZ: return Border.nZ;
            case Border.nX: return Border.pX;
            case Border.nZ: return Border.pZ;
            default: return Border.nX;
        }
    }
}