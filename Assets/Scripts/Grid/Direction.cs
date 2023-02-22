using System;
using System.Collections.ObjectModel;

public class Direction {
    public static readonly Direction pZ = new Direction( 0,  1);
    public static readonly Direction nZ = new Direction( 0, -1);
    public static readonly Direction pX = new Direction( 1,  0);
    public static readonly Direction nX = new Direction(-1,  0);
    public static readonly ReadOnlyCollection<Direction> Values = new ReadOnlyCollection<Direction>(new Direction[] {pZ, nZ, pX, nX});
    public readonly int x;
    public readonly int z;

    private Direction(int x, int z) {
        this.x = x;
        this.z = z;
    }
}