using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class restartGame : MonoBehaviour
{
	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.R)){
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}	
	}
}
