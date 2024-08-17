using UnityEngine;

public class BackgroundBGM : MonoBehaviour
{
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        //audioSource.volume = 0;
    }
}
