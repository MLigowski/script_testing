using UnityEngine;

public class SwordManEnemy : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;      // tu przeci¹gasz animator z body
    public Rigidbody2D rb;

    [Header("Body Parts (flip)")]
    public Transform body;
    public Transform head;
    public Transform weapon;
    public Transform leg1;
    public Transform leg2;

    [Header("Stats")]
    public float speed = 2f;
    public float detectRange = 6f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public int damage = 5;
    public int health = 20;

    [Header("Layers")]
    public LayerMask playerLayer;

    float attackTimer;
    bool isDead;

    void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectRange)
        {
            ChasePlayer();

            if (distance <= attackRange)
            {
                Attack();
            }
        }
        else
        {
            Idle();
        }
    }

    void ChasePlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;

        rb.linearVelocity = new Vector2(dir.x * speed, rb.linearVelocity.y);

        animator.SetBool("Walk", true);
        animator.SetBool("Attack", false);
        Debug.Log("Walk TRUE");

        Flip(dir.x);
    }

    void Idle()
    {
        rb.linearVelocity = Vector2.zero;

        animator.SetBool("Walk", false);
        animator.SetBool("Attack", false);
    }

    void Attack()
    {
        rb.linearVelocity = Vector2.zero;

        animator.SetBool("Attack", true);

        if (attackTimer <= 0)
        {
            attackTimer = attackCooldown;

            Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);

            if (hit != null)
            {
                hit.GetComponent<Health>()?.Damage(damage);
            }
        }

        attackTimer -= Time.deltaTime;
    }

    void Flip(float dir)
    {
        float scale = dir > 0 ? 1 : -1;

        body.localScale = new Vector3(scale, 1, 1);
        head.localScale = new Vector3(scale, 1, 1);
        weapon.localScale = new Vector3(scale, 1, 1);
        leg1.localScale = new Vector3(scale, 1, 1);
        leg2.localScale = new Vector3(scale, 1, 1);
    }

    public void TakeDamage(int dmg)
    {
        if (isDead) return;

        health -= dmg;

        if (health <= 0)
            Die();
    }

    void Die()
    {
        isDead = true;

        animator.SetBool("isDead", true);

        rb.linearVelocity = Vector2.zero;

        Destroy(gameObject, 2f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}