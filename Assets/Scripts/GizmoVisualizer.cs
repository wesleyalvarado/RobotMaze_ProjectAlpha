using UnityEngine;

public class GizmoVisualizer : MonoBehaviour
{
    public Vector3 gizmoSize = new Vector3(1f, 1f, 1f); // Size of the gizmo to draw

    void OnDrawGizmos()
    {
        // Set the Gizmo color
        Gizmos.color = Color.green;

        // Draw a wire cube at the object's position
        Gizmos.DrawWireCube(transform.position, gizmoSize);
    }
}
