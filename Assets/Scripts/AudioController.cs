using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour
{
    public static AudioSource[] audioSources;

    [Range(0, 100)]
    public int MasterVolume, SFXVolume, BGMVolume;

    public static float MASTER_VOLUME = 100f;

    public static float SFX_VOLUME = 50f;
    public static float BGM_VOLUME = 50f;

    private void Awake()
    {
        MASTER_VOLUME = MasterVolume / 100f;
        SFX_VOLUME = SFXVolume / 100f;
        BGM_VOLUME = BGMVolume / 100f;

        audioSources = new AudioSource[5];

        for (int i = 0; i < audioSources.Length; i++)
        {
            if (GameObject.Find(string.Format("AudioSource {0}", i)))
            {
                audioSources[i] = GameObject.Find(string.Format("AudioSource {0}", i)).GetComponent<AudioSource>();
            }
            else
            {
                audioSources[i] = new GameObject(string.Format("AudioSource {0}", i)).AddComponent<AudioSource>();

                audioSources[i].volume = SFX_VOLUME * MASTER_VOLUME;

                DontDestroyOnLoad(audioSources[i].gameObject);
            }
        }

        audioSources[4].volume = BGM_VOLUME * MASTER_VOLUME;
    }

    public static void PlaySFX(AudioClip sfx)
    {
        audioSources[0].PlayOneShot(sfx);
    }

    public static void Pause()
    {
        foreach (AudioSource source in FindObjectsOfType<AudioSource>())
        {
            source.Pause();
        }
    }

    public static void Resume()
    {
        foreach (AudioSource source in FindObjectsOfType<AudioSource>())
        {
            source.UnPause();
        }
    }
}
