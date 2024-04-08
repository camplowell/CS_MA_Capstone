using Newtonsoft.Json;
using UnityEngine;

[RequireComponent(typeof(GridView))]
public partial class WorldGenerator : MonoBehaviour
{
    public static WorldGenerator instance;
    public int maxTries = 16;
    public float iterationRadius = 5;

    public int width = 10;
    public int length = 10;
    public int height = 2;
    public string bakePath = "Tiles/TilePrototypes";

    protected Grid grid;

    [HideInInspector] public float tallestTile = 5;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null) {
            instance = this;
        } else {
            Debug.LogWarning("Multiple WorldGenerators in scene: " + gameObject.name);
        }

        var prototypes = LoadPrototypes();
        this.grid = new Grid(width, length, prototypes);
    }

    PrototypeDict LoadPrototypes() {
        TextAsset protoText = Resources.Load<TextAsset>(bakePath);
        return JsonConvert.DeserializeObject<PrototypeDict>(protoText.text);
    }

    void OnDestroy()
    {
        if (instance == this) instance = null;
    }
}
