using UnityEngine;
using UnityEngine.SceneManagement; // For scene management
using DG.Tweening; // For DOTween animations

public class SettingsMenuController : MonoBehaviour
{
    public GameObject settingsMenuPanel; // Reference to the Settings Menu Panel
    public string homeSceneName = "HomeScene"; // Name of the home scene to load

    private bool isSettingsMenuVisible = false; // Tracks the menu visibility state
    private Vector3 initialScale; // Store the initial scale of the panel

    private void Start()
    {
        // Ensure the settings menu is initially hidden
        if (settingsMenuPanel != null)
        {
            initialScale = settingsMenuPanel.transform.localScale; // Store the initial scale
            settingsMenuPanel.SetActive(false); // Hide the panel initially
            settingsMenuPanel.transform.localScale = Vector3.zero; // Set scale to 0 for animation
        }
    }

    // Called when the Settings Button is clicked
    public void ToggleSettingsMenu()
    {
        if (settingsMenuPanel != null)
        {
            isSettingsMenuVisible = !isSettingsMenuVisible;

            if (isSettingsMenuVisible)
            {
                settingsMenuPanel.SetActive(true); // Show the panel
                settingsMenuPanel.transform.DOScale(initialScale, 0.5f) // Scale to the initial size
                    .SetEase(Ease.OutBounce); // Bounce effect
            }
            else
            {
                settingsMenuPanel.transform.DOScale(Vector3.zero, 0.5f) // Scale to zero size
                    .SetEase(Ease.InBack) // Smooth transition back
                    .OnComplete(() => settingsMenuPanel.SetActive(false)); // Deactivate panel after animation
            }
        }
    }

    // Called when the Home Button is clicked
    public void GoToHomeScene()
    {
        SceneManager.LoadScene(homeSceneName); // Load the home scene
    }
}
