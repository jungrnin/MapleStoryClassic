using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MonsterState
{ 
    Idel,
    Patrol,
    Chase,
    Knockback,
    Die
}
public class MonsterBase : MonoBehaviour
{
    [SerializeField] private MonsterData data;
    private int HP;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator animator;

    private bool moveRight = true;
    private Vector2 startPos;

    private MonsterState state = MonsterState.Patrol;

    [SerializeField] private float chaseRange = 5f;
    [SerializeField] private float chaseSpeed = 2.5f;
    private Transform targetPlayer;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        Initialize(data);
    }

    public void Initialize(MonsterData monsterData)
    {
        data = monsterData;
        HP = data.maxHp;
        sprite.sprite = data.sprite;
        animator.runtimeAnimatorController = data.animatorController;

        startPos = transform.position;
        state = MonsterState.Patrol;
    }

    private void Update()
    {
        switch (state)
        {
            case MonsterState.Patrol:
                Patrol();
                break;

            case MonsterState.Chase:
                Chase();
                break;

            case MonsterState.Knockback:
                break;

            case MonsterState.Die:
                rb.velocity = Vector2.zero;
                break;

        }

    }

    void Patrol()
    {
        float moveDir = moveRight ? 1 : -1;
        rb.velocity = new Vector2(moveDir * data.moveSpeed, rb.velocity.y);

        animator.SetBool("IsMoving", Mathf.Abs(rb.velocity.x) > 0.05f);

        if(moveRight && transform.position.x >= startPos.x + data.rightOffset)
        {
            moveRight = false;
            sprite.flipX = true;
        }
        else if(!moveRight && transform.position.x <= startPos.x + data.leftOffset)
        {
            moveRight = true;
            sprite.flipX = false;
        }
    }

    void Chase()
    {
        if(targetPlayer == null)
        {
            state = MonsterState.Patrol;
            return;
        }

        float distance = Vector2.Distance(transform.position, targetPlayer.position);

        if (distance > chaseRange * 1.5f)
        {
            targetPlayer = null;
            state = MonsterState.Patrol;
            return;
        }

        float direction = (targetPlayer.position.x - transform.position.x) > 0 ? 1 : -1;
        rb.velocity = new Vector2(direction * chaseSpeed, rb.velocity.y);

        sprite.flipX = direction < 0;
        animator.SetBool("IsMoving", true);
    }

    public void TakeDamage(int damage, Transform Player = null)
    {
        if(state == MonsterState.Die)
        {
            return;
        }

        HP -= damage;
        animator.SetTrigger("Hit");

        if(Player != null)
        {
            targetPlayer = Player;
            state = MonsterState.Chase;
            StartCoroutine(KnockbackRoutine(Player));
        }

        if (HP <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        if(state == MonsterState.Die)
        {
            return;
        }

        rb.velocity = Vector2.zero;
        animator.SetTrigger("Die");
        state = MonsterState.Die;
        //Die애니메이션 출력 후 파괴
        StartCoroutine(DeathDelay());
    }

    IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    IEnumerator KnockbackRoutine(Transform Player)
    {
        state = MonsterState.Knockback;

        Vector2 dir = (transform.position - Player.position).normalized;

        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(dir.x * 2f, 0), ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.5f);

        if (HP > 0 && targetPlayer != null)
        {
            state = MonsterState.Chase;
        }
        else if (HP > 0)
        {
            state = MonsterState.Patrol;
        }
    }

}
