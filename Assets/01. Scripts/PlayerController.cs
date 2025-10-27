using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float jumpPower = 7f;
    [SerializeField] private Transform attackPoint;

    [Header("GroundCheck")]
    public Transform groundCheck;
    public float groundRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask platformLayer;

    [Header("Ladder")]
    public Tilemap ladderTilemap;
    [SerializeField] private float climbSpeed = 2f;
    public LayerMask ladderLayer;

    [Header("BasicAttack")]
    private float curTime;
    public float coolTime = 0.5f;
    public Transform pos;
    public Vector2 boxSize;



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
        Prone();
        Attack();
    }

    void Move()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float moveX = inputX * moveSpeed;

        rb.velocity = new Vector2(moveX, rb.velocity.y);

        if (inputX != 0)
        {
            transform.localScale = new Vector3(inputX > 0 ? -1 : 1, 1, 1);
            AttackPointFlip(inputX);
        }
        animator.SetBool("IsWalking", inputX != 0);
    }

    void AttackPointFlip(float inputX)
    {
        if (inputX == 0) 
        {
            return;
        }
        Vector3 attackPos = attackPoint.localScale;
        attackPos.x = Mathf.Abs(attackPos.x) * (inputX > 0 ? -1 : 1);
        attackPoint.localScale = attackPos;
    }

    void Jump()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer | platformLayer);

        if (isGrounded && Input.GetKey(KeyCode.LeftAlt) && isJump)
        {
            if (Input.GetAxisRaw("Vertical") < 0)
            {
                Collider2D platform = Physics2D.OverlapCircle(groundCheck.position, groundRadius, platformLayer);

                if (platform != null)
                {
                    StartCoroutine(DownJump(platform));
                    return;
                }

            }

            StartCoroutine(JumpCoolDown());
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
        }

        if (rb.velocity.y > 0.1f)
        {
            animator.SetBool("IsJumping", true);
        }
        else if (isGrounded)
        {
            animator.SetBool("IsJumping", false);
        }

    }


    void LadderCheck()
    {
        Collider2D ladder = Physics2D.OverlapBox(transform.position, new Vector2(0.12f, 0.2f), 0f, ladderLayer);

        if (ladder != null && Input.GetAxisRaw("Vertical") != 0)
        {
            isClimbing = true;
            rb.gravityScale = 0f;
            rb.velocity = Vector2.zero;

            Vector3Int cellPos = ladderTilemap.WorldToCell(transform.position);
            Vector3 cellCenter = ladderTilemap.GetCellCenterWorld(cellPos);
            transform.position = new Vector2(cellCenter.x, transform.position.y);

            animator.SetBool("IsClimbing", true);
        }

        if (isClimbing)
        {
            float inputY = Input.GetAxisRaw("Vertical");
            rb.velocity = new Vector2(rb.velocity.x, inputY * climbSpeed);

            if (Mathf.Abs(inputY) > 0.1f)
            {
                animator.speed = 1f;
            }
            else
            {
                animator.speed = 0f;
            }

            if (ladder == null || Input.GetKeyDown(KeyCode.LeftAlt))
            {
                isClimbing = false;
                rb.gravityScale = originalGravity;

                animator.SetBool("IsClimbing", false);
                animator.speed = 1f;

            }
        }
        else
        {
            animator.SetBool("IsClimbing", false);
            animator.speed = 1f;
        }
    }

    void Prone()
    {
        if(Input.GetAxisRaw("Vertical") < 0 && isGrounded && !isClimbing)
        {
            animator.SetBool("IsProne", true);
        }
        else
        {
            animator.SetBool("IsProne", false);
        }
    }

    void Attack()
    {
        if (curTime <= 0)
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                Collider2D[] collider2D = Physics2D.OverlapBoxAll(pos.position, boxSize, 0);
                foreach (Collider2D collider in collider2D)
                {
                    Debug.Log(collider.tag);
                }

                animator.SetTrigger("IsAttack");
                curTime = coolTime;
            }
        }
        else
        {
            curTime -= Time.deltaTime;
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

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(pos.position, boxSize);

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
