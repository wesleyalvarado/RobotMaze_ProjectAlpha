using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class ManualAgent : Agent
{
    public float moveSpeed = 10f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        // Reset the agent's state at the start of each episode
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Collect observations from the environment
        sensor.AddObservation(transform.position);
        sensor.AddObservation(rb.velocity);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Apply actions to the robot
        // This part will be used during training and can be left empty for manual control
    }

    void Update()
    {
        // Manual control with keyboard input
        float moveX = 0;
        float moveZ = 0;

        if (Input.GetKey(KeyCode.W))
        {
            moveZ = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveZ = -1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveX = -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveX = 1;
        }

        Vector3 movement = new Vector3(moveX, 0, moveZ) * moveSpeed * Time.deltaTime;
        rb.MovePosition(transform.position + movement);
    }
}