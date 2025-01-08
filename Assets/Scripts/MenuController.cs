using UnityEngine;
using UnityEngine.SceneManagement; // For scene management

public class MenuController : MonoBehaviour
{
    public string mainSceneName = "MainScene"; // Name of the main scene to load

    // Called when the Play button is clicked
    public void PlayGame()
    {
        SceneManager.LoadScene(mainSceneName); // Load the main scene
    }

    // Called when the Exit button is clicked
    public void ExitGame()
    {
        Debug.Log("Exiting the game..."); // Debug log for confirmation
        Application.Quit(); // Quit the application
    }
}