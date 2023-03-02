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
    private bool wasRunning;
    const int MAX_RETRIES = 4;

    void Start()
    {
        this.prototypes = Loader.LoadPrototypes();
        this.grid = new Grid(size_x, size_z, this.prototypes);
        this.gridView = ScriptableObject.CreateInstance<PooledGridView>();
        this.gridView.Init(size_x, size_z, this.spacing);
        this.random = new System.Random();

        _Modify();
    }

    void Update()
    {
        this.countdown -= Time.deltaTime;
        if (this.modifyTask.IsCompleted && this.wasRunning) {
            this.grid = modifyTask.Result;

            this.gridView.Refresh(this.grid);
            Invoke("_Modify", countdown);
        }
    }

    private void _Modify() {

        this.countdown = UnityEngine.Random.Range(modifyDelay_min, modifyDelay_max);
        this.modifyTask = Modify();
    }

    public async Task<Grid> Modify() {
        this.wasRunning = true;
        Grid nextGrid = new Grid(this.grid, this.prototypes);
        Position epicenter = null;
        int iterCount = 0;
        int retryCount = 0;
        do {
            Position collapsed;
            if (epicenter == null) {
                collapsed = Collapser.Collapse(nextGrid, this.random, ignorePrev: true);
                epicenter = collapsed;
            } else {
                collapsed = RadialCollapser.Collapse(nextGrid, this.random, epicenter, ignorePrev: false);
            }
            if (!await AsyncPropagator.Propagate(nextGrid, collapsed)) {
                retryCount++;
                if (retryCount > MAX_RETRIES) {
                    grid.Reset(clearCurrent: true);
                } else {
                    grid.Reset(this.grid);
                }
            }
            iterCount++;
        } while (!nextGrid.IsFullyCollapsed());
        Debug.Log("Finished with " + iterCount + " iterations and " + retryCount + " retries");
        return nextGrid; 
    }
}
