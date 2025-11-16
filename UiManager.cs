using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
public class UiManager : MonoBehaviour
{
    [SerializeField] private GameObject[] hearts;
    [SerializeField] private GameObject LoseMessage;
    [SerializeField] private Image image;

    private Coroutine damageCoroutine; // track running coroutine

    public void InitializeHearts(int maxLives)
    {
        // Validate array size
        if (hearts == null || hearts.Length < maxLives)
        {
            Debug.LogError("Hearts array is null or too small!");
            return;
        }

        // Show initial hearts
        for (int i = 0; i < maxLives; i++)
        {
            hearts[i].SetActive(true);
        }

        // Hide extra hearts
        for (int i = maxLives; i < hearts.Length; i++)
        {
            hearts[i].SetActive(false);
        }
    }

    // Call this when the player takes damage
    public void TakeDamageDisplay()
    {
        if (image == null) return;
        if (damageCoroutine != null) StopCoroutine(damageCoroutine);
        damageCoroutine = StartCoroutine(ShowDamageDisplay());
    }

    private IEnumerator ShowDamageDisplay()
    {

        // Set alpha to 0.5
        Color c = image.color;
        c.a = 0.5f;
        image.color = c;

        // Keep it for 1 second
        yield return new WaitForSeconds(0.3f);

        // Reset alpha to 0
        c = image.color;
        c.a = 0f;
        image.color = c;

        damageCoroutine = null;
    }

    public void UpdateHealthDisplay(int currentLives)
    {
        if (hearts == null) return;

        // Show/hide hearts based on current lives
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].SetActive(i < currentLives);
        }
    }
    
    public void ShowLoseMessage()
    {
        Time.timeScale = 0f; // Pause the game
        if (LoseMessage != null)
        {
            LoseMessage.SetActive(true);
        }
        
        // Hide all hearts
        if (hearts != null)
        {
            foreach (var heart in hearts)
            {
                if (heart != null) heart.SetActive(false);
            }
        }
    }

    public void HideLoseMessage()
    {
        Time.timeScale = 1f; // Resume the game
        if (LoseMessage != null)
        {
            LoseMessage.SetActive(false);
        }
    }
}
