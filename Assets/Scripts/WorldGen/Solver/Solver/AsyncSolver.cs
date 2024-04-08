using System.Threading.Tasks;
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class AsyncSolver : WorldGenerator
{
    static readonly int BACKUP_INTERVAL = 16;
    GridView view;
    Task<(Grid grid, bool fullyDestroyed)> solver;
    static CancellationTokenSource tokenSource = new CancellationTokenSource();
    static CancellationToken ct = tokenSource.Token;
    Position epicenter;
    IEnumerator Start()
    {
        this.solver = Task.Run<(Grid grid, bool fullyDestroyed)>(() => 
            SolveFull(this.grid, new Grid(this.grid), 0)
        );
        GridView.TransitionFinished.AddListener(StartModify);
        return WaitForSolver();
    }

    void StartModify() {
        Debug.Log("Starting Modify");
        NewEpicenter();
        this.solver = Task.Run<(Grid grid, bool fullyDestroyed)>(() => 
            Solve(this.grid, this.epicenter, iterationRadius, maxTries)
        );
        StartCoroutine(WaitForSolver());
    }

    void NewEpicenter() {
        this.epicenter = new Position(
            UnityEngine.Random.Range(0, this.grid.size_x),
            UnityEngine.Random.Range(0, this.grid.size_z)
        );
    }

    IEnumerator WaitForSolver() {
        while (!solver.IsCompleted) {
            yield return null;
        }
        var result = solver.Result;
        if (result.fullyDestroyed) {
            Done(result.grid);
        } else {
            Done(result.grid, this.epicenter, iterationRadius);
        }
    }

    void Done(Grid grid) {
        var epicenter = new Position(width / 2, length / 2);
        Done(grid, epicenter, Mathf.CeilToInt(epicenter.Distance(new Position(width, length))));
    }

    void Done(Grid grid, Position epicenter, float radius) {
        this.grid = grid;
        GridView.StartTransition(epicenter, radius);
    }

    static (Grid grid, bool fullyDestroyed) SolveFull(Grid initial, Grid nextGrid, int tries = 0, int triesAt0 = 0, int totalSolved = 0) {
        var random = new System.Random();
        if (tries == 0) {
            initial.Reset();
            nextGrid.Reset();
        }
        Grid backup = new Grid(nextGrid);
        int solved = 0;
        do {
            ct.ThrowIfCancellationRequested();
            Position collapsed = Collapser.Collapse(nextGrid, random);
            if (Propagator.Propagate(nextGrid, collapsed)) {
                Debug.Log("Retrying full: " + (tries++) + ", " + totalSolved + " (" + solved + ")");
                if (solved == 0 && triesAt0 > 8) {
                    return SolveFull(initial, new Grid(initial), tries, 0, 0);
                } else {
                    return SolveFull(initial, backup, tries, solved == 0 ? triesAt0 + 1 : 0, totalSolved);
                }
            } else if (solved++ % BACKUP_INTERVAL == 0) {
                backup = new Grid(nextGrid);
                totalSolved += BACKUP_INTERVAL;
            }
        } while (!nextGrid.IsFullyCollapsed());
        Debug.Log("Done with iteration");
        foreach(var pos in nextGrid.Positions) {
            GridView.UpdateCell(pos, nextGrid[pos].prototype);
        }
        return (nextGrid, true);

    }
    

    static (Grid grid, bool fullyDestroyed) Solve(Grid initial, Position epicenter, float radius, int tryCountdown) {
        System.Random random = new System.Random();
        List<Position> modified = new List<Position>();
        Grid nextGrid = ResetInRadius(initial, epicenter, radius);
        int solved = 0;
        do {
            ct.ThrowIfCancellationRequested();
            Position collapsed = Collapser.Collapse(nextGrid, random);
            modified.Add(collapsed);
            if (Propagator.Propagate(nextGrid, collapsed)) {
                Debug.Log("Retrying partial (" + solved + ")");
                if (tryCountdown < 0) {
                    return SolveFull(initial, new Grid(initial, true));
                } else {
                    return Solve(initial, epicenter, radius, tryCountdown - 1);
                }
            } else {
                solved++;
            }
        } while (!nextGrid.IsFullyCollapsed());

        Debug.Log("Done with iteration");

        foreach (var pos in modified) {
            GridView.UpdateCell(pos, nextGrid[pos].prototype);
        }
        return (nextGrid, false);
    }

    static Grid ResetInRadius(Grid initial, Position epicenter, float radius) {
        Grid nextGrid = new Grid(initial, true);
        
        foreach (var pos in nextGrid.Positions) {
            if (pos.Distance(epicenter) > radius && initial[pos].collapsed) {
                nextGrid[pos].Collapse(initial[pos].current);
                if (Propagator.Propagate(nextGrid, pos)) {
                    Debug.LogError("Invalid cell in reset?!");
                }
            }
        }
        return nextGrid;
    }

    static Grid ResetGrid(Grid initial) {
        return new Grid(initial, true);
    }

    void OnDestroy() {
        GridView.TransitionFinished.RemoveListener(StartModify);
        tokenSource.Cancel();
        solver.Wait();
        solver.Dispose();
    }
}