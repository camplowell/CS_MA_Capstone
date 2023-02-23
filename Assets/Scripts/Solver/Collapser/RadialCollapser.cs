using System;

public class RadialCollapser {
    public static Position Collapse(Grid grid, Random random, Position origin, bool ignorePrev = true) {
        Position toCollapse = ChoosePosition(grid, origin);
        Collapse(grid[toCollapse], random, ignorePrev);
        return toCollapse;
    }

    private static void Collapse(GridCell cell, Random random, bool ignorePrev) {
        if (cell.current != null && cell.superposition.Contains(cell.current) && !ignorePrev) {
            cell.Collapse(cell.current);
        } else {
            cell.Collapse(random);
        }
    }

    private static Position ChoosePosition(Grid grid, Position origin) {
        Position bestPosition = null;
        float bestDistance = float.PositiveInfinity;
        float lowestEntropy = float.PositiveInfinity;
        foreach (Position pos in Position.Range(grid.size_x, grid.size_z)) {
            if (grid[pos].collapsed) {
                continue;
            }
            float entropy = grid[pos].entropy;
            if (entropy < lowestEntropy) {
                bestDistance = float.PositiveInfinity;
                lowestEntropy = entropy;
            }

            float distance = pos.Distance(origin);
            if (entropy == lowestEntropy && distance < bestDistance) {
                bestPosition = pos;
                bestDistance = distance;
            }
        }

        return bestPosition;
    }
}