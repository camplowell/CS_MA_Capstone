using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public partial class GridView : MonoBehaviour
{
    public static UnityEvent TransitionFinished = new UnityEvent();
    [ColorUsage(false, true)] public Color glow = Color.cyan;
    public float transitionSpeed = 5;
    public float transitionFade = 5;
    public float tileSize = 10;
    public float tileHeight = 5;

    private static GridView instance;
    private ConcurrentQueue<(Position pos, Prototype prototype)> addQueue =
        new ConcurrentQueue<(Position pos, Prototype prototype)>();
    private Queue<GameObject> stale = new Queue<GameObject>();
    private Dictionary<Position, TransitionVisibility> stable = new Dictionary<Position, TransitionVisibility>();
    private Dictionary<Position, TransitionVisibility> fresh = new Dictionary<Position, TransitionVisibility>();
    private Dictionary<string, GameObject> prefabs;


    private bool toStartTransition;
    private Position transitionPos;
    private float transitionRadius;
    private bool isNew;

    public static void UpdateCell(Position pos, Prototype prototype) {
        instance.addQueue.Enqueue((pos, prototype));
    }
    public static void StartTransition(Position pos, float radius) {
        instance.transitionPos = pos;
        instance.transitionRadius = radius;
        instance.toStartTransition = true;
    }

    void Awake()
    {
        if (instance == null) {
            instance = this;
        } else {
            Debug.LogWarning("Muliple GridViews active in scene: " + gameObject.name);
            this.enabled = false;
        }
        LoadPrefabs();
        isNew = true;
    }

    void Update()
    {
        AddFresh();
        CollectStale();
    }

    IEnumerator Transition(Position epicenter, float radius) {
        Debug.Log("Starting Transition");
        toStartTransition = false;
        radius += Mathf.Sqrt(2.0f);
        radius *= tileSize;
        float currentRadius = 0;
        float timeSinceStart = 0;
        float timeSinceTransitionEnd = 0;
        while (timeSinceTransitionEnd < transitionFade) {
            timeSinceStart += Time.deltaTime;
            currentRadius = timeSinceStart * timeSinceStart * timeSinceStart * transitionSpeed;
            if (currentRadius > radius) { timeSinceTransitionEnd += Time.deltaTime; }
            float fade = 1 - Mathf.Clamp01(timeSinceTransitionEnd / transitionFade);
            UpdateTransition(
                epicenter,
                currentRadius,
                fade
            );
            yield return null;
        }
        FinishTransition();
    }

    void UpdateTransition(Position epicenter, float radius, float strength) {
        var transitionSettings = CapstonePipeline.activeInstance.environment.transition;
        transitionSettings.position = new Vector2(epicenter.x, epicenter.z) * tileSize;
        transitionSettings.glow = glow * strength;
        transitionSettings.radius = radius;
        ColumnPassRenderer.distortionMultiplier = strength;
    }


    void FinishTransition() {
        Debug.Log("Finished transition");
        foreach (Position pos in fresh.Keys) {
            // Mark stable tile stale
            if (stable.ContainsKey(pos)) {
                stable[pos].gameObject.SetActive(false);
                stale.Enqueue(stable[pos].gameObject);
            }
            // Mark fresh tile stable
            fresh[pos].SetTransitionVisibility(true, true);
            stable[pos] = fresh[pos];
        }
        fresh.Clear();
        UpdateTransition(new Position(0, 0), 0, 0);
        TransitionFinished.Invoke();
    }

    void AddFresh() {
        if (addQueue.IsEmpty) {
            if (toStartTransition) StartCoroutine(Transition(transitionPos, transitionRadius));
            return;
        }
        if (isNew) {
            UpdateTransition(new Position(0, 0), 0, 0);
        }
        addQueue.TryDequeue(out (Position pos, Prototype prototype) dequeued);
        Position pos = dequeued.pos;

        if (stable.ContainsKey(pos)) {
            stable[pos].SetTransitionVisibility(false, true);
        }

        Prototype prototype = dequeued.prototype;

        Vector3 positionWS = new Vector3(
            (pos.x + 0.5f) * tileSize,
            prototype.elevation * tileHeight,
            (pos.z + 0.5f) * tileSize
        );

        Quaternion rotation = Quaternion.Euler(0, -90 * prototype.rot, 0);

        GameObject instanced = Instantiate(prefabs[prototype.prefab], positionWS, rotation);
        TransitionVisibility visibility = instanced.GetComponentInChildren<TransitionVisibility>();
        fresh.Add(pos, visibility);
    }

    void CollectStale() {
        if (stale.Count == 0) { return; }
        Destroy(stale.Dequeue());
    }


    void OnDestroy()
    {
        if (instance == this) instance = null;
    }


    void LoadPrefabs() {
        this.prefabs = new Dictionary<string, GameObject>();
        foreach (GameObject prefab in Resources.LoadAll<GameObject>("Tiles")) {
            this.prefabs.Add(prefab.name, prefab);
        }
    }
}