using UnityEngine;
using UnityEngine.SceneManagement;

public class PushMenu : MonoBehaviour
{
    public static bool GameIsPused = false;
    public GameObject puseMenuUI;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPused)
            {
                Resume(); // if game is alreany pused and player pressed Escape key again game will resume
            }
            else
            {
                Pused(); // if game is not pused and player pressed Escape key game will puse
            }
        }
    }
   
    void Pused()
    {
        puseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPused = true;
    }

    public void Resume()
    {
        puseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPused = false;
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        GameIsPused = false;
        SceneManager.LoadScene("Main Menu");
    }

    public void QuitGame()
    {
        Debug.Log("QuitGame");
        Application.Quit();
    }

    public void PlayAgain()
    {
        Time.timeScale = 1f;
        GameIsPused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
