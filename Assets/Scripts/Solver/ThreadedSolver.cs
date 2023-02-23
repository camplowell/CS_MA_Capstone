using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class ThreadedSolver : MonoBehaviour
{
    public int size_x = 10;
    public int size_z = 10;
    public Vector3 spacing = new Vector3(2.0f, 1.0f, 2.0f);

    public float modifyDelay_min = 3.0f;
    public float modifyDelay_max = 5.0f;

    private PrototypeDict prototypes;
    private Grid grid;
    private GridView gridView;
    private Task<(Grid grid, Position epicenter)> solver;
    private bool wasRunning = false;
    private float modifyCountdown = 0.0f;

    void Start()
    {
        this.prototypes = LoadPrototypes();
        this.grid = new Grid(size_x, size_z, prototypes);
        this.gridView = ScriptableObject.CreateInstance<GridView>();
        this.gridView.Init(size_x, size_z, this.spacing);
        SolveAsync();
    }

    void Update()
    {
        if (this.solver.IsCompleted && this.wasRunning) {
            this.wasRunning = false;

            this.grid = this.solver.Result.grid;
            this.gridView.Refresh(this.grid);
            
            Invoke("ModifyAsync", Mathf.Max(this.modifyCountdown, 0.0f));
        }
        this.modifyCountdown -= Time.deltaTime;
    }

    public PrototypeDict LoadPrototypes() {
        TextAsset protoText = Resources.Load<TextAsset>("Tiles/TilePrototypes");
        return JsonConvert.DeserializeObject<PrototypeDict>(protoText.text);
    }

    public void SolveAsync() {
        this.modifyCountdown = UnityEngine.Random.Range(modifyDelay_min, modifyDelay_max);
        this.solver = Task.Run<(Grid grid, Position epicenter)>(() => Solve(new Grid(this.grid, this.prototypes)));
        this.wasRunning = true;
    }

    public void ModifyAsync() {
        this.modifyCountdown = UnityEngine.Random.Range(modifyDelay_min, modifyDelay_max);
        this.solver = Task.Run<(Grid grid, Position epicenter)>(() => Modify(new Grid(this.grid, this.prototypes)));
        this.wasRunning = true;
    }

    private static (Grid grid, Position epicenter) Modify(Grid grid) {
        System.Random random = new System.Random();
        Position epicenter = null;
        do {
            Position collapsed;
            if (epicenter == null) {
                collapsed = Collapser.Collapse(grid, random, ignorePrev: true);
                epicenter = collapsed;
            } else {
                collapsed = RadialCollapser.Collapse(grid, random, epicenter, ignorePrev: false);
            }
            if (epicenter == null) {
                epicenter = collapsed;
            }

            QueuePropagator.Propagate(grid, collapsed);

        } while (!grid.IsFullyCollapsed());

        return (grid, epicenter);
    }


    private static (Grid grid, Position epicenter) Solve(Grid grid) {
        System.Random random = new System.Random();
        Position epicenter = null;
        do {
            Position collapsed = Collapser.Collapse(grid, random, ignorePrev: true);
            if (epicenter == null) {
                epicenter = collapsed;
            }

            QueuePropagator.Propagate(grid, collapsed);

        } while (!grid.IsFullyCollapsed());
        return (grid, epicenter);
    }
}