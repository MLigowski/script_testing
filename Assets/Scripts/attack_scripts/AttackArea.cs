using UnityEngine;

public class AttackArea : MonoBehaviour
{
    [Header("Attack Settings")]
    public int damage = 3;            // ile HP zabiera atak
    public float range = 1f;          // zasiêg ataku od œrodka gracza

    private void OnDrawGizmosSelected()
    {
        // Podgl¹d zasiêgu w editorze
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Health health = collider.GetComponent<Health>();
        if (health != null)
        {
            health.Damage(damage);
        }

        var slime = collider.GetComponent<Slime>();
        if (slime != null)
            slime.TakeDamage(damage);

        var zombie = collider.GetComponent<Zombie>();
        if (zombie != null)
            zombie.TakeDamage(damage);
    }
}
