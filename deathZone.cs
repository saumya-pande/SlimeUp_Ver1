using UnityEngine;
using System.Collections;

public class deathZone : MonoBehaviour
{
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip respawnSound;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private bool canRespawn = true;

    private void OnTriggerEnter(Collider other)
    {
        HandlePlayerDeathTrigger(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandlePlayerDeathTrigger(collision.gameObject);
    }

    private void HandlePlayerDeathTrigger(GameObject obj)
    {
        if (!obj.CompareTag("Player")) return;

        PlayerHealth playerHealth = obj.GetComponent<PlayerHealth>();
        if (playerHealth == null) return;

        if (playerHealth.IsDead) return;  // Don't process if already dead

        // Play death audio
        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        // Deal damage
        playerHealth.TakeDamage(damageAmount);
        Debug.Log("Player entered death zone, applying damage: " + damageAmount);

        // Respawn if enabled and checkpoint assigned
        if (canRespawn && respawnPoint != null)
        {
            StartCoroutine(RespawnPlayer(obj, playerHealth));
        }
    }

    private IEnumerator RespawnPlayer(GameObject player, PlayerHealth playerHealth)
    {
        // Wait for death feedback
        yield return new WaitForSeconds(0.5f);

        // Play respawn sound
        if (respawnSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(respawnSound);
        }

        // Stop physics
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Move to checkpoint
        player.transform.position = respawnPoint.position;
        player.transform.rotation = respawnPoint.rotation;

        // Call respawn on PlayerHealth
        playerHealth.Respawn();

        Debug.Log("Player respawned at checkpoint: " + respawnPoint.name);
    }

    public void SetRespawnPoint(Transform newRespawnPoint)
    {
        respawnPoint = newRespawnPoint;
        Debug.Log("Respawn point updated to: " + (newRespawnPoint != null ? newRespawnPoint.name : "null"));
    }
}
