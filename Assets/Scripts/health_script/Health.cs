using UnityEngine;
using TMPro;
using System.Collections;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Healing")]
    public int healAmount = 25;
    public float healCooldown = 10f;
    private bool canHeal = true;
    private float healCooldownTimer;

    [Header("UI Prefabs (3D Text)")]
    public GameObject healthTextPrefab;
    private GameObject healthTextInstance;
    private TextMeshPro healthTextTMP;
    public Vector3 healthOffset = new Vector3(0, 1.2f, 0);

    [Header("UI (Canvas)")]
    public TextMeshProUGUI healCooldownText;
    public TextMeshProUGUI deathCounterText;

    [Header("Respawn and Invincibility")]
    public float invincibilityTime = 5f;
    private bool isInvincible = false;
    private int deathCount = 0;
    private Vector3 spawnPoint;

    private SpriteRenderer spriteRenderer;
    private Coroutine blinkRoutine;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spawnPoint = transform.position;

        // Create floating health text (3D TextMeshPro)
        if (healthTextPrefab != null)
        {
            healthTextInstance = Instantiate(healthTextPrefab, transform.position + healthOffset, Quaternion.identity);
            healthTextTMP = healthTextInstance.GetComponent<TextMeshPro>();
        }

        UpdateHealthBar();
        UpdateDeathCounter();
        UpdateHealCooldownUI();
    }

    void Update()
    {
        // Heal (H)
        if (Input.GetKeyDown(KeyCode.H) && canHeal && currentHealth < maxHealth)
        {
            Heal(healAmount);
            StartCoroutine(HealCooldown());
        }

        // Cooldown text update
        if (!canHeal)
        {
            healCooldownTimer -= Time.deltaTime;
            UpdateHealCooldownUI();
        }

        // Always update floating text position
        if (healthTextInstance != null)
        {
            healthTextInstance.transform.position = transform.position + healthOffset;
            healthTextInstance.transform.rotation = Quaternion.identity;
        }
    }

    public void Damage(int amount)
    {
        if (isInvincible) return;

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            StartCoroutine(DieAndRespawn());
        }

        UpdateHealthBar();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        UpdateHealthBar();
    }

    private IEnumerator HealCooldown()
    {
        canHeal = false;
        healCooldownTimer = healCooldown;

        while (healCooldownTimer > 0)
        {
            UpdateHealCooldownUI();
            yield return null;
        }

        canHeal = true;
        UpdateHealCooldownUI();
    }

    private IEnumerator DieAndRespawn()
    {
        deathCount++;
        UpdateDeathCounter();

        if (healthTextTMP != null)
        {
            healthTextTMP.text = "YOU ARE DEAD";
            healthTextTMP.fontSize = 15;
            healthTextTMP.color = Color.red;
        }

        yield return new WaitForSeconds(2f);

        transform.position = spawnPoint;
        currentHealth = maxHealth;
        UpdateHealthBar();

        StartCoroutine(Invincibility());
    }

    private IEnumerator Invincibility()
    {
        isInvincible = true;
        float timer = 0f;
        if (spriteRenderer == null) yield break;

        blinkRoutine = StartCoroutine(BlinkEffect());

        while (timer < invincibilityTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);

        spriteRenderer.enabled = true;
        isInvincible = false;
    }

    private IEnumerator BlinkEffect()
    {
        while (true)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void UpdateHealthBar()
    {
        if (healthTextTMP == null) return;

        if (currentHealth <= 0)
        {
            healthTextTMP.text = "YOU ARE DEAD";
            healthTextTMP.fontSize = 15;
            healthTextTMP.color = Color.red;
        }
        else
        {
            healthTextTMP.text = $"HP: {currentHealth}/{maxHealth}";
            healthTextTMP.fontSize = 3;

            if (currentHealth > maxHealth * 0.6f)
                healthTextTMP.color = Color.green;
            else if (currentHealth > maxHealth * 0.3f)
                healthTextTMP.color = Color.yellow;
            else
                healthTextTMP.color = Color.red;
        }
    }

    private void UpdateHealCooldownUI()
    {
        if (healCooldownText == null) return;

        if (canHeal)
            healCooldownText.text = "<color=green>Heal Ready (H)</color>";
        else
            healCooldownText.text = $"<color=red>Heal CD: {healCooldownTimer:F1}s</color>";
    }

    private void UpdateDeathCounter()
    {
        if (deathCounterText != null)
            deathCounterText.text = $"<color=red>Deaths: {deathCount}</color>";
    }
}
