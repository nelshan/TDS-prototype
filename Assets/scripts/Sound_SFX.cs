using UnityEngine;

public class Sound_SFX : MonoBehaviour
{
    private AudioSource source;
	[SerializeField] private AudioClip[] clips;

	private void Start()
	{
		source = GetComponent<AudioSource>();
		source.clip = clips[Random.Range(0, clips.Length)];
		source.Play();
	}
}
