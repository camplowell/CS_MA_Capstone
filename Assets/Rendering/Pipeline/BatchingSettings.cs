using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BatchingSettings
{
    public bool useDynamicBatching = true;
    public bool useGPUInstancing = true;
    public bool useSRPBatcher = true;
}
