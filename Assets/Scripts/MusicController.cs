using UnityEngine;
using System.Collections;

public class MusicController : MonoBehaviour
{
    public AudioClip Music_Invincibility;
    public AudioClip Music_SpeedUp;
    public AudioClip Music_1UP;
    public AudioClip Music_Clear;
    public AudioClip Music_Boss;
    public AudioClip Music_Stage;
    public AudioClip Music_Drowning;
    public AudioClip Music_SuperForm;
    public AudioClip Music_GameOver;

    public float Boss_LoopStart;
    public float Boss_LoopEnd;
    public float Stage_LoopStart;
    public float Stage_LoopEnd;

    public static string ToPlay;
    public static string Playing;
    public static string QueuedPlaying;
    public static float QueuedTime;
    public static int Fade;
    public static bool FadeStop;
    public static float FadeStopCount;
    public static float FadeSpeed = 2f;
    public static string SongToPlay;

    private float LoopStart;
    private float LoopEnd;
    private float LifeDuration;

    private float BGM_VOLUME;
    private AudioSource musicSource;

    private void Start()
    {
        musicSource = SoundManager.bgmSource;
        BGM_VOLUME = (SoundManager.BGM_VOLUME * (SoundManager.MASTER_VOLUME / 100f)) / 100f;

        ToPlay = "Stage";
        musicSource.time = 0f;
    }

    private void FixedUpdate()
    {
        //Fade
        if (Fade == 0)
        {
            musicSource.volume = BGM_VOLUME;
        }
        if (Fade >= 1)
        {
            musicSource.volume = Mathf.Min(musicSource.volume + (FadeSpeed / 100f), BGM_VOLUME);
        }
        if (Fade <= -1)
        {
            musicSource.volume = Mathf.Max(musicSource.volume - (FadeSpeed / 100f), 0f);
        }

        if (FadeStop && SongToPlay != Playing)
        {
            Fade = 1;
            FadeStopCount += FadeSpeed;
        }
        if (SongToPlay == Playing)
        {
            Fade = 0;
            SongToPlay = string.Empty;
            FadeStop = false;
            FadeStopCount = 0f;
        }
        if (FadeStop && (FadeStopCount > 100f || musicSource.volume <= 0f))
        {
            musicSource.Stop();
            ToPlay = SongToPlay;
            Fade = 0;
            SongToPlay = string.Empty;
            FadeStop = false;
            FadeStopCount = 0f;
        }

        //Loop
        if (LoopEnd > 0f && musicSource.time >= (LoopEnd / 1000f))
        {
            musicSource.time = LoopStart / 1000f;
        }

        //Playlist
        if (ToPlay == "Invincible")
        {
            Playing = ToPlay;
            musicSource.clip = Music_Invincibility;
            LoopStart = 1420f;
            LoopEnd = 52540f;
            musicSource.loop = true;
            musicSource.time = QueuedTime;
            musicSource.Play();
            ToPlay = string.Empty;
        }
        if (ToPlay == "Speed Up")
        {
            Playing = ToPlay;
            musicSource.clip = Music_SpeedUp;
            LoopStart = 1470f;
            LoopEnd = 25030f;
            musicSource.loop = true;
            musicSource.time = QueuedTime;
            musicSource.Play();
            ToPlay = string.Empty;
        }
        if (ToPlay == "1-UP")
        {
            QueuedPlaying = Playing;
            QueuedTime = musicSource.time;

            Playing = ToPlay;
            musicSource.clip = Music_1UP;
            LifeDuration = Music_1UP.length;
            LoopStart = 0f;
            LoopEnd = 0f;
            musicSource.loop = false;
            musicSource.time = 0f;
            musicSource.Play();
            ToPlay = string.Empty;
        }
        if (Playing == "1-UP" && musicSource.time >= (LifeDuration - 0.01f))
        {
            Fade = 1;
            FadeSpeed = 2f;
            musicSource.volume = 0f;
            ToPlay = QueuedPlaying;
        }
        if (ToPlay == "Clear")
        {
            Playing = ToPlay;
            musicSource.clip = Music_Clear;
            LoopStart = 0f;
            LoopEnd = 0f;
            musicSource.loop = false;
            musicSource.Play();
            ToPlay = string.Empty;
        }
        if (ToPlay == "Boss")
        {
            Playing = ToPlay;
            musicSource.clip = Music_Boss;
            LoopStart = Boss_LoopStart;
            LoopEnd = Boss_LoopEnd;
            musicSource.loop = true;
            musicSource.time = QueuedTime;
            musicSource.Play();
            ToPlay = string.Empty;
        }
        if (ToPlay == "Stage")
        {
            Playing = ToPlay;
            musicSource.clip = Music_Stage;
            LoopStart = Stage_LoopStart;
            LoopEnd = Stage_LoopEnd;
            musicSource.loop = true;
            musicSource.time = QueuedTime;
            musicSource.Play();
            ToPlay = string.Empty;
        }
        if (ToPlay == "Drowning")
        {
            QueuedPlaying = Playing;
            QueuedTime = 0f;

            Playing = ToPlay;
            musicSource.clip = Music_Drowning;
            LoopStart = 0f;
            LoopEnd = 0f;
            musicSource.loop = false;
            musicSource.time = 0f;
            musicSource.Play();
            ToPlay = string.Empty;
        }
        if (ToPlay == "Super")
        {
            Playing = ToPlay;
            musicSource.clip = Music_SuperForm;
            LoopStart = 23270f;
            LoopEnd = 104720f;
            musicSource.loop = true;
            musicSource.time = QueuedTime;
            musicSource.Play();
            ToPlay = string.Empty;
        }
        if (ToPlay == "Game Over")
        {
            Playing = ToPlay;
            musicSource.clip = Music_GameOver;
            LoopStart = 0f;
            LoopEnd = 0f;
            musicSource.loop = false;
            musicSource.time = 0f;
            musicSource.Play();
            ToPlay = string.Empty;
        }
    }
}
