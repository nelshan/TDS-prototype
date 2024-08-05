using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   public void PlayGame()
   {
        SceneManager.LoadScene("Boxey TDS");
        Time.timeScale = 1f;
        PushMenu.GameIsPused = false;
   }

   public void QuitGame()
   {
        Application.Quit();
   }
}
