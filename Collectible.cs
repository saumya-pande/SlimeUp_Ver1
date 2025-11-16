using UnityEngine;

// Attach this to each collectible item
public class Collectible : MonoBehaviour
{
    [Header("Collectible Settings")]
    public int pointValue = 10;
    public bool rotateItem = true;
    public float rotationSpeed = 50f;
    
    [Header("Effects")]
    public AudioClip collectSound;
    public GameObject collectEffect; // Optional particle effect
    
    void Update()
    {
        // Rotate collectible for visual effect
        if (rotateItem)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Check if player collected it
        if (other.CompareTag("Player"))
        {
            Collect(other.gameObject);
        }
    }
    
    void Collect(GameObject player)
    {
        // Add to player's collection
        CollectibleManager manager = player.GetComponent<CollectibleManager>();
        if (manager != null)
        {
            manager.AddCollectible(pointValue);
        }
        
        // Play sound effect
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }
        
        // Spawn particle effect
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }
        
        // Destroy the collectible
        Destroy(gameObject);
    }
}