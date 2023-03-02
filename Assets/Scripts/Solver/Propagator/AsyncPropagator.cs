using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AsyncPropagator
{
    public static async Task<bool> Propagate(Grid grid, Position seed) {
        Queue<Position> toUpdate = new Queue<Position>(new Position[] {seed});
        while (toUpdate.Count > 0) {
            Position here = toUpdate.Dequeue();
            GridCell current = grid[here];
            foreach (Direction dir in grid.GetValidDirections(here)) {
                Position neighbor = here + dir;
                grid[neighbor].Intersect(current.ValidNeighborValues(dir));
                if (grid[neighbor].IsInvalid()) {
                    return false;
                }
            }
            await Task.Yield();
        }
        return true;
    }
}
