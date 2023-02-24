using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AsyncSolver : MonoBehaviour
{
    public int size_x = 10;
    public int size_z = 10;
    public Vector3 spacing = new Vector3(2.0f, 1.0f, 2.0f);
    public float modifyDelay_min = 3.0f;
    public float modifyDelay_max = 5.0f;

    private PrototypeDict prototypes;
    private Grid grid;
    private PooledGridView gridView;
    private System.Random random;

    private Task<Grid> modifyTask;
    private float countdown;

    void Start()
    {
        this.prototypes = Loader.LoadPrototypes();
        this.grid = new Grid(size_x, size_z, this.prototypes);
        this.gridView = ScriptableObject.CreateInstance<PooledGridView>();
        this.gridView.Init(size_x, size_z, this.spacing);
        this.random = new System.Random();

        this.modifyTask = Modify(0.0f);
    }

    void Update()
    {
        this.countdown -= Time.deltaTime;
        if (this.modifyTask.IsCompleted) {
            this.grid = modifyTask.Result;

            this.gridView.Refresh(this.grid);

            this.modifyTask = Modify(countdown);
        }
    }

    public async Task<Grid> Modify(float initialDelay) {
        await Task.Delay((int)(initialDelay * 1000));
        this.countdown = UnityEngine.Random.Range(modifyDelay_min, modifyDelay_max);
        Grid nextGrid = new Grid(this.grid, this.prototypes);
        Position epicenter = null;
        do {
            Position collapsed;
            if (epicenter == null) {
                collapsed = Collapser.Collapse(nextGrid, this.random, ignorePrev: true);
                epicenter = collapsed;
            } else {
                collapsed = RadialCollapser.Collapse(nextGrid, this.random, epicenter, ignorePrev: false);
            }
            await AsyncPropagator.Propagate(nextGrid, collapsed);
        } while (!nextGrid.IsFullyCollapsed());

        return nextGrid;
    }
}
