using UnityEngine;

public class Interactions : MonoBehaviour
{
    public GameObject npcCanvas;  

    void Start()
    {
        npcCanvas.SetActive(false);  // Hide canvas initially
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            npcCanvas.SetActive(true);  // Show UI when player enters
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            npcCanvas.SetActive(false);  // Hide UI when player leaves
        }
    }
}
