using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrototypeBuilder
{
    public string name;
    public GameObject toInstance;
    public int rotation;
    public List<string> neighbor_pX;
    public List<string> neighbor_nX;
    public List<string> neighbor_pZ;
    public List<string> neighbor_nZ;

    public string socket_pX;
    public string socket_nX;
    public string socket_pZ;
    public string socket_nZ;

    public PrototypeBuilder(GameObject toInstance, int rotation) {
        this.name = toInstance.name + "_" + rotation;
        this.toInstance = toInstance;
        this.rotation = rotation;

        this.neighbor_pX = new List<string>();
        this.neighbor_nX = new List<string>();
        this.neighbor_pZ = new List<string>();
        this.neighbor_nZ = new List<string>();

        AdjacencyRules rules = toInstance.GetComponent<AdjacencyRules>();

        this.socket_pX = rules.getPX(rotation);
        this.socket_pZ = rules.getPZ(rotation);
        this.socket_nX = rules.getNX(rotation);
        this.socket_nZ = rules.getNZ(rotation);
    }

    public void AddNeighbors(Dictionary<string, PrototypeBuilder> protos) {
        foreach (KeyValuePair<string, PrototypeBuilder> entry in protos) {
            string key = entry.Key;
            PrototypeBuilder other = entry.Value;
            PrototypeBuilder val = entry.Value;
            if (CanConnect(this.socket_pZ, other.socket_nZ)) {
                this.neighbor_pZ.Add(key);
            }
            if (CanConnect(this.socket_pX, other.socket_nX)) {
                this.neighbor_pX.Add(key);
            }
            if (CanConnect(this.socket_nZ, other.socket_pZ)) {
                this.neighbor_nZ.Add(key);
            }
            if (CanConnect(this.socket_nX, other.socket_pX)) {
                this.neighbor_nX.Add(key);
            }
        }
    }

    public static bool CanConnect(string socketA, string socketB) {
        string suffixA = socketA.Substring(2);
        string suffixB = socketB.Substring(2);
        if (!suffixA.Equals(suffixB)) {
            return false;
        }

        string prefixA = socketA.Substring(0, 2);
        string prefixB = socketB.Substring(0, 2);
        if (prefixA.Equals("s_")) {
            return prefixB.Equals("s_");
        }
        if (prefixA.Equals("a_")) {
            return prefixB.Equals("b_");
        }
        if (prefixA.Equals("b_")) {
            return prefixB.Equals("a_");
        }
        return false;
    }

    public TilePrototype build() {
        TilePrototype proto = new TilePrototype();
        proto.protoID = this.name;
        proto.objName = this.toInstance.name;
        proto.rotation = this.rotation;

        proto.neighbor_pZ.UnionWith(this.neighbor_pZ);
        proto.neighbor_pX.UnionWith(this.neighbor_pX);
        proto.neighbor_nZ.UnionWith(this.neighbor_nZ);
        proto.neighbor_nX.UnionWith(this.neighbor_nX);

        return proto;
    }
}