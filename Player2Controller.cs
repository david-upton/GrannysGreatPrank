using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Player2Controller : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 10f;
    public float dashSpeed = 15f;
    public float dashCooldown = 1f; 
    public float dashDuration = 0.2f; 
    public Transform groundCheck;
    public LayerMask groundLayer;
     public Transform spawnPoint;
     public static Transform sharedSpawnPoint;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool facingRight = true;
    private Collider2D currentStairs; 
    private float moveInput;
    private bool isDashing = false;
    private float nextDashTime = 0f;
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
        
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        if (!isDashing) 
        {
            moveInput = 0f;
            if (Input.GetKey(KeyCode.LeftArrow)) moveInput = -1f;
            if (Input.GetKey(KeyCode.RightArrow)) moveInput = 1f;

            if (moveInput > 0 && !facingRight)
                Flip();
            else if (moveInput < 0 && facingRight)
                Flip();

            movement.x = moveInput;
        }

        
        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            anim.SetTrigger("Jump"); 
        }

        
        if (Input.GetKey(KeyCode.DownArrow) && currentStairs != null)
        {
            StartCoroutine(DropThroughStairs());
        }

        
        if (Input.GetKeyDown(KeyCode.RightShift) && Time.time >= nextDashTime)
        {
            StartCoroutine(Dash());
        }

        
        anim.SetBool("isWalking", movement.x != 0); 
        anim.SetBool("isGrounded", isGrounded); 
        anim.SetBool("isDashing", isDashing);
        anim.SetBool("isJumping", !isGrounded && rb.velocity.y > 0); 
        anim.SetBool("isFalling", !isGrounded && rb.velocity.y < 0); 
    }

    void FixedUpdate()
    {
        if (!isDashing) 
        {
            rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;
        nextDashTime = Time.time + dashCooldown;

        float originalGravity = rb.gravityScale; 
        rb.gravityScale = 0f; 
        float dashDirection = facingRight ? 1f : -1f;
        rb.velocity = new Vector2(dashDirection * dashSpeed, 0f); 

        anim.SetTrigger("Dash"); 
        yield return new WaitForSeconds(dashDuration); 

        rb.velocity = Vector2.zero; 
        rb.gravityScale = originalGravity; 
        isDashing = false;
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

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == currentStairs)
        {
            currentStairs = null;
        }
    }

        void RespawnAllPlayers()
    {
        
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
}
