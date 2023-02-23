using System.Collections.Generic;
using UnityEngine;

public class GridView: ScriptableObject {
    private (string name, GameObject obj)?[,] tileObjects;
    private Vector3 spacing;
    private Dictionary<string, GameObject> prefabs;

    public void Init(int size_x, int size_z, Vector3 spacing, Dictionary<string, GameObject> prefabs = null) {
        this.tileObjects = new (string name, GameObject obj)?[size_x, size_z];
        this.spacing = spacing;
        if (this.prefabs == null) {
            this.prefabs = LoadPrefabs();
        } else {
            this.prefabs = prefabs;
        }
    }

    public void Refresh(Grid grid) {
        foreach (Position pos in Position.Range(grid.size_x, grid.size_z)) {
            Refresh(pos, grid);
        }
    }

    public void Refresh(int x, int z, Grid grid) {
        GridCell cell = grid[x, z];
        ref var objTuple = ref this.tileObjects[x, z];

        if (cell.current == null) {
            if (objTuple != null) {
                Destroy(objTuple.Value.obj);
                objTuple = null;
            }
            return;
        }

        Prototype proto = cell.Prototype();
        
        if (objTuple == null) {
            objTuple = (proto.model, Instantiate(this.prefabs[proto.model]));
        } else if (!objTuple.Value.name.Equals(proto.model)) {
            Destroy(objTuple.Value.obj);
            objTuple = (proto.model, Instantiate(this.prefabs[proto.model]));
        }

        Transform transform = objTuple.Value.obj.transform;
        transform.position = Vector3.Scale(new Vector3(x, proto.elevation, z), spacing);
        transform.rotation = Quaternion.Euler(0, proto.rot, 0);
    }

    public void Refresh(Position pos, Grid grid) {
        Refresh(pos.x, pos.z, grid);
    }

    public static Dictionary<string, GameObject> LoadPrefabs() {
        var prefabs = new Dictionary<string, GameObject>();
        foreach (GameObject prefab in Resources.LoadAll<GameObject>("Tiles")) {
            prefabs.Add(prefab.name, prefab);
        }
        return prefabs;
    }
}