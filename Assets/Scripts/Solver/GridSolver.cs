using Newtonsoft.Json;
using UnityEngine;

public class GridSolver : MonoBehaviour
{
    public int size_x = 10;
    public int size_z = 10;
    public Vector3 spacing = new Vector3(2.0f, 1.0f, 2.0f);

    private Grid grid;
    private GridView gridView;
    private System.Random random;
    private bool done;

    void Start()
    {
        PrototypeDict prototypes = Loader.LoadPrototypes();
        this.grid = new Grid(size_x, size_z, prototypes);
        this.gridView = ScriptableObject.CreateInstance<GridView>();
        this.gridView.Init(size_x, size_z, this.spacing);
        this.random = new System.Random();
        this.done = false;
    }

    void Update()
    {
        if (this.done) {
            return;
        }
        Position collapsed = Collapser.Collapse(this.grid, random);
        QueuePropagator.Propagate(this.grid, collapsed);
        this.gridView.Refresh(this.grid);
        this.done = this.grid.IsFullyCollapsed();
        if (this.done) {
            Debug.Log("Done");
        }
    }
}