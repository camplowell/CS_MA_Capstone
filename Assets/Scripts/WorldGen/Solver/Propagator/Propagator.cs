using System.Collections.Generic;

public class Propagator {
    public static bool Propagate(Grid grid, Position seed) {
        var toUpdate = new Queue<Position>(new Position[] {seed});

        while (toUpdate.Count > 0) {
            var here = toUpdate.Dequeue();
            var current = grid[here];

            foreach (var dir in grid.ValidDirections(here)) {
                Cell neighbor = grid[here + dir];
                neighbor.Intersect(current, dir);
                if (neighbor.IsInvalid) {
                    return true;
                }
            }
        }
        return false;
    }
}