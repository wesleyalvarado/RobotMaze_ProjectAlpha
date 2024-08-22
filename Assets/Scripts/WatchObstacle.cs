using UnityEngine;

public class WatchObstacle : MonoBehaviour
{
    public Transform obstacleToWatch; // The obstacle to watch
    public Vector3 offset = new Vector3(0, 5, -10); // Camera offset from the obstacle

    void LateUpdate()
    {
        if (obstacleToWatch != null)
        {
            // Update the camera's position to follow the obstacle with the specified offset
            transform.position = obstacleToWatch.position + offset;
            // Optionally, look at the obstacle
            transform.LookAt(obstacleToWatch);
        }
    }
}
