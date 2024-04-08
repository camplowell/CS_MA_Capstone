public class Edge {
    public readonly (int height, Biome biome) left;
    public readonly (int height, Biome biome) right;
    public readonly EdgeFeature feature;

    public Edge((int height, Biome biome) left, EdgeFeature feature, (int height, Biome biome) right) {
        this.left = left;
        this.feature = feature;
        this.right = right;
    }
    public static bool Matches(Edge a, int a_elevation, Edge b, int b_elevation) {
        return (
            a.left.height + a_elevation == b.right.height + b_elevation
            && a.right.height + a_elevation == b.left.height + b_elevation
            && a.left.biome.name == b.right.biome.name
            && a.right.biome.name == b.left.biome.name
            && a.feature == b.feature
        );

    }
}