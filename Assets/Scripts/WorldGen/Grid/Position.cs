using System;
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

    public static PositionRange Range(int size_x, int size_z) { return new Position.PositionRange(0, 0, size_x, size_z); }
    public static PositionRange Range(int minX, int minZ, int maxX, int maxZ) { return new Position.PositionRange(minX, minZ, maxX, maxZ); }
    public class PositionRange: IEnumerable<Position> {
        private readonly int minX, minZ, maxX, maxZ;
        public PositionRange(int minX, int minZ, int maxX, int maxZ) {
            if (minX > maxX) {
                throw new ArgumentException(string.Format("MinX ({0}) is greater than maxX ({1})", minX, maxX));
            }if (minZ > maxZ) {
                throw new ArgumentException(string.Format("minZ ({0}) is greater than maxZ ({1})", minZ, maxZ));
            }
            this.minX = minX;
            this.minZ = minZ;
            this.maxX = maxX;
            this.maxZ = maxZ;
        }

        public IEnumerator<Position> _GetEnumerator()
        {
            for (int x = minX; x < maxX; x++) {
                for (int z = minZ; z < maxZ; z++) {
                    yield return new Position(x, z);
                }
            }
        }

        public IEnumerator<Position> GetEnumerator() => _GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _GetEnumerator();
    }

    public override string ToString()
    {
        return "(" + this.x + ", " + this.z + ")";
    }

    public override bool Equals(object obj)
    {
        if ((obj == null) || !this.GetType().Equals(obj.GetType())) {
            return false;
        }
        Position pos = (Position) obj;
        return this.x == pos.x && this.z == pos.z;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.x, this.z);
    }
}