using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LagMeter : MonoBehaviour
{
    public Text msCounter;
    public int frameHistory = 100;
    private Queue<float> msHistory;
    // Start is called before the first frame update
    void Start()
    {
        msHistory = new Queue<float>();
    }

    // Update is called once per frame
    void Update()
    {
        msHistory.Enqueue(Time.deltaTime * 1000);
        if (msHistory.Count > frameHistory) {
            msHistory.Dequeue();
        }
        float maxMs = 0.0f;
        float avgMs = 0.0f;
        foreach (float frame in msHistory) {
            avgMs += frame;
            maxMs = Mathf.Max(maxMs, frame);
        }
        avgMs /= msHistory.Count;
        msCounter.text = "Max: " + maxMs.ToString("0.0") + "\nAverage: " + avgMs.ToString("0.0");
    }
}
