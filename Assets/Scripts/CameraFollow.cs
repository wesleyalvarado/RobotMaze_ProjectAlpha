using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;  // The robot's transform to follow
    public Vector3 offset;    // Offset position from the target

    void LateUpdate()
    {
        if (target != null)
        {
            // Set the camera's position to the target's position with the offset
            transform.position = target.position + offset;

            // Optionally, make the camera look at the target
            transform.LookAt(target);
        }
    }
}
