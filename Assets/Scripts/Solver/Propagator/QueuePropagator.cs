
using System.Collections.Generic;

public class QueuePropagator {

    public static bool Propagate(Grid grid, Position seed) {
        Queue<Position> toUpdate = new Queue<Position>(new Position[] {seed});
        while (toUpdate.Count > 0) {
            Position here = toUpdate.Dequeue();
            GridCell current = grid[here];
            foreach (Direction dir in grid.GetValidDirections(here)) {
                Position neighbor = here + dir;
                grid[neighbor].Intersect(current.ValidNeighborValues(dir));
                if (grid[neighbor].IsInvalid()) {
                    grid.Reset(clearCurrent: true);
                    return false;
                }
            }
        }
        return true;
    }
}