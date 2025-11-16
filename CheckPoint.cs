
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private deathZone deathZone;
    [SerializeField] private GameObject checkpointEffect;
    
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player reached checkpoint: " + gameObject.name);
            // Activate checkpoint effect
            if (checkpointEffect != null)
            {
                checkpointEffect.SetActive(true);
            }
            
            // Update the death zone's respawn point
            if (deathZone != null)
            {
                deathZone.SetRespawnPoint(transform);
                Debug.Log("Checkpoint reached: " + gameObject.name);
            }
            else
            {
                Debug.LogWarning("DeathZone reference missing on checkpoint!");
            }
        }
    }
}