using UnityEngine;
using TMPro;

public class Zombie : MonoBehaviour
{
    [Header("Statystyki")]
    public int maxHealth = 15;
    private int currentHealth;

    [Header("Atak i ruch")]
    public int damage = 20;
    public float speed = 2f;
    public float jumpForce = 5f;
    public float detectionRange = 10f;

    [Header("Fizyka")]
    public LayerMask groundLayer;
    public float rayLength = 0.5f;
    private bool facingRight = true;

    [Header("Cel")]
    public Transform player;

    [Header("UI")]
    [Tooltip("Prefab TextMeshPro do wyświetlania HP nad głową")]
    public TextMeshPro healthTextPrefab;
    private TextMeshPro healthTextInstance;
    public Vector3 healthOffset = new Vector3(0, 1.2f, 0);

    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        rb.freezeRotation = true;

        // 🩸 Stworzenie tekstu HP nad głową
        if (healthTextPrefab != null)
        {
            healthTextInstance = Instantiate(healthTextPrefab, transform.position + healthOffset, Quaternion.identity);
            healthTextInstance.text = $"{currentHealth}/{maxHealth}";
            healthTextInstance.alignment = TextAlignmentOptions.Center;

            var renderer = healthTextInstance.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.sortingLayerName = "UI";
                renderer.sortingOrder = 200;
            }
        }
    }

    void Update()
    {
        if (player == null) return;

        // 🔍 Sprawdź czy stoi na ziemi
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, rayLength, groundLayer);

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance > detectionRange)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            UpdateHealthText();
            return;
        }

        // 🧭 Idź w stronę gracza
        float dir = player.position.x > transform.position.x ? 1f : -1f;
        rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);
        facingRight = dir > 0;

        // 🪜 Skok przy przeszkodzie lub dziurze
        if (isGrounded && (IsObstacleAhead() || IsGapAhead()))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // 🔄 Obracanie sprite'a
        Vector3 scale = transform.localScale;
        scale.x = facingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;

        UpdateHealthText();
    }

    private void UpdateHealthText()
    {
        if (healthTextInstance != null)
        {
            healthTextInstance.text = $"{currentHealth}/{maxHealth}";
            healthTextInstance.transform.position = transform.position + healthOffset;
        }
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
                playerHealth.Damage(damage);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        UpdateHealthText();

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (healthTextInstance != null)
            Destroy(healthTextInstance.gameObject);

        Destroy(gameObject);
    }
}
