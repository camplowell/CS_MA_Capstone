using System;
using System.Collections.Generic;

public class Collapser {
    public static Position Collapse(Grid grid, Random random, bool ignorePrev = true) {
        Position toCollapse = ChoosePosition(grid, random);
        Collapse(grid[toCollapse], random, ignorePrev);
        return toCollapse;
    }

    private static void Collapse(GridCell cell, Random random, bool ignorePrev) {
        if (
            cell.current != null 
            && cell.superposition.Contains(cell.current)
            && !ignorePrev
        ) {
            cell.Collapse(cell.current);
            return;
        }

        cell.Collapse(random);
    }

    private static Position ChoosePosition(Grid grid, Random random) {
        List<Position> orderlyPositions = new List<Position>();
        float lowestEntropy = float.PositiveInfinity;
        foreach (Position pos in Position.Range(grid.size_x, grid.size_z)) {
            if (grid[pos].collapsed) {
                continue;
            }
            float entropy = grid[pos].entropy;
            if (entropy < lowestEntropy) {
                orderlyPositions.Clear();
                lowestEntropy = entropy;
            }
            if (entropy == lowestEntropy) {
                orderlyPositions.Add(pos);
            }
        }
        return orderlyPositions[random.Next(orderlyPositions.Count)];
    }
}