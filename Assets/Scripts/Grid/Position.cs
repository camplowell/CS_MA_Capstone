using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position {
    public readonly int x;
    public readonly int z;

    public Position(int x, int z) {
        this.x = x;
        this.z = z;
    }
    public static Position operator +(Position a, Direction b)
        => new Position(a.x + b.x, a.z + b.z);

    public static Position operator +(Direction a, Position b)
        => b + a;

    public float Distance(Position other) {
        float dx = this.x - other.x;
        float dz = this.z - other.z;
        return Mathf.Sqrt(dx * dx + dz * dz);
    }

    public static PositionRange Range(int size_x, int size_z) { return new Position.PositionRange(size_x, size_z); }
    public class PositionRange: IEnumerable<Position> {
        private readonly int size_x;
        private readonly int size_z;
        public PositionRange(int size_x, int size_z) {
            this.size_x = size_x;
            this.size_z = size_z;
        }

        public IEnumerator<Position> GetEnumerator()
        {
            for (int x = 0; x < size_x; x++) {
                for (int z = 0; z < size_z; z++) {
                    yield return new Position(x, z);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int x = 0; x < size_x; x++) {
                for (int z = 0; z < size_z; z++) {
                    yield return new Position(x, z);
                }
            }
        }
    }

    public override string ToString()
    {
        return "(" + this.x + ", " + this.z + ")";
    }
}