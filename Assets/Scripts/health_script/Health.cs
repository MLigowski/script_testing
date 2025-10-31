using UnityEngine;
using TMPro;
using System.Collections;

public class Health : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI Prefabs")]
    public GameObject healthTextPrefab;
    private TextMeshPro healthTextTMP;

    [Header("UI References")]
    public TextMeshProUGUI healCooldownText;
    public TextMeshProUGUI deathCounterText;

    [Header("Healing Settings")]
    public KeyCode healKey = KeyCode.H;
    public int healAmount = 30;
    public float healCooldown = 5f;
    private bool canHeal = true;
    private float healTimer;

    [Header("Respawn Settings")]
    public float respawnDelay = 2f;

    [Header("Invincibility Settings")]
    public float invincibilityDuration = 5f;
    private bool isInvincible = false;

    private bool isDead = false;
    public static bool PlayerIsDead = false;
    private int deathCount = 0;

    private SpriteRenderer spriteRenderer;
    private PlayerRespawn respawnScript;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        respawnScript = GetComponent<PlayerRespawn>();

        if (healthTextPrefab != null)
        {
            GameObject textObj = Instantiate(healthTextPrefab, transform.position + Vector3.up * 1.2f, Quaternion.identity);
            textObj.transform.SetParent(transform);
            healthTextTMP = textObj.GetComponent<TextMeshPro>();
        }

        UpdateHealthText();
        UpdateHealCooldownUI();
        UpdateDeathCounterUI();
    }

    void Update()
    {
        if (isDead) return;

        if (Input.GetKeyDown(healKey) && canHeal && currentHealth < maxHealth)
        {
            Heal();
        }

        if (!canHeal)
        {
            healTimer -= Time.deltaTime;
            if (healTimer <= 0)
            {
                canHeal = true;
                UpdateHealCooldownUI();
            }
            else
            {
                UpdateHealCooldownUI();
            }
        }

        if (healthTextTMP != null)
        {
            healthTextTMP.transform.rotation = Quaternion.identity;
        }
    }

    public void Damage(int amount)
    {
        if (isDead || isInvincible) return;

        currentHealth -= amount;
        UpdateHealthText();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal()
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);
        canHeal = false;
        healTimer = healCooldown;
        UpdateHealthText();
        UpdateHealCooldownUI();
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        PlayerIsDead = true;
        deathCount++;
        UpdateDeathCounterUI();

        if (healthTextTMP != null)
        {
            healthTextTMP.text = "YOU ARE DEAD";
            healthTextTMP.fontSize = 15f;
            healthTextTMP.color = Color.red;
        }

        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        DisablePlayer();

        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        if (respawnScript != null)
        {
            respawnScript.RespawnPlayer();
        }

        currentHealth = maxHealth;
        UpdateHealthText();

        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        EnablePlayer();
        isDead = false;
        PlayerIsDead = false;

        StartCoroutine(InvincibilityFlash());
    }

    private IEnumerator InvincibilityFlash()
    {
        isInvincible = true;
        float timer = 0f;

        while (timer < invincibilityDuration)
        {
            if (spriteRenderer != null)
                spriteRenderer.enabled = !spriteRenderer.enabled;

            timer += 0.2f;
            yield return new WaitForSeconds(0.2f);
        }

        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        isInvincible = false;
    }

    private void DisablePlayer()
    {
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        var coll = GetComponent<Collider2D>();
        if (coll != null)
        {
            coll.enabled = false;
        }
    }

    private void EnablePlayer()
    {
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = true;
        }

        var coll = GetComponent<Collider2D>();
        if (coll != null)
        {
            coll.enabled = true;
        }
    }

    private void UpdateHealthText()
    {
        if (healthTextTMP == null) return;

        float ratio = (float)currentHealth / maxHealth;
        Color color = Color.green;
        if (ratio < 0.5f && ratio >= 0.25f)
            color = Color.yellow;
        else if (ratio < 0.25f)
            color = Color.red;

        healthTextTMP.fontSize = 7f;
        healthTextTMP.color = color;
        healthTextTMP.text = $"{currentHealth}/{maxHealth} HP";
    }

    private void UpdateHealCooldownUI()
    {
        if (healCooldownText == null) return;

        if (canHeal)
        {
            healCooldownText.text = "Heal ready [H]";
        }
        else
        {
            healCooldownText.text = $"Heal cooldown: {healTimer:F1}s";
        }
    }

    private void UpdateDeathCounterUI()
    {
        if (deathCounterText == null) return;
        deathCounterText.text = $"Deaths: {deathCount}";
    }
}
