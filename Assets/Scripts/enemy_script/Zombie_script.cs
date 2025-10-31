using UnityEngine;
using TMPro;

public class Zombie : MonoBehaviour
{
    [Header("Statystyki")]
    public int maxHealth = 15;
    private int currentHealth;

    [Header("Atak i ruch")]
    public int damage = 20;
    public float damageInterval = 1f; // ⏱️ obrażenia co 1 sek.
    public float speed = 2f;
    public float jumpForce = 5f;
    public float detectionRange = 10f;

    [Header("Fizyka")]
    public LayerMask groundLayer;
    public float rayLength = 0.5f;
    private bool facingRight = true;

    [Header("Cel")]
    public Transform player;

    [Header("HP Text UI")]
    public GameObject healthTextPrefab;
    private TextMeshPro healthText;
    public Vector3 textOffset = new Vector3(0, 1f, 0);

    private Rigidbody2D rb;
    private bool isGrounded;
    private float nextDamageTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        rb.linearDamping = 5f; // 🧊 mniej ślizgania

        currentHealth = maxHealth;

        // 🩸 Stworzenie tekstu HP nad głową
        if (healthTextPrefab != null)
        {
            GameObject textObj = Instantiate(healthTextPrefab, transform.position + textOffset, Quaternion.identity);
            healthText = textObj.GetComponent<TextMeshPro>();
        }

        UpdateHealthText();
    }

    void Update()
    {
        if (player == null) return;

        // Aktualizacja pozycji tekstu HP
        if (healthText != null)
        {
            healthText.transform.position = transform.position + textOffset;
        }

        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, rayLength, groundLayer);

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance > detectionRange)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        float dir = player.position.x > transform.position.x ? 1f : -1f;
        rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);
        facingRight = dir > 0;

        if (isGrounded && (IsObstacleAhead() || IsGapAhead()))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        Vector3 scale = transform.localScale;
        scale.x = facingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    private bool IsObstacleAhead()
    {
        Vector2 origin = (Vector2)transform.position + Vector2.up * 0.2f;
        Vector2 direction = facingRight ? Vector2.right : Vector2.left;
        return Physics2D.Raycast(origin, direction, rayLength, groundLayer);
    }

    private bool IsGapAhead()
    {
        Vector2 origin = (Vector2)transform.position + (facingRight ? Vector2.right : Vector2.left) * 0.4f;
        return !Physics2D.Raycast(origin, Vector2.down, rayLength + 0.1f, groundLayer);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && Time.time >= nextDamageTime)
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
                playerHealth.Damage(damage);

            nextDamageTime = Time.time + damageInterval;
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
            if (healthText != null)
                Destroy(healthText.gameObject);
        }
        else
        {
            UpdateHealthText();
        }
    }

    private void UpdateHealthText()
    {
        if (healthText == null) return;

        healthText.text = $"HP: {currentHealth}/{maxHealth}";

        float healthPercent = (float)currentHealth / maxHealth;
        if (healthPercent > 0.6f)
            healthText.color = Color.green;
        else if (healthPercent > 0.3f)
            healthText.color = Color.yellow;
        else
            healthText.color = Color.red;
    }
}
