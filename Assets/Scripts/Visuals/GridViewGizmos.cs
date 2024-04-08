using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GridView : MonoBehaviour
{
    void OnDrawGizmos()
    {
        WorldGenerator generator = GetComponent<WorldGenerator>();
        Vector3 size = new Vector3(
            tileSize * generator.width,
            tileHeight * generator.height,
            tileSize * generator.length
        );
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = Color.red;
        foreach (var camera in Camera.allCameras) {
            DrawFrustumAt(camera, 0);
            DrawFrustumAt(camera, size.y);
        }
        Gizmos.color = Color.gray;
        
        Gizmos.DrawWireCube(
            size / 2,
            size
        );
    }

    void DrawFrustumAt(Camera camera, float height) {
        Vector3 bottomLeft = GetPointAtHeight(camera.ViewportPointToRay(new Vector3(0, 0, 0)), height);
        Vector3 bottomRight = GetPointAtHeight(camera.ViewportPointToRay(new Vector3(1, 0, 0)), height);
        Vector3 topLeft = GetPointAtHeight(camera.ViewportPointToRay(new Vector3(0, 1, 0)), height);
        Vector3 topRight = GetPointAtHeight(camera.ViewportPointToRay(new Vector3(1, 1, 0)), height);

        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(bottomLeft, topLeft);
        Gizmos.DrawLine(bottomRight, topRight);
    }

    Vector3 GetPointAtHeight(Ray ray, float height) {
        return ray.origin + (((ray.origin.y - height) / -ray.direction.y) * ray.direction);
    }
}
