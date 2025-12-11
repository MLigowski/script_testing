using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Jump Cooldown")]
    public float jumpCooldown = 3f;
    private float lastJumpTime = -Mathf.Infinity;

    [Header("Dash Settings")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    private bool isDashing = false;
    private bool canDash = true;

    private float horizontalInput;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isDashing) return;

        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Skok z cooldownem
        if (Input.GetButtonDown("Jump") && Time.time >= lastJumpTime + jumpCooldown)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            lastJumpTime = Time.time;
            Debug.Log("Skok! (Cooldown)");
        }

        // Dash
        // Dash
        if (Input.GetKeyDown(KeybindManager.Instance.DashKey) && canDash)
        {
            StartCoroutine(Dash());
        }

    }

    void FixedUpdate()
    {
        if (isDashing) return;

        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    System.Collections.IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;

        Health.IsInvincible = true; // ✅ Nietykalność w czasie dash'a

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        // ✅ Pobierz ID warstw (z małej litery!)
        int playerLayer = LayerMask.NameToLayer("test1");
        int enemyLayer = LayerMask.NameToLayer("Enemy");

        // ✅ Wyłącz kolizje między graczem a przeciwnikami (jeśli obie warstwy istnieją)
        if (playerLayer >= 0 && enemyLayer >= 0)
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

        // ✅ Efekt migania
        Coroutine blinkRoutine = StartCoroutine(BlinkEffect(sr));

        // ✅ Ruch dash
        rb.linearVelocity = new Vector2(transform.localScale.x * dashSpeed, 0f);
        yield return new WaitForSeconds(dashDuration);

        // ✅ Koniec dash'a
        rb.gravityScale = originalGravity;
        isDashing = false;

        // ✅ Zatrzymaj miganie i przywróć kolor
        StopCoroutine(blinkRoutine);
        sr.color = Color.white;

        Health.IsInvincible = false;

        // ✅ Przywróć kolizje po dashu
        if (playerLayer >= 0 && enemyLayer >= 0)
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);

        yield return new WaitForSeconds(0.5f);
        canDash = true;
    }


    IEnumerator BlinkEffect(SpriteRenderer sr)
    {
        while (true)
        {
            sr.color = new Color(1, 1, 1, 0.3f); // półprzezroczysty
            yield return new WaitForSeconds(0.1f);
            sr.color = new Color(1, 1, 1, 1f); // z powrotem normalny
            yield return new WaitForSeconds(0.1f);
        }
    }

}

