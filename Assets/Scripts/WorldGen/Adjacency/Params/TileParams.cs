using UnityEngine;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class TileParams : MonoBehaviour
{
    [SerializeField] float _weight = 1;
    [SerializeField] TileShape shape;
    [SerializeField] Biomes biomes;
    [SerializeField] EdgeFeatures edgeFeatures;

    public Dictionary<Direction, Edge> edges;
    public int MaxHeight { get {
        return Mathf.Max(new int[] {
            shape.northEast, 
            shape.northWest,
            shape.southEast,
            shape.southWest
        });
    }}
    public float Weight { get {
        return _weight * 0.25f * (
            biomes.northWest.weight 
            + biomes.northEast.weight 
            + biomes.southWest.weight 
            + biomes.southEast.weight
        );
    }}

    [System.Serializable]
    public struct Biomes
    {
        public Biome northWest;
        public Biome northEast;
        public Biome southWest;
        public Biome southEast;
    }

    [System.Serializable]
    public struct EdgeFeatures
    {
        public EdgeFeature north;
        public EdgeFeature south;
        public EdgeFeature east;
        public EdgeFeature west;
    }

    void OnValidate()
    {
        edges = new Dictionary<Direction, Edge>();
        edges[Direction.north] = new Edge(
            (shape.northWest, biomes.northWest), 
            edgeFeatures.north, 
            (shape.northEast, biomes.northEast)
        );
        edges[Direction.south] = new Edge(
            (shape.southEast, biomes.southEast),
            edgeFeatures.south,
            (shape.southWest, biomes.southWest)
        );
        edges[Direction.east] = new Edge(
            (shape.northEast, biomes.northEast),
            edgeFeatures.east,
            (shape.southEast, biomes.southEast)
        );
        edges[Direction.west] = new Edge(
            (shape.southWest, biomes.southWest),
            edgeFeatures.west,
            (shape.northWest, biomes.northWest)
        );
    }

    string North() {
        string edgeFeature = edgeFeatures.north == null ? "" : edgeFeatures.north.name + "_";
        return biomes.northWest.name + shape.northWest + "_" + edgeFeature + biomes.northEast.name + shape.northEast;
    }

    string South() {
        string edgeFeature = edgeFeatures.south == null ? "" : edgeFeatures.south.name + "_";
        return biomes.southEast.name + shape.northEast + "_" + edgeFeature + biomes.northWest.name + shape.northWest;
    }

    string East() {
        string edgeFeature = edgeFeatures.east == null ? "" : edgeFeatures.east.name + "_";
        return biomes.northEast.name + shape.northEast + "_" + edgeFeature + biomes.southEast.name + shape.southEast;
    }
    string West() {
        string edgeFeature = edgeFeatures.west == null ? "" : edgeFeatures.west.name + "_";
        return biomes.southWest.name + shape.southWest + "_" + edgeFeature + biomes.northWest.name + shape.northWest;
    }
}
