using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Collapser {
    public static Position Collapse(Grid grid) {
        Position toCollapse = ChoosePosition(grid);
        grid[toCollapse].Collapse();
        return toCollapse;
    }

    private static Position ChoosePosition(Grid grid) {
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
        return orderlyPositions[Random.Range(0, orderlyPositions.Count)];
    }
}