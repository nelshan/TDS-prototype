using UnityEngine;

public class Sound_SFX : MonoBehaviour
{
	private void Start()
	{
		AudioManager.Instance.PlaySFX("enemy dead sfx1");
		AudioManager.Instance.PlaySFX("enemy dead sfx2");
		AudioManager.Instance.PlaySFX("enemy dead sfx3");
	}
}
