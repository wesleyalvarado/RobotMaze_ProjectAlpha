using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    public GameObject[] obstacles; // Array to hold references to the obstacle GameObjects

    void Start()
    {
        // Any initialization if needed
    }

    // Method to reset all obstacles
    public void ResetObstacles()
    {
        foreach (var obstacle in obstacles)
        {
            // Ensure each obstacle has an ObstacleMovement component and call ResetObstacle
            var obstacleMovement = obstacle.GetComponent<ObstacleMovement>();
            if (obstacleMovement != null)
            {
                obstacleMovement.ResetObstacle(); // Reset obstacle position
            }
            else
            {
                Debug.LogWarning("ObstacleMovement component not found on " + obstacle.name);
            }
        }
    }
}
