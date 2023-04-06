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
    public static string StageMusic;
    public static string QueuedMusic;
    public static float QueuedTime;
    public static int Fade;
    public static bool FadeStop;
    public static float FadeStopCount;
    public static float FadeSpeed = 2f;
    public static string SongToPlay;

    private float LoopStart;
    private float LoopEnd;
    private float LifeDuration;

    private void Start()
    {
        if (AudioController.audioSources[4].clip != null)
        {
            Playing = StageMusic = QueuedMusic = string.Empty;
            QueuedTime = AudioController.audioSources[4].time = 0f;
            AudioController.audioSources[4].clip = null;
        }
    }

    private void FixedUpdate()
    {
        //Fade
        if (Fade == 0)
        {
            AudioController.audioSources[4].volume = AudioController.BGM_VOLUME * AudioController.MASTER_VOLUME;
        }
        if (Fade >= 1)
        {
            AudioController.audioSources[4].volume = Mathf.Min(AudioController.audioSources[4].volume + (FadeSpeed / 100f), AudioController.BGM_VOLUME * AudioController.MASTER_VOLUME);
        }
        if (Fade <= -1)
        {
            AudioController.audioSources[4].volume = Mathf.Max(AudioController.audioSources[4].volume - (FadeSpeed / 100f), 0f);
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
        if (FadeStop && (FadeStopCount > 100f || AudioController.audioSources[4].volume <= 0f))
        {
            AudioController.audioSources[4].Stop();
            ToPlay = SongToPlay;
            Fade = 0;
            SongToPlay = string.Empty;
            FadeStop = false;
            FadeStopCount = 0f;
        }

        //Loop
        if (LoopEnd > 0f && AudioController.audioSources[4].time >= (LoopEnd / 1000f))
        {
            AudioController.audioSources[4].time = LoopStart / 1000f;
        }

        //Playlist
        if (ToPlay == "Invincible")
        {
            Playing = ToPlay;
            AudioController.audioSources[4].clip = Music_Invincibility;
            LoopStart = 1420f;
            LoopEnd = 52540f;
            AudioController.audioSources[4].loop = true;
            AudioController.audioSources[4].time = QueuedTime;
            AudioController.audioSources[4].Play();
            ToPlay = string.Empty;
        }
        if (ToPlay == "Speed Up")
        {
            Playing = ToPlay;
            AudioController.audioSources[4].clip = Music_SpeedUp;
            LoopStart = 1470f;
            LoopEnd = 25030f;
            AudioController.audioSources[4].loop = true;
            AudioController.audioSources[4].time = QueuedTime;
            AudioController.audioSources[4].Play();
            ToPlay = string.Empty;
        }
        if (ToPlay == "1-UP")
        {
            QueuedMusic = Playing;
            QueuedTime = AudioController.audioSources[4].time;

            Playing = ToPlay;
            AudioController.audioSources[4].clip = Music_1UP;
            LifeDuration = Music_1UP.length;
            LoopStart = 0f;
            LoopEnd = 0f;
            AudioController.audioSources[4].loop = false;
            AudioController.audioSources[4].time = 0f;
            AudioController.audioSources[4].Play();
            ToPlay = string.Empty;
        }
        if (Playing == "1-UP" && AudioController.audioSources[4].time >= (LifeDuration - 0.01f))
        {
            Fade = 1;
            FadeSpeed = 2f;
            AudioController.audioSources[4].volume = 0f;
            ToPlay = QueuedMusic;
        }
        if (ToPlay == "Clear")
        {
            QueuedMusic = string.Empty;
            QueuedTime = 0f;

            Playing = ToPlay;
            AudioController.audioSources[4].clip = Music_Clear;
            LoopStart = 0f;
            LoopEnd = 0f;
            AudioController.audioSources[4].loop = false;
            AudioController.audioSources[4].time = 0f;
            AudioController.audioSources[4].Play();
            ToPlay = string.Empty;
        }
        if (ToPlay == "Boss")
        {
            StageMusic = Playing = ToPlay;
            AudioController.audioSources[4].clip = Music_Boss;
            LoopStart = Boss_LoopStart;
            LoopEnd = Boss_LoopEnd;
            AudioController.audioSources[4].loop = true;
            AudioController.audioSources[4].time = QueuedTime;
            AudioController.audioSources[4].Play();
            ToPlay = string.Empty;
        }
        if (ToPlay == "Stage")
        {
            StageMusic = Playing = ToPlay;
            AudioController.audioSources[4].clip = Music_Stage;
            LoopStart = Stage_LoopStart;
            LoopEnd = Stage_LoopEnd;
            AudioController.audioSources[4].loop = true;
            AudioController.audioSources[4].time = QueuedTime;
            AudioController.audioSources[4].Play();
            ToPlay = string.Empty;
        }
        if (ToPlay == "Drowning")
        {
            QueuedMusic = Playing;
            QueuedTime = 0f;

            Playing = ToPlay;
            AudioController.audioSources[4].clip = Music_Drowning;
            LoopStart = 0f;
            LoopEnd = 0f;
            AudioController.audioSources[4].loop = false;
            AudioController.audioSources[4].time = 0f;
            AudioController.audioSources[4].Play();
            ToPlay = string.Empty;
        }
        if (ToPlay == "Super")
        {
            StageMusic = Playing = ToPlay;
            AudioController.audioSources[4].clip = Music_SuperForm;
            LoopStart = 23270f;
            LoopEnd = 104720f;
            AudioController.audioSources[4].loop = true;
            AudioController.audioSources[4].time = QueuedTime;
            AudioController.audioSources[4].Play();
            ToPlay = string.Empty;
        }
        if (ToPlay == "Game Over")
        {
            Playing = ToPlay;
            AudioController.audioSources[4].clip = Music_GameOver;
            LoopStart = 0f;
            LoopEnd = 0f;
            AudioController.audioSources[4].loop = false;
            AudioController.audioSources[4].time = 0f;
            AudioController.audioSources[4].Play();
            ToPlay = string.Empty;
        }
    }
}
