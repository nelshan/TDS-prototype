using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class restartGame : MonoBehaviour
{
    public TextMeshProUGUI scoreDisplayFromPlayerScore;
	private player_controller player;

	private void Start()
	{
		player = FindObjectOfType<player_controller>();
	}

	private void Update()
	{

		scoreDisplayFromPlayerScore.text = player.score.ToString();

		if(Input.GetKeyDown(KeyCode.R)){
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
		
	}
}
