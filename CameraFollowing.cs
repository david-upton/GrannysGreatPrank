using UnityEngine;

public class CameraFollowing : MonoBehaviour
{
    public Transform player1;  // Assign Player 1
    public Transform player2;  // Assign Player 2
    public float smoothSpeed = 0.1f;  // Smooth movement
    public float minSize = 5f;  // Minimum camera size
    public float zoomMultiplier = 1.5f; // Zoom scaling

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>(); // Get Camera component
    }

    void LateUpdate()
    {
        if (player1 == null || player2 == null) return; // Avoid errors if players are missing

        // **1. Find the midpoint between both players**
        Vector3 midpoint = (player1.position + player2.position) / 2f;
        midpoint.z = transform.position.z; // Keep original camera Z position

        // **2. Adjust the camera position smoothly**
        transform.position = Vector3.Lerp(transform.position, midpoint, smoothSpeed);

        // **3. Adjust the camera size based on player distance**
        float distance = Vector2.Distance(player1.position, player2.position);
        float newSize = Mathf.Max(minSize, distance * zoomMultiplier); // Ensure minimum size
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newSize, smoothSpeed);
    }
}
