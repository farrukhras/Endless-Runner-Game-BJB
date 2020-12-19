using UnityEngine;
using UnityEngine.SceneManagement;

// use the screen manager to load the next scene, when the user presses the RUN button on the Splash Screen
public class LoadNextScreen : MonoBehaviour
{
    public void BeginGame() {
        // in the build settings, the seconds scene (with index 1) is the scene where player runs, so will load the scene from here
        // Only specifying the sceneName or sceneBuildIndex will load the Scene with the Single mode
        SceneManager.LoadScene(1);
    }

    public void CancelGame() {
        // in the build settings, the seconds scene (with index 0) is the scene main menu screen
        // Only specifying the sceneName or sceneBuildIndex will load the Scene with the Single mode
        SceneManager.LoadScene(0);
    }

    public void CloseGame() {
        // Debug.Log("quit");
        Application.Quit();
    }
}
