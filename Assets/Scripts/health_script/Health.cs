using UnityEngine;
using TMPro;
using System.Collections;

public class Health : MonoBehaviour
{
    [Header("Statystyki zdrowia")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("UI nad głową (3D TMP)")]
    public TextMeshPro healthTextPrefab;
    private TextMeshPro healthTextInstance;
    public Vector3 healthTextOffset = new Vector3(0, 1.5f, 0);

    [Header("UI na ekranie (Canvas TMP)")]
    public TextMeshProUGUI healCooldownText; // TMP w lewym górnym rogu
    public TextMeshProUGUI deathCountText;   // TMP poniżej cooldownu

    private int deathCount = 0;
    private PlayerRespawn respawnScript;

    void Start()
    {
        currentHealth = maxHealth;
        respawnScript = GetComponent<PlayerRespawn>();

        // 🧾 Stwórz tekst nad głową (3D TMP)
        if (healthTextPrefab != null)
        {
            healthTextInstance = Instantiate(healthTextPrefab, transform.position + healthTextOffset, Quaternion.identity);
            healthTextInstance.text = $"HP: {currentHealth}/{maxHealth}";
            healthTextInstance.alignment = TextAlignmentOptions.Center;
            healthTextInstance.fontSize = 2f;

            var renderer = healthTextInstance.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.sortingLayerName = "UI";
                renderer.sortingOrder = 200;
            }
        }

        // 🖥️ Canvas UI inicjalizacja
        if (healCooldownText != null)
            healCooldownText.text = "Heal cooldown: 0s";

        if (deathCountText != null)
            deathCountText.text = "Deaths: 0";
    }

    void Update()
    {
        // Aktualizuj pozycję napisu nad głową
        if (healthTextInstance != null)
            healthTextInstance.transform.position = transform.position + healthTextOffset;
    }

    public void Damage(int amount)
    {
        if (respawnScript != null && respawnScript.IsInvincible())
            return;

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }

        UpdateHealthText();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHealthText();
    }

    private void Die()
    {
        deathCount++;

        // 🔴 Nad głową pokazuje się “You are dead”
        if (healthTextInstance != null)
        {
            healthTextInstance.text = "You are dead";
            healthTextInstance.color = Color.red;
        }

        // 🔢 Licznik śmierci w UI
        if (deathCountText != null)
            deathCountText.text = $"Deaths: {deathCount}";

        if (respawnScript != null)
            respawnScript.RespawnPlayer();

        StartCoroutine(ResetHealthAfterRespawn());
    }

    private IEnumerator ResetHealthAfterRespawn()
    {
        yield return new WaitForSeconds(0.2f);
        currentHealth = maxHealth;
        UpdateHealthText();
    }

    private void UpdateHealthText()
    {
        if (healthTextInstance != null)
        {
            healthTextInstance.text = $"HP: {currentHealth}/{maxHealth}";
            healthTextInstance.color = Color.green;
        }
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
}
