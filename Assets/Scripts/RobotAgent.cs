using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RobotAgent : Agent
{
    public float moveSpeed = 10f;
    public Transform startPosition;  // Assign in Unity Editor for agent's start position
    public Transform exitPoint;      // Assign in Unity Editor for target position

    private float lastSignificantMoveTime;
    private float significantMoveThreshold = 1.0f; // Time in seconds to consider a move significant

    private Rigidbody rb;
    private float previousDistanceToTarget;
    private Vector3 previousDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.useGravity = true;

        // Ensure the target and startPosition are assigned
        if (exitPoint == null) Debug.LogError("Exit point (target) is not assigned!");
        if (startPosition == null) Debug.LogError("Start position is not assigned!");
    }

    public override void OnEpisodeBegin()
    {
        // Reset the agent's velocity and position
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (startPosition != null)
        {
            transform.position = startPosition.position;
        }

        if (exitPoint != null)
        {
            previousDistanceToTarget = Vector3.Distance(transform.position, exitPoint.position);
            previousDirection = transform.forward; // Initialize with the initial direction
            lastSignificantMoveTime = Time.time;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Collect observations
        sensor.AddObservation(transform.position);
        sensor.AddObservation(transform.forward);
        sensor.AddObservation(rb.velocity);
        sensor.AddObservation(exitPoint.position);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Get continuous actions from Unity ML-Agents
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        // Debug logging to check action values
        Debug.Log($"Action Received - X: {moveX}, Z: {moveZ}");

        // Create a movement vector and apply force
        Vector3 direction = new Vector3(moveX, 0, moveZ).normalized;
        Vector3 movement = direction * moveSpeed;
        rb.AddForce(movement, ForceMode.VelocityChange);
        
        // Debug logging to check movement values
        Debug.Log($"Direction: {direction}, Movement: {movement}");

        // Calculate and log rewards
        float distanceToTarget = Vector3.Distance(transform.position, exitPoint.position);

        // Log distance for debugging
        Debug.Log($"Distance to Target: {distanceToTarget}");

        // Proportional reward based on progress
        float distanceDelta = previousDistanceToTarget - distanceToTarget;
        if (distanceDelta > 0)
        {
            float reward = Mathf.Clamp(distanceDelta / previousDistanceToTarget, 0.01f, 0.1f);
            AddReward(reward); // Reward for moving closer to the target
        }
        else
        {
            AddReward(-0.01f); // Penalty for moving away or minimal movement
        }

        // Penalize for colliding with walls
        if (rb.velocity.magnitude < 0.01f) // Check if the agent is stationary
        {
            AddReward(-0.01f); // Penalty for getting stuck
        }

        // Penalize for stalling
        if (Time.time - lastSignificantMoveTime > significantMoveThreshold)
        {
            AddReward(-0.05f); // Penalty for not making significant progress
        }

        // Encourage exploration by rewarding directional changes
        Vector3 currentDirection = transform.forward;
        if (Vector3.Dot(currentDirection, previousDirection) < 0.9f) // Reward for significant direction change
        {
            AddReward(0.01f); // Small reward for exploring different directions
        }
        else
        {
            AddReward(-0.01f); // Penalty for sticking to the same direction
        }
        previousDirection = currentDirection;

        lastSignificantMoveTime = Time.time; // Update time of significant move
        previousDistanceToTarget = distanceToTarget;

        // Check if the agent has reached the target
        if (distanceToTarget < 1.0f)
        {
            SetReward(1f);  // Max reward for reaching the target
            EndEpisode();
        }

        // Log current cumulative reward
        Debug.Log($"Current Cumulative Reward: {GetCumulativeReward()}");
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Debug logging for heuristic input
        Debug.Log($"Heuristic Input - X: {moveX}, Z: {moveZ}");

        continuousActionsOut[0] = moveX;
        continuousActionsOut[1] = moveZ;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Collision detected with: {collision.gameObject.name}");
    }
}
