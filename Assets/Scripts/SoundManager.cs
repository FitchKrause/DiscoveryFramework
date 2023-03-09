using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    protected static AudioSource sfxSource;
    protected static AudioSource bgmSource;

    public static float MASTER_VOLUME = 100f;

    public static float SFX_VOLUME = 50f;
    public static float BGM_VOLUME = 50f;

    private void Awake()
    {
        if (GameObject.Find("SFX Source"))
        {
            sfxSource = GameObject.Find("SFX Source").GetComponent<AudioSource>();
        }
        else
        {
            sfxSource = new GameObject("SFX Source").AddComponent<AudioSource>();
            bgmSource = new GameObject("BGM Source").AddComponent<AudioSource>();

            sfxSource.volume = (SFX_VOLUME * (MASTER_VOLUME / 100f)) / 100f;
            bgmSource.volume = (BGM_VOLUME * (MASTER_VOLUME / 100f)) / 100f;

            DontDestroyOnLoad(sfxSource.gameObject);
            DontDestroyOnLoad(bgmSource.gameObject);
        }
    }

    public static void PlaySFX(AudioClip sfx)
    {
        sfxSource.PlayOneShot(sfx);
    }

    public static void Pause()
    {
        sfxSource.Pause();
        bgmSource.Pause();
        foreach (AudioSource source in FindObjectsOfType<AudioSource>())
        {
            source.Pause();
        }
    }

    public static void Resume()
    {
        sfxSource.UnPause();
        bgmSource.UnPause();
        foreach (AudioSource source in FindObjectsOfType<AudioSource>())
        {
            source.UnPause();
        }
    }
}
