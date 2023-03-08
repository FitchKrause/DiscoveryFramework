using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    private static AudioSource audioSource;

    public static float VOLUME = 50f;

    private void Start()
    {
        if (GameObject.Find("Audio Source"))
        {
            audioSource = GameObject.Find("Audio Source").GetComponent<AudioSource>();
        }
        else
        {
            audioSource = new GameObject("Audio Source").AddComponent<AudioSource>();
            audioSource.volume = VOLUME / 100f;
            DontDestroyOnLoad(audioSource.gameObject);
        }
    }

    public static void PlaySFX(AudioClip sfx)
    {
        audioSource.PlayOneShot(sfx);
    }

    public static void Pause()
    {
        audioSource.Pause();
    }

    public static void Resume()
    {
        audioSource.UnPause();
    }
}
