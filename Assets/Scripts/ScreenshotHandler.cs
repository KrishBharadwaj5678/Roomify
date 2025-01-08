using System.Collections;
using UnityEngine;
using DG.Tweening;  // Make sure DOTween is imported
using System;

public class ScreenshotHandler : MonoBehaviour
{
    public GameObject sendPanel; // Reference to the send panel GameObject
    public string screenshotBaseName = "Roomify"; // Base screenshot file name
    public float fadeDuration = 1f; // Duration of the fade-in and fade-out
    public float displayDuration = 3f; // Duration to keep the panel visible before fading out

    private CanvasGroup canvasGroup; // Reference to CanvasGroup for controlling the alpha

    private void Start()
    {
        // Ensure the send panel is initially hidden
        if (sendPanel != null)
        {
            sendPanel.SetActive(false); // Hide panel initially
            canvasGroup = sendPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = sendPanel.AddComponent<CanvasGroup>(); // Add CanvasGroup if not present
            }
            canvasGroup.alpha = 0f; // Set the alpha to 0 for initial invisibility
        }
    }

    // Call this function when the screenshot button is clicked
    public void CaptureScreenshot()
    {
        StartCoroutine(CaptureScreenshotCoroutine());
    }

    private IEnumerator CaptureScreenshotCoroutine()
    {
        // Wait for the end of the frame to ensure the UI is rendered
        yield return new WaitForEndOfFrame();

        // Generate a unique file name with timestamp to ensure no overwrite
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff");
        string screenshotFileName = $"{screenshotBaseName}_{timestamp}.png";

        // Define the file path for the screenshot
        string screenshotPath = System.IO.Path.Combine(Application.persistentDataPath, screenshotFileName);

        // Capture the screenshot and save it to the defined path
        ScreenCapture.CaptureScreenshot(screenshotFileName);
        Debug.Log($"Screenshot saved at: {screenshotPath}");

        // Wait briefly to ensure the file is saved
        yield return new WaitForSeconds(0.5f);

        // Fade-in the send panel
        if (sendPanel != null)
        {
            sendPanel.SetActive(true); // Make the panel active
            canvasGroup.DOFade(1f, fadeDuration); // Fade in
        }

        // Wait for the specified display duration before fading out
        yield return new WaitForSeconds(displayDuration);

        // Fade-out the send panel
        if (sendPanel != null)
        {
            canvasGroup.DOFade(0f, fadeDuration).OnKill(() => sendPanel.SetActive(false)); // Fade out and deactivate afterward
        }
    }
}