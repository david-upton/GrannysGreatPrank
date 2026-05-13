using UnityEngine;

public class CameraFollowing : MonoBehaviour
{
    public Transform player1;  
    public Transform player2;  
    public float smoothSpeed = 0.1f;  
    public float minSize = 5f;  
    public float zoomMultiplier = 1.5f; 
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>(); 
    }

    void LateUpdate()
    {
        if (player1 == null || player2 == null) return; 

       
        Vector3 midpoint = (player1.position + player2.position) / 2f;
        midpoint.z = transform.position.z; 

       
        transform.position = Vector3.Lerp(transform.position, midpoint, smoothSpeed);

        
        float distance = Vector2.Distance(player1.position, player2.position);
        float newSize = Mathf.Max(minSize, distance * zoomMultiplier); 
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newSize, smoothSpeed);
    }
}
