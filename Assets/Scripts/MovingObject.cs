using UnityEngine;
using Unity.MLAgents;

public class ObstacleMovement : MonoBehaviour
{
    public float speed = 4f;          // Speed of movement
    public float distance = 20f;      // Distance to move
    public LayerMask wallLayer;       // Layer for walls to check collisions
    public Vector2 floorSize = new Vector2(50f, 50f); // Size of the floor area for randomization
    public float floorY = 0f;         // Y position of the floor

    private Vector3 startPosition;    // Starting position of the obstacle
    private Rigidbody rb;             // Reference to the Rigidbody

    void Start()
    {
        // Get the Rigidbody component attached to this GameObject
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        // Ensure Rigidbody is not kinematic and gravity is disabled
        rb.isKinematic = false;       // Allow physics interactions
        rb.useGravity = false;        // Disable gravity to prevent floating
        rb.drag = 0;                  // No drag to affect movement
        rb.angularDrag = 0;           // No angular drag to prevent rotation effects

        // Prevent rotation by freezing all rotation axes
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // Randomize the starting position
        RandomizePosition();
    }

    void FixedUpdate()
    {
        if (rb.velocity.magnitude < 0.1f)
{
        rb.AddForce(Vector3.up * 0.1f, ForceMode.Impulse); // Small upward force
}
        // Calculate movement using PingPong for back-and-forth motion along the X-axis
        float movement = Mathf.PingPong(Time.time * speed, distance);

        // Calculate the new position
        Vector3 newPosition = startPosition + new Vector3(movement, 0, 0);

        // Ensure the Y and Z positions stay the same
        newPosition.y = transform.position.y;
        newPosition.z = transform.position.z;

        // Draw a line to visualize movement
        Debug.DrawLine(transform.position, newPosition, Color.red);

        // Set the velocity instead of moving the position directly
        rb.velocity = (newPosition - transform.position) / Time.fixedDeltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Log collision information
        Debug.Log("MO Collided with: " + collision.gameObject.name);

        // Apply a bounce back force if the object collides with a wall
        if ((wallLayer & (1 << collision.gameObject.layer)) != 0) // Check if collision layer is in wallLayer
        {
            Vector3 collisionNormal = collision.contacts[0].normal;
            rb.AddForce(collisionNormal * 2f, ForceMode.Impulse); // Bounce back force
        }
    }

void RandomizePosition()
{
    bool validPositionFound = false;
    Vector3 randomPosition = Vector3.zero;

    while (!validPositionFound)
    {
        // Generate a random position within the floor area
        randomPosition = new Vector3(
            Random.Range(-floorSize.x / 2f, floorSize.x / 2f),
            floorY,
            Random.Range(-floorSize.y / 2f, floorSize.y / 2f)
        );

        // Check if the position is not on top of a wall
        bool isOnWall = Physics.CheckBox(randomPosition, Vector3.one * 0.1f, Quaternion.identity, wallLayer);
        bool isOverlappingOtherObstacles = false;

        // Check if the position is overlapping with other obstacles
        Collider[] overlappingObstacles = Physics.OverlapBox(randomPosition, Vector3.one * 0.1f);
        foreach (Collider collider in overlappingObstacles)
        {
            if (collider.gameObject != gameObject) // Exclude itself
            {
                isOverlappingOtherObstacles = true;
                break;
            }
        }

        if (!isOnWall && !isOverlappingOtherObstacles)
        {
            validPositionFound = true;
        }
    }

    // Set the position if it's valid
    startPosition = randomPosition;
    transform.position = startPosition;
}
    // Method to reset the position at the start of each episode
    public void ResetObstacle()
    {
        RandomizePosition();
    }
}
