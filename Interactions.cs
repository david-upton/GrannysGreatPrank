using UnityEngine;

public class Interactions : MonoBehaviour
{
    public GameObject npcCanvas;  

    void Start()
    {
        npcCanvas.SetActive(false);  
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            npcCanvas.SetActive(true);  
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            npcCanvas.SetActive(false);  
        }
    }
}
