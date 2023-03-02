using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileParams : MonoBehaviour
{
    [Header("Global")]

    public float weight = 1.0f;

    [Header("Corner Properties")]

    public CornerParams nn;

    public CornerParams np;

    public CornerParams pn;

    public CornerParams pp;

    [Header("Edge Properties")]

    public string pZ = "";
    public string nX = "";
    public string nZ = "";
    public string pX = "";

    public CornerParams GetCorner(Corner corner) {
        switch(corner) {
            case Corner.nn:
                return this.nn;
            case Corner.np:
                return this.np;
            case Corner.pp:
                return this.pp;
            default:
                return this.pn;
        }
    }

    public string GetBorder(Border border) {
        switch(border) {
            case Border.pZ:
                return this.pZ;
            case Border.nX:
                return this.nX;
            case Border.nZ:
                return this.nZ;
            default:
                return this.pX;
        }
    }

    public int getMaxHeight() {
        return Mathf.Max(Mathf.Max(this.nn.height, this.np.height), Mathf.Max(this.pn.height, this.pp.height));
    }

    public int getMinHeight() {
        return Mathf.Min(Mathf.Min(this.nn.height, this.np.height), Mathf.Min(this.pn.height, this.pp.height));
    }
    
}

[System.Serializable]
public class CornerParams {
    public Biome biome;
    public int height;

    public CornerParams(Biome biome, int height) {
        this.biome = biome;
        this.height = height;
    }

    public bool Equals(CornerParams other) {
        if (other == null) {
            return false;
        }
        return this.biome == other.biome && this.height == other.height;
    }
}