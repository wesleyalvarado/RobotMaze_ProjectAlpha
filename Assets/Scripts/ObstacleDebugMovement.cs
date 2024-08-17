using UnityEngine;

public class ObstacleDebugMovement : MonoBehaviour
{
    public float moveSpeed = 5f;  // Speed at which the object moves
    public bool showDebug = true;  // Toggle to show debug logs

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Ensure Rigidbody is set up for physics interactions
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = 1f;  // Set reasonable mass
        }

        // Ensure Rigidbody is not kinematic and uses gravity
        rb.isKinematic = false;
        rb.useGravity = true;
    }

    void Update()
    {
        // Manual movement controls using arrow keys or WASD
        float moveHorizontal = Input.GetAxis("Horizontal"); // Left/Right or A/D
        float moveVertical = Input.GetAxis("Vertical");     // Up/Down or W/S

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rb.MovePosition(transform.position + movement * moveSpeed * Time.deltaTime);

        // Debugging object position
        if (showDebug)
        {
            Debug.Log("Object Position: " + transform.position);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Log collision information
        if (showDebug)
        {
            Debug.Log("Collided with: " + collision.gameObject.name);
            Debug.Log("Collision Point: " + collision.contacts[0].point);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        // Log ongoing collision information
        if (showDebug)
        {
            Debug.Log("Still colliding with: " + collision.gameObject.name);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // Log when the collision ends
        if (showDebug)
        {
            Debug.Log("Stopped colliding with: " + collision.gameObject.name);
        }
    }
}
