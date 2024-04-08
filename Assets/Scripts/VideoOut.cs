using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class VideoOut : MonoBehaviour
{
    [SerializeField] string outFolder = "Renders/";
    [SerializeField] int frameRate = 24;
    Texture2D tex;
    Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponent<Camera>();
        tex = new Texture2D(camera.targetTexture.width, camera.targetTexture.height, TextureFormat.RGB24, false);
        Time.captureFramerate = frameRate;
    }

    // Update is called once per frame
    void Update()
    {
        tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        tex.Apply();
        System.IO.File.WriteAllBytes(outFolder + FrameString() + ".png", tex.EncodeToPNG());
    }

    string FrameString() {
        return Time.frameCount.ToString("D4");
    }
}
