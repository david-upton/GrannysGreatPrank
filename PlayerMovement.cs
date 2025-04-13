using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 10f;
    public float breakRadius = 1f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public Transform respawnPoint;
     public Transform spawnPoint;
     public static Transform sharedSpawnPoint;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool facingRight = true;
    private Collider2D currentStairs;
    private Animator anim;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        anim = GetComponent<Animator>();
          if (sharedSpawnPoint == null)
        {
            sharedSpawnPoint = transform;
        }
    }

    void Update()
    {
        float moveInput = 0f;

        if (Input.GetKey(KeyCode.A)) moveInput = -1f;
        if (Input.GetKey(KeyCode.D)) moveInput = 1f;

        if (moveInput > 0 && !facingRight)
            Flip();
        else if (moveInput < 0 && facingRight)
            Flip();

        movement.x = moveInput;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            anim.SetTrigger("Jump");
        }

        if (Input.GetKey(KeyCode.S) && currentStairs != null)
        {
            StartCoroutine(DropThroughStairs());
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryBreakTile();
        }

        // **Update Animator Parameters**
        anim.SetBool("isWalking", movement.x != 0);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isJumping", !isGrounded && rb.velocity.y > 0);
        anim.SetBool("isFalling", !isGrounded && rb.velocity.y < 0);
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(movement.x * speed, rb.velocity.y);
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private IEnumerator DropThroughStairs()
    {
        Collider2D stairs = currentStairs;
        stairs.enabled = false;

        yield return new WaitForSeconds(0.5f);

        stairs.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Stairs"))
        {
            currentStairs = other;
        }

        if (other.CompareTag("Danger"))
        {
            RespawnAllPlayers(); 
        }

        
        if (other.CompareTag("Checkpoint"))
        {
            UpdateSpawnPoint(other.transform);
        }
    }

    void RespawnAllPlayers()
    {
        // Find both players and move them to the shared spawn point
        PlayerController[] players1 = FindObjectsOfType<PlayerController>();
        Player2Controller[] players2 = FindObjectsOfType<Player2Controller>();

        foreach (PlayerController player in players1)
        {
            player.transform.position = sharedSpawnPoint.position;
        }
        foreach (Player2Controller player in players2)
        {
            player.transform.position = sharedSpawnPoint.position;
        }
         
    }

    void UpdateSpawnPoint(Transform newSpawnPoint)
    {
        sharedSpawnPoint = newSpawnPoint;
        Debug.Log("Checkpoint updated for both players!");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == currentStairs)
        {
            currentStairs = null;
        }
    }


    private void TryBreakTile()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, breakRadius);
        foreach (Collider2D col in colliders)
        {
            if (col.CompareTag("Breakable"))
            {
                anim.SetTrigger("Stomp");
                StartCoroutine(BreakTileWithDelay(col));
                break;
        }
    }
    IEnumerator BreakTileWithDelay(Collider2D col)
    {
    yield return new WaitForSeconds(0.5f); // Adjust time to match animation length

    if (col != null)
    {
        col.enabled = false;
        Destroy(col.gameObject);
    }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, breakRadius);
    }
}
}  