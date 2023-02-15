using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjacencyRules : MonoBehaviour
{
    public string zSocket = "s_bottom";
    public string xSocket = "s_bottom";
    public string nzSocket = "s_bottom";
    public string nxSocket = "s_bottom";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string getPX(int rotation) {
        switch(rotation) {
            case 0: return xSocket;
            case 90: return zSocket;
            case 180: return nxSocket;
            case 270: return nzSocket;
            default: return "-1";
        }
    }

    public string getPZ(int rotation) {
        switch(rotation) {
            case 0: return zSocket;
            case 90: return nxSocket;
            case 180: return nzSocket;
            case 270: return xSocket;
            default: return "-1";
        }
    }

    public string getNX(int rotation) {
        switch(rotation) {
            case 0: return nxSocket;
            case 90: return nzSocket;
            case 180: return xSocket;
            case 270: return zSocket;
            default: return "-1";
        }
    }

    public string getNZ(int rotation) {
        switch(rotation) {
            case 0: return nzSocket;
            case 90: return xSocket;
            case 180: return zSocket;
            case 270: return nxSocket;
            default: return "-1";
        }
    }
}
