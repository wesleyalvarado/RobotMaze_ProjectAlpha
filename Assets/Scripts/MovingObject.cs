using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    public float speed = 4f;          // Speed of movement
    public float distance = 20f;      // Distance to move
    private Vector3 startPosition;    // Starting position of the obstacle
    private Rigidbody rb;             // Reference to the Rigidbody

    void Start()
    {
        // Store the starting position
        startPosition = transform.position;

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
    }

    void FixedUpdate()
    {
        // Calculate movement using PingPong for back-and-forth motion along the X-axis
        float movement = Mathf.PingPong(Time.time * speed, distance);

        // Calculate the new position
        Vector3 newPosition = startPosition + new Vector3(movement, 0, 0);

        // Ensure the Y and Z positions stay the same
        newPosition.y = transform.position.y;
        newPosition.z = transform.position.z;

        // Draw a line to visualize movement
        Debug.DrawLine(transform.position, newPosition, Color.red);

        // Move the Rigidbody to the new position
        rb.MovePosition(newPosition);

        // Adjust velocity for consistent movement speed
        rb.velocity = (newPosition - transform.position) / Time.fixedDeltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Log collision information
        Debug.Log("MO Collided with: " + collision.gameObject.name);
    }
}
