using System.Collections.Generic;
using UnityEngine;

public class Grid {
    private GridCell[,] cells;

    public readonly int size_x;
    public readonly int size_z;

    public Grid(int size_x, int size_z, PrototypeDict prototypes) {
        this.size_x = size_x;
        this.size_z = size_z;
        this.cells = new GridCell[size_x, size_z];
        for (int x = 0; x < size_x; x++) {
            for (int z = 0; z < size_z; z++) {
                this.cells[x, z] = new GridCell(prototypes);
            }
        }
    }

    public void Reset(bool clearCurrent = true) {
        foreach (Position pos in Position.Range(size_x, size_z)) {
            this[pos].Reset(clearCurrent);
        }
    }

    public List<Direction> GetValidDirections(Position pos) {
        List<Direction> validDirs = new List<Direction>();
        foreach (Direction dir in Direction.Values) {
            Position there = pos + dir;
            if (InBounds(there)) {
                validDirs.Add(dir);
            }
        }
        return validDirs;
    }

    public bool InBounds(Position pos) {
        return pos.x >= 0 &&
            pos.x < cells.GetLength(0) &&
            pos.z >= 0 &&
            pos.z < cells.GetLength(1);
    }

    public bool IsFullyCollapsed() {
        for (int x = 0; x < size_x; x++) {
            for (int z = 0; z < size_z; z++) {
                if (!this.cells[x, z].collapsed) {
                    return false;
                }
            }
        }
        return true;
    }

    public GridCell this[Position pos] {
        get => cells[pos.x, pos.z];
        set { cells[pos.x, pos.z] = value; }
    }

    public GridCell this[int x, int z] {
        get => cells[x, z];
        set { cells[x, z] = value; }
    }
}