using System;
using System.Collections.ObjectModel;

public class Direction {
    public static readonly Direction north = new Direction( 0,  1);
    public static readonly Direction south = new Direction( 0, -1);
    public static readonly Direction east = new Direction( 1,  0);
    public static readonly Direction west = new Direction(-1,  0);
    public static readonly ReadOnlyCollection<Direction> Values = new ReadOnlyCollection<Direction>(new Direction[] {north, south, east, west});
    public readonly int x;
    public readonly int z;

    private Direction(int x, int z) {
        this.x = x;
        this.z = z;
    }

    public Direction Rotate(int times) {
        times = ((times % Values.Count) + Values.Count) % Values.Count;
        Direction dir = this;
        for (int i = 0; i < times; i++) {
            dir = dir.Rotate();
        }
        return dir;
    }

    public Direction Rotate() {
        if (this == north) return east;
        if (this == east) return south;
        if (this == south) return west;
        if (this == west) return north;
        throw new Exception("Unknown Direction");
    }

    public Direction Opposite { get {
        if (this == north) return south;
        if (this == south) return north;
        if (this == east) return west;
        if (this == west) return east;
        throw new Exception("Unknown Direction");
    }}
}