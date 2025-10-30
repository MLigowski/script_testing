using UnityEngine;
using System.Collections;

public class PlayerRespawn : MonoBehaviour
{
    [Header("Respawn Settings")]
    public Transform spawnPoint;
    public float invincibilityTime = 5f;

    private bool isInvincible = false;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        if (spawnPoint == null)
        {
            GameObject defaultSpawn = new GameObject("DefaultSpawnPoint");
            defaultSpawn.transform.position = transform.position;
            spawnPoint = defaultSpawn.transform;
        }
    }

    public void RespawnPlayer()
    {
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        transform.position = spawnPoint.position;
        isInvincible = true;

        if (sr != null)
            StartCoroutine(InvincibilityFlash());

        yield return new WaitForSeconds(invincibilityTime);

        isInvincible = false;

        if (sr != null)
            sr.color = new Color(1, 1, 1, 1);
    }

    private IEnumerator InvincibilityFlash()
    {
        float flashInterval = 0.2f;
        while (isInvincible)
        {
            if (sr != null)
                sr.color = new Color(1, 1, 1, 0.3f);
            yield return new WaitForSeconds(flashInterval);
            if (sr != null)
                sr.color = new Color(1, 1, 1, 1f);
            yield return new WaitForSeconds(flashInterval);
        }
    }

    public bool IsInvincible()
    {
        return isInvincible;
    }
}
