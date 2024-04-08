using System.Collections;
using System.Collections.Generic;

public class Grid
{
    public Dictionary<Position, Cell> cells;
    public IEnumerable<Position> Positions => cells.Keys;

    public readonly int size_x;
    public readonly int size_z;

    public Grid(int size_x, int size_z, PrototypeDict prototypes) {
        this.size_x = size_x;
        this.size_z = size_z;
        this.cells = new Dictionary<Position, Cell>();
        foreach (Position pos in Position.Range(size_x, size_z)) {
            this[pos] = new Cell(prototypes);
        }
    }

    public Grid(Grid copy, bool reset = false) {
        this.size_x = copy.size_x;
        this.size_z = copy.size_z;
        this.cells = new Dictionary<Position, Cell>();
        foreach (var pos in Position.Range(size_x, size_z)) {
            if (reset) {
                this[pos] = new Cell(copy[pos].prototypes);
            } else {
                this[pos] = new Cell(copy[pos]);
            }
        }
    }

    public void Reset() {
        foreach (var pos in Position.Range(size_x, size_z)) {
            this[pos].Reset();
        }
    }

    public bool IsFullyCollapsed() {
        foreach (var cell in cells.Values) {
            if (!cell.collapsed) { return false; }
        }
        return true;
    }

    public IEnumerable<Direction> ValidDirections(Position pos) => new ValidDirectionEnumerator(this, pos);

    public bool InBounds(Position pos) => cells.ContainsKey(pos);

    public Cell this[Position pos] {
        get => cells[pos];
        set => cells[pos] = value;
    }

    class ValidDirectionEnumerator: IEnumerable<Direction> {
        Grid grid;
        Position pos;

        public ValidDirectionEnumerator(Grid grid, Position pos) {
            this.grid = grid;
            this.pos = pos;
        }

        IEnumerator<Direction> _GetEnumerator() {
            foreach (var dir in Direction.Values) {
                if (grid.InBounds(pos + dir)) {
                    yield return dir;
                }
            }
        }

        public IEnumerator<Direction> GetEnumerator() => _GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _GetEnumerator();
    }
}