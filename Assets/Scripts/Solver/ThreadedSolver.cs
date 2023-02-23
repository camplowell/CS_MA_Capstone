using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class ThreadedSolver : MonoBehaviour
{
    public int size_x = 10;
    public int size_z = 10;
    public Vector3 spacing = new Vector3(2.0f, 1.0f, 2.0f);

    private PrototypeDict prototypes;
    private Grid grid;
    private GridView gridView;
    private Task<Grid> solver;

    void Start()
    {
        this.prototypes = LoadPrototypes();
        this.grid = new Grid(size_x, size_z, prototypes);
        this.gridView = ScriptableObject.CreateInstance<GridView>();
        this.gridView.Init(size_x, size_z, this.spacing);
        this.solver = SolveAsync();
    }

    void Update()
    {
        if (this.solver.IsCompleted) {
            this.grid = this.solver.Result;
            this.gridView.Refresh(this.grid);
        }
    }

    private PrototypeDict LoadPrototypes() {
        TextAsset protoText = Resources.Load<TextAsset>("Tiles/TilePrototypes");
        return JsonConvert.DeserializeObject<PrototypeDict>(protoText.text);
    }

    private Task<Grid> SolveAsync() {
        return Task.Run<Grid>(() => Solve(new Grid(this.grid, this.prototypes)));
    }


    private static Grid Solve(Grid grid) {
        bool ignorePrev = true;
        System.Random random = new System.Random();
        do {
            Position collapsed = Collapser.Collapse(grid, random, ignorePrev);
            ignorePrev = false;
            QueuePropagator.Propagate(grid, collapsed);
        } while (!grid.IsFullyCollapsed());
        return grid;
    }
}