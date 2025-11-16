using UnityEngine;

public class DamageObject : MonoBehaviour
{
    public int damageAmount = 1;

    private void OnCollisionEnter(Collision collision)
    {
        PlayerHealth player = collision.collider.GetComponent<PlayerHealth>();

        if (player != null)
        {
            player.TakeDamage(damageAmount);
            Debug.Log("Damaged player for " + damageAmount + " points.");
        }
    }
}
