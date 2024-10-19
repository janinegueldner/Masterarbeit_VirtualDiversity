using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartSceneButton : MonoBehaviour
{
    // This method will be called when the button is clicked
    public void RestartScene()
    {
        // Get the current active scene and restart it
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
