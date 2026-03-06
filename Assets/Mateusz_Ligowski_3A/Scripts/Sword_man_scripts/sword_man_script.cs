using UnityEngine;

public class SwordManAI : MonoBehaviour
{
    public Transform player;

    public float speed = 3f;
    public float detectionRange = 8f;
    public float attackRange = 1.5f;

    public float attackCooldown = 1.2f;
    private float attackTimer;

    public int damage = 10;

    public int maxHealth = 50;
    private int currentHealth;

    public LayerMask playerLayer;

    private Rigidbody2D rb;
    private Animator anim;

    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        if (anim == null)
        {
            Debug.LogError("Animator NOT FOUND");
        }
        else
        {
            Debug.Log("Animator OK");
        }

        currentHealth = maxHealth;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (isDead) return;

        attackTimer -= Time.deltaTime;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            if (distance > attackRange)
            {
                ChasePlayer();
            }
            else
            {
                AttackPlayer();
            }
        }
        else
        {
            anim.SetBool("Walk", false);
        }
    }

    void ChasePlayer()
    {
        anim.SetBool("Walk", true);
        anim.SetBool("Attack", false);

        Vector2 direction = (player.position - transform.position).normalized;

        rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);

        if (direction.x > 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
    }

    void AttackPlayer()
    {
        rb.linearVelocity = Vector2.zero;

        anim.SetBool("Walk", false);
        anim.SetBool("Attack", true);

        if (attackTimer <= 0)
        {
            attackTimer = attackCooldown;

            Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);

            if (hit != null)
            {
                hit.GetComponent<Health>()?.Damage(damage);
            }
        }
    }

    public void TakeDamage(int dmg)
    {
        if (isDead) return;

        currentHealth -= dmg;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        anim.SetBool("isDead", true);

        rb.linearVelocity = Vector2.zero;

        Destroy(gameObject, 2f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}