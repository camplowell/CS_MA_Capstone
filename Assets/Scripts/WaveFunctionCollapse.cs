using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

public class WaveFunctionCollapse : MonoBehaviour
{
    public int size_x = 10;
    public int size_z = 10;

    private PrototypeCollection protos;
    private Dictionary<string, GameObject> objects;

    private Queue<(int x, int z)> toProcess = new Queue<(int x, int z)>();

    private bool done;

    private Cell[,] grid;
    // Start is called before the first frame update
    void Start()
    {
        GatherResources();
        InitializeGrid();
        Debug.Log(this.protos);
        Debug.Log(this.objects);
        Debug.Log(this.grid);
        done = false;
    }

    // Update is called once per frame
    void Update()
    {
        var collapsed = CollapseOne();
        if (done) {
            return;
        }
        Cell collapsedCell = grid[collapsed.x, collapsed.z];
        TilePrototype toInstantiate = protos[collapsedCell.current];
        GameObject instantiated = Instantiate(objects[toInstantiate.objName]);
        instantiated.transform.position = new Vector3(2 * collapsed.x, 0, 2 * collapsed.z);
        instantiated.transform.Rotate(new Vector3(0, toInstantiate.rotation, 0));
        toProcess.Enqueue(collapsed);
        while (toProcess.Count > 0) {
            PropagateIteration(toProcess);
        } 
    }

    void InitializeGrid() {
        this.grid = new Cell[size_x, size_z];
        for (int x = 0; x < size_x; x++) {
            for (int z = 0; z < size_z; z++) {
                this.grid[x,z] = new Cell(protos);
            }
        }
    }

    void GatherResources() {

        TextAsset protoText = Resources.Load("Tiles/TileInfo") as TextAsset;
        this.protos = JsonConvert.DeserializeObject<PrototypeCollection>(protoText.text);

        GameObject[] tileObjects = Resources.LoadAll<GameObject>("Tiles/");
        this.objects = new Dictionary<string, GameObject>();
        foreach (GameObject tile in tileObjects) {
            this.objects.Add(tile.name, tile);
        }
    }

    List<Direction> ValidDirections((int x, int z) pos) {
        List<Direction> valid = new List<Direction>();
        foreach (Direction dir in Direction.Values) {
            var there = (x: pos.x + dir.x, z: pos.z + dir.z);
            if (there.x >= 0 && there.x < size_x
                && there.z >= 0 && there.z < size_z
            ) {
                valid.Add(dir);
            }
        }
        return valid;
    }

    void PropagateIteration(Queue<(int x, int z)> toProcess) {
        (int x, int z) here = toProcess.Dequeue();
        Cell cell = grid[here.x, here.z];

        foreach (Direction dir in ValidDirections(here)) {
            (int x, int z) there = (here.x + dir.x, here.z + dir.z);
            Cell neighbor = grid[there.x, there.z];
            if (neighbor.isResolved()) {
                continue;
            }
            bool trimmed = false;
            if (dir == Direction.N) {
                trimmed = neighbor.Intersect(cell.N);
            } else if (dir == Direction.E) {
                trimmed = neighbor.Intersect(cell.E);
            } else if (dir == Direction.S) {
                trimmed = neighbor.Intersect(cell.S);
            } else if (dir == Direction.W) {
                trimmed = neighbor.Intersect(cell.W);
            }
            if (trimmed) {
                toProcess.Enqueue(there);
            }
        }
    }

    (int x, int z) CollapseOne() {
        if (done) {
            return (-1, -1);
        }
        List<(int x, int z)> orderlyCells = new List<(int x, int z)>();
        float lowestEntropy = float.PositiveInfinity;
        for (int z = 0; z < size_z; z++) {
            for (int x = 0; x < size_x; x++) {
                Cell cell = this.grid[x, z];
                if (cell.isResolved() || cell.entropy > lowestEntropy) {
                    continue;
                }
                if (cell.entropy < lowestEntropy) {
                    lowestEntropy = cell.entropy;
                    orderlyCells.Clear();
                }
                orderlyCells.Add((x, z));
            }
        }
        if (orderlyCells.Count == 0) {
            done = true;
            return (-1, -1);
        }
        var toCollapse = orderlyCells[(int)UnityEngine.Random.Range(0.0f, orderlyCells.Count)];
        grid[toCollapse.x, toCollapse.z].Collapse();
        return toCollapse;
    }
}

public sealed class Direction: IComparable<Direction> {
    public readonly int x;
    public readonly int z;

    private Direction(int x, int z) {
        this.x = x;
        this.z = z;
    }
    
    public static readonly Direction N = new Direction(0, 1);
    public static readonly Direction E = new Direction(1, 0);
    public static readonly Direction S = new Direction(0, -1);
    public static readonly Direction W = new Direction(-1, 0);

    public static readonly Direction[] Values = {N, E, S, W};

    public int CompareTo(Direction other)
    {
        throw new NotImplementedException();
    }
}
