using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PooledGridView : ScriptableObject
{
    public static readonly int initial_instances = 10;
    private Vector3 spacing;
    private Dictionary<string, GameObject> prefabs;

    private (string model, GameObject obj)?[,] tileObjects;

    private Dictionary<string, LinkedPool<GameObject>> pools;

    public void Init(int size_x, int size_z, Vector3 spacing, Dictionary<string, GameObject> prefabs = null) {
        this.tileObjects = new (string model, GameObject obj)?[size_x, size_z];
        this.spacing = spacing;
        if (this.prefabs == null) {
            this.prefabs = Loader.LoadPrefabs();
        } else {
            this.prefabs = prefabs;
        }
        this.pools = new Dictionary<string, LinkedPool<GameObject>>();
        foreach (string prefabName in this.prefabs.Keys) {
            this.pools.Add(prefabName, new LinkedPool<GameObject>(
                createFunc: () => {
                    return Instantiate<GameObject>(this.prefabs[prefabName]);
                },
                actionOnGet: shape => {
                    shape.gameObject.SetActive(true);
                },
                actionOnRelease: shape => {
                    shape.gameObject.SetActive(false);
                },
                actionOnDestroy: shape => {
                    Destroy(shape.gameObject);
                },
                collectionCheck: false,
                maxSize: size_x * size_z
                ));
            
            for (int i = 0; i < initial_instances; i++) {
                GameObject obj = this.pools[prefabName].Get();
                this.pools[prefabName].Release(obj);
            }
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
                Release(objTuple.Value);
                objTuple = null;
            }
            return;
        }

        Prototype proto = cell.Prototype();
        
        if (objTuple == null) {
            objTuple = Get(proto.model);
        } else if (!objTuple.Value.model.Equals(proto.model)) {
            Release(objTuple.Value);
            objTuple = Get(proto.model);
        }

        Transform transform = objTuple.Value.obj.transform;
        transform.position = Vector3.Scale(new Vector3(x, proto.elevation, z), spacing);
        transform.rotation = Quaternion.Euler(0, proto.rot, 0);
    }

    public void Refresh(Position pos, Grid grid) {
        Refresh(pos.x, pos.z, grid);
    }

    private (string model, GameObject obj) Get(string model) {
        return (model, this.pools[model].Get());
    }

    private void Release((string model, GameObject obj) tuple) {
        this.pools[tuple.model].Release(tuple.obj);
    }
}
