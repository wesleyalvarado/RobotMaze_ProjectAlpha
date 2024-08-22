using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RobotAgentArchive : Agent
{
    public float moveSpeed = 10f;
    public Transform target;  // Endpoint that the agent should move towards

    public Vector3 robotStartPosition; // Exposed start position for the robot
    public Vector3 targetStartPosition; // Exposed start position for the target

    private float previousDistanceToTarget;

    void Start()
    {
        // Initialization code if needed
    }

    public override void OnEpisodeBegin()
    {
        // Reset robot and target positions
        transform.position = robotStartPosition;
        if (target != null)
        {
            target.position = targetStartPosition;
        }

        // Initialize previous distance to target
        previousDistanceToTarget = Vector3.Distance(transform.position, target.position);
    }

public override void CollectObservations(VectorSensor sensor)
{
    // Existing observations
    sensor.AddObservation(transform.position / 10f);
    sensor.AddObservation(transform.forward);
    sensor.AddObservation(target.position / 10f);

    // Additional observations for enhanced autonomy
    // For example, observing the distance to obstacles
    RaycastHit hit;
    if (Physics.Raycast(transform.position, transform.forward, out hit, 10f))
    {
        sensor.AddObservation(hit.distance);
    }

}


    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        Debug.Log($"MoveX: {moveX}, MoveZ: {moveZ}");

        // Move the robot using Transform
        Vector3 movement = new Vector3(moveX, 0, moveZ) * moveSpeed * Time.deltaTime;
        transform.position += movement;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget < previousDistanceToTarget)
        {
            AddReward(0.05f);
        }
        else
        {
            AddReward(-0.05f);
        }

        previousDistanceToTarget = distanceToTarget;

        if (distanceToTarget < 1.0f)
        {
            SetReward(1f);
            Debug.Log("Target reached. Reward: 1.0");
            EndEpisode();
        }

        Debug.Log($"Current Reward: {GetCumulativeReward()}, Distance to Target: {distanceToTarget}");
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}
