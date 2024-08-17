using UnityEngine;

public class HorizontalObstacleMovement : MonoBehaviour
{
    public float moveSpeed = 2f;  // Speed of movement
    public float distance = 5f;   // Distance to move back and forth

    private Rigidbody rb;
    private Vector3 startPosition;
    private Vector3 targetPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Ensure Rigidbody is set up for physics interactions
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.isKinematic = false;  // Ensure physics interactions are enabled
        rb.useGravity = true;   // Disable gravity to prevent falling
        rb.drag = 0;             // Ensure no drag affects movement
        rb.angularDrag = 0;      // Ensure no angular drag affects rotation

        startPosition = transform.position;
    }

    void FixedUpdate()
    {
        // Calculate movement along the X-axis
        float movement = Mathf.PingPong(Time.time * moveSpeed, distance);
        targetPosition = startPosition + new Vector3(movement, 0, 0);

        // Move the Rigidbody towards the target position
        Vector3 direction = targetPosition - transform.position;
        rb.velocity = new Vector3(direction.x, 0, direction.z).normalized * moveSpeed;

        // Debugging
        Debug.Log("Object Position: " + transform.position);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Log collision information
        Debug.Log("Collided with: " + collision.gameObject.name);
    }

    void OnCollisionStay(Collision collision)
    {
        // Log ongoing collision information
        Debug.Log("Still colliding with: " + collision.gameObject.name);
    }

    void OnCollisionExit(Collision collision)
    {
        // Log when the collision ends
        Debug.Log("Stopped colliding with: " + collision.gameObject.name);
    }
}