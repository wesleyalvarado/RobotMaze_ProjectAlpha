using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RobotAgent : Agent
{
    public float moveSpeed = 10f;

    public Vector2 floorSize = new Vector2(50f, 50f); // Size of the floor area for randomization
    public float floorY = 1f; // Y position of the floor

    public float minimumDistanceFromWalls = 2f; // Minimum distance from walls to avoid overlap
    public float floorHeight = 2f; // Height of the floor
    public Transform startPosition;  // Assign in Unity Editor for agent's start position
    public Transform exitPoint;      // Assign in Unity Editor for target position

    private float lastSignificantMoveTime;
    private float significantMoveThreshold = 1.0f; // Time in seconds to consider a move significant

    private Rigidbody rb;
    private float previousDistanceToTarget;
    private Vector3 previousDirection;

    public float raycastLength = 10f;
    public int numberOfRaycasts = 5;
    public float raycastAngleStep = 10f;

    private float frontDistance;
    private float leftDistance;
    private float rightDistance;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.useGravity = true;

        if (exitPoint == null) Debug.LogError("Exit point (target) is not assigned!");
    }

    public override void OnEpisodeBegin()
    {
        // Ensure the robot's position is reset to a random valid starting position
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Set a random start position
        transform.position = GetRandomStartPosition();

        if (exitPoint != null)
        {
            previousDistanceToTarget = Vector3.Distance(transform.position, exitPoint.position);
            previousDirection = transform.forward;
            lastSignificantMoveTime = Time.time;
        }
    }

    private Vector3 GetRandomStartPosition()
    {
        Vector3 randomPosition = Vector3.zero;
        bool validPosition = false;

        while (!validPosition)
        {

        // Generate a random position within the maze boundaries
        randomPosition = new Vector3(
            Random.Range(-floorSize.x / 2f, floorSize.x / 2f),
            3.0f, // Ensure the y-coordinate is set to the height of the floor
            Random.Range(-floorSize.y / 2f, floorSize.y / 2f)
        );

            // Check if the position is valid
            validPosition = IsValidStartPosition(randomPosition);

            // Debugging output to verify random positions and validity
            Debug.Log($"Generated Position: {randomPosition}, Valid: {validPosition}");
        }

        return randomPosition;
    }

    private bool IsValidStartPosition(Vector3 position)
    {
        // Check for collisions or proximity to walls
        Collider[] hitColliders = Physics.OverlapSphere(position, minimumDistanceFromWalls);
        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("Wall")) // Assuming walls are tagged with "Wall"
            {
                return false;
            }
        }

        return true;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(transform.forward);
        sensor.AddObservation(rb.velocity);
        sensor.AddObservation(exitPoint.position);

        RaycastHit hit;

        // Raycast forward
        frontDistance = Physics.Raycast(transform.position, transform.forward, out hit, raycastLength) ? hit.distance : raycastLength;
        sensor.AddObservation(frontDistance);

        // Raycast left
        leftDistance = Physics.Raycast(transform.position, -transform.right, out hit, raycastLength) ? hit.distance : raycastLength;
        sensor.AddObservation(leftDistance);

        // Raycast right
        rightDistance = Physics.Raycast(transform.position, transform.right, out hit, raycastLength) ? hit.distance : raycastLength;
        sensor.AddObservation(rightDistance);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        Vector3 direction = new Vector3(moveX, 0, moveZ).normalized;
        Vector3 movement = direction * moveSpeed;
        rb.AddForce(movement, ForceMode.VelocityChange);

        float distanceToTarget = Vector3.Distance(transform.position, exitPoint.position);
        float distanceDelta = previousDistanceToTarget - distanceToTarget;

        if (distanceDelta > 0)
        {
            float reward = Mathf.Clamp(distanceDelta / previousDistanceToTarget, 0.01f, 0.02f); // Reduced reward
            AddReward(reward);
        }
        else
        {
            AddReward(-0.001f); // Increased penalty for moving away from the target
        }

        if (rb.velocity.magnitude < 0.01f)
        {
            AddReward(-0.05f); // Reduced penalty for getting stuck
        }

        if (Time.time - lastSignificantMoveTime > significantMoveThreshold)
        {
            AddReward(-0.05f); // Reduced penalty for stalling
        }

        // Raycast-based penalties
        if (frontDistance < 1f || leftDistance < 1f || rightDistance < 1f)
        {
            AddReward(-0.05f); // Penalty for getting too close to obstacles
        }

        if (frontDistance > 2f)
        {
            AddReward(0.01f); // Small reward for maintaining a safe distance
        }

        Vector3 currentDirection = transform.forward;
        if (Vector3.Dot(currentDirection, previousDirection) < 0.9f)
        {
            AddReward(0.03f); // Reduced reward for exploring
        }
        else
        {
            AddReward(-0.001f); // Minor penalty for not exploring
        }

        previousDirection = currentDirection;
        lastSignificantMoveTime = Time.time;
        previousDistanceToTarget = distanceToTarget;

        if (distanceToTarget < 3.0f)
        {
            AddReward(100f); // Reward for reaching the goal
            EndEpisode();
            Debug.Log("Episode ended");
        }

        Debug.Log($"Current Cumulative Reward: {GetCumulativeReward()}");

        // Check if the agent falls below the floor level
        if (transform.position.y < floorHeight - 0.1f)
        {
            AddReward(-1f); // Large penalty for falling off
            EndEpisode();
            Debug.Log("Episode ended due to falling off the floor");
        }
    }

    // Method to check if the agent is close to a wall
    private bool IsCloseToWall()
    {
        RaycastHit hit;
        float rayDistance = 1.0f;
        bool hitWall = Physics.Raycast(transform.position, transform.forward, out hit, rayDistance) ||
                       Physics.Raycast(transform.position, -transform.forward, out hit, rayDistance) ||
                       Physics.Raycast(transform.position, transform.right, out hit, rayDistance) ||
                       Physics.Raycast(transform.position, -transform.right, out hit, rayDistance);
        return hitWall;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        continuousActionsOut[0] = moveX;
        continuousActionsOut[1] = moveZ;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Collision detected with: {collision.gameObject.name}");
    }
}
