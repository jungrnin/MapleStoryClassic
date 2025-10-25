using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 0f;
    public float jumpPower = 0f;

    [Header("GroundCheck")]
    public Transform groundCheck;
    public float groundRadius = 0.1f;
    public LayerMask groundLayer;


    [Header("Ladder")]
    public Tilemap ladderTilemap;
    public float climbSpeed = 3f;
    public LayerMask ladderLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isJump = true;
    private bool isClimbing = false;
    private float originalGravity;

    Animator animator;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 3f;
        rb.drag = 0f;
        originalGravity = rb.gravityScale;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!isClimbing)
        {
            Move();
        }
        Jump();
        LadderCheck();

        
    }

    void Move()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float moveX = inputX * moveSpeed;

        rb.velocity = new Vector2(moveX, rb.velocity.y);

        if (inputX != 0)
        {
            transform.localScale = new Vector3(inputX > 0 ? -1 : 1, 1, 1);
        }
        animator.SetBool("IsWalking", inputX != 0);
    }

    void Jump()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        if (isGrounded && Input.GetKey(KeyCode.LeftAlt) && isJump)
        {
            
            if (Input.GetAxisRaw("Vertical") < 0)
            {
                Collider2D platform = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

                if (platform != null)
                {
                    StartCoroutine(DownJump(platform));
                    return;
                }
                
            }
            
            StartCoroutine(JumpCoolDown());
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            
        }
    }

  
    void LadderCheck()
    {
        Collider2D ladder = Physics2D.OverlapBox(transform.position, new Vector2(0.12f, 0.2f), 0f, ladderLayer);

        if(ladder != null && Input.GetAxisRaw("Vertical")!=0)
        {
            isClimbing = true;
            rb.gravityScale = 0f;
            rb.velocity = Vector2.zero;

            Vector3Int cellPos = ladderTilemap.WorldToCell(transform.position);
            Vector3 cellCenter = ladderTilemap.GetCellCenterWorld(cellPos);

            transform.position = new Vector2(cellCenter.x,transform.position.y);


        }

        if(isClimbing)
        {
            float inputY = Input.GetAxisRaw("Vertical");
            rb.velocity = new Vector2(rb.velocity.x, inputY * climbSpeed);

           
            if (ladder == null || Input.GetKeyDown(KeyCode.LeftAlt))
            {
                isClimbing = false;
                rb.gravityScale = originalGravity;
               
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if(groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(groundCheck.position, groundRadius);
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.12f);
    }
    IEnumerator JumpCoolDown()
    {
        isJump = false;
        yield return new WaitForSeconds(0.45f);
        isJump = true;
    }

    IEnumerator DownJump(Collider2D platform)
    {
        if(platform == null)
        {
            yield break;
        }
        Collider2D playerCollider = GetComponent<Collider2D>();
        if(playerCollider == null)
        {
            yield break;
        }

        Physics2D.IgnoreCollision(playerCollider, platform, true);

        rb.velocity = new Vector2(rb.velocity.x, -3f);

        yield return new WaitForSeconds(0.3f);

        Physics2D.IgnoreCollision(playerCollider, platform, false);
    }
}
