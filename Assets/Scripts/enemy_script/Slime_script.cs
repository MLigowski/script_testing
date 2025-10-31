using UnityEngine;
using TMPro;

public class Slime : MonoBehaviour
{
    [Header("Statystyki")]
    public int maxHealth = 5;
    private int currentHealth;

    [Header("Atak i ruch")]
    public int damage = 10;
    public float jumpForce = 5f;
    public float jumpCooldown = 1.5f;
    public float moveSpeed = 2f;
    public float detectionRange = 8f;

    [Header("Fizyka")]
    public LayerMask groundLayer;
    private Rigidbody2D rb;
    private bool isGrounded;
    private float nextJumpTime = 0f;

    [Header("Cel")]
    public Transform player;

    [Header("UI")]
    public TextMeshPro healthTextPrefab;
    private TextMeshPro healthTextInstance;
    public Vector3 healthOffset = new Vector3(0, 1.2f, 0);

    private SpriteRenderer sr;
    private bool facingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;

        // Naprawa ślizgania: blokada obrotu + zwiększone tarcie
        rb.freezeRotation = true;
        rb.sharedMaterial = new PhysicsMaterial2D() { friction = 1f, bounciness = 0f };

        // Tworzymy tekst HP nad Slime
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

        // Sprawdź, czy stoi na ziemi
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.6f, groundLayer);

        float distance = Vector2.Distance(transform.position, player.position);

        // Skakanie tylko jeśli cooldown minął
        if (Time.time >= nextJumpTime && isGrounded)
        {
            if (distance <= detectionRange)
                JumpTowardsPlayer();
            else
                JumpRandomly();

            nextJumpTime = Time.time + jumpCooldown;
        }

        // Aktualizacja tekstu HP nad głową
        if (healthTextInstance != null)
        {
            healthTextInstance.transform.position = transform.position + healthOffset;
        }
    }

    void JumpTowardsPlayer()
    {
        float dirX = player.position.x - transform.position.x;
        float dir = dirX > 0 ? 1f : -1f;

        rb.linearVelocity = new Vector2(dir * moveSpeed, jumpForce);

        facingRight = dir > 0;
        UpdateDirection();
    }

    void JumpRandomly()
    {
        float dir = Random.value < 0.5f ? -1f : 1f;
        rb.linearVelocity = new Vector2(dir * moveSpeed * 0.8f, jumpForce * 0.9f);

        facingRight = dir > 0;
        UpdateDirection();
    }

    void UpdateDirection()
    {
        Vector3 scale = transform.localScale;
        scale.x = facingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Zadaje obrażenia co każde zetknięcie z graczem
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

    void UpdateHealthText()
    {
        if (healthTextInstance != null)
        {
            healthTextInstance.text = $"{currentHealth}/{maxHealth}";

            // Kolor HP (zielony → żółty → czerwony)
            float healthPercent = (float)currentHealth / maxHealth;
            if (healthPercent > 0.6f)
                healthTextInstance.color = Color.green;
            else if (healthPercent > 0.3f)
                healthTextInstance.color = Color.yellow;
            else
                healthTextInstance.color = Color.red;
        }
    }

    void Die()
    {
        if (healthTextInstance != null)
            Destroy(healthTextInstance.gameObject);

        Destroy(gameObject);
    }
}
