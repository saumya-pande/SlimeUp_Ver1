// Attach this to the player
using UnityEngine;
public class CollectibleManager : MonoBehaviour
{
    [Header("Collection Stats")]
    public int totalCollected = 0;
    public int totalPoints = 0;

    public void AddCollectible(int points)
    {
        totalCollected++;
        totalPoints += points;

        Debug.Log("Collected! Total: " + totalCollected + " | Points: " + totalPoints);

        // Update UI if you have one
        UpdateUI();
    }

    void UpdateUI()
    {
        // If you have a UI text element, update it here
        // Example: collectibleText.text = "Collected: " + totalCollected;
    }

    public int GetTotalCollected()
    {
        return totalCollected;
    }

    public int GetTotalPoints()
    {
        return totalPoints;
    }
}