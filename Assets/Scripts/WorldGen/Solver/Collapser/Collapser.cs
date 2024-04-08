using System;
using System.Collections.Generic;

public class Collapser {
    public static Position Collapse(Grid grid, Random random) {
        Position toCollapse = ChoosePosition(grid, random);
        grid[toCollapse].Collapse(random);
        return toCollapse;
    }

    public static void Collapse(Grid grid, Position position, string to) {
        grid[position].Collapse(to);
    }

    static Position ChoosePosition(Grid grid, Random random) {
        var orderlyPositions = new List<Position>();
        float lowestEntropy = float.PositiveInfinity;
        foreach (var pos in grid.Positions) {
            float entropy = grid[pos].entropy;
            if (entropy > lowestEntropy || grid[pos].collapsed) continue;

            if (grid[pos].entropy < lowestEntropy) {
                orderlyPositions.Clear();
            }
            orderlyPositions.Add(pos);
        }
        return orderlyPositions[random.Next(orderlyPositions.Count)];
    }
}