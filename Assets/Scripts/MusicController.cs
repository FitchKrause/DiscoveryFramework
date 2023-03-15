using UnityEngine;
using System.Collections;

public class MusicController : AudioController
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

    private PlayerPhysics player;

    private void Start()
    {
        if (bgmSource.clip != null)
        {
            Playing = StageMusic = QueuedMusic = string.Empty;
            QueuedTime = bgmSource.time = 0f;
            bgmSource.clip = null;
        }
        player = FindObjectOfType<PlayerPhysics>();
    }

    private void FixedUpdate()
    {
        if (!player.SuperForm && player.Invincibility == 0 && !player.SpeedSneakers && player.Action != 9 && player.Action != 10 &&
            !LevelController.Boss && !LevelController.StageClear &&
            Playing != "Stage" && Playing != "1-UP" && Playing != "Drowning")
        {
            ToPlay = "Stage";
        }

        float bgmVol = (BGM_VOLUME * (MASTER_VOLUME / 100f)) / 100f;

        //Fade
        if (Fade == 0)
        {
            bgmSource.volume = bgmVol;
        }
        if (Fade >= 1)
        {
            bgmSource.volume = Mathf.Min(bgmSource.volume + (FadeSpeed / 100f), bgmVol);
        }
        if (Fade <= -1)
        {
            bgmSource.volume = Mathf.Max(bgmSource.volume - (FadeSpeed / 100f), 0f);
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
        if (FadeStop && (FadeStopCount > 100f || bgmSource.volume <= 0f))
        {
            bgmSource.Stop();
            ToPlay = SongToPlay;
            Fade = 0;
            SongToPlay = string.Empty;
            FadeStop = false;
            FadeStopCount = 0f;
        }

        //Loop
        if (LoopEnd > 0f && bgmSource.time >= (LoopEnd / 1000f))
        {
            bgmSource.time = LoopStart / 1000f;
        }

        //Playlist
        if (ToPlay == "Invincible")
        {
            Playing = ToPlay;
            bgmSource.clip = Music_Invincibility;
            LoopStart = 1420f;
            LoopEnd = 52540f;
            bgmSource.loop = true;
            bgmSource.time = QueuedTime;
            bgmSource.Play();
            ToPlay = string.Empty;
        }
        if (ToPlay == "Speed Up")
        {
            Playing = ToPlay;
            bgmSource.clip = Music_SpeedUp;
            LoopStart = 1470f;
            LoopEnd = 25030f;
            bgmSource.loop = true;
            bgmSource.time = QueuedTime;
            bgmSource.Play();
            ToPlay = string.Empty;
        }
        if (ToPlay == "1-UP")
        {
            QueuedMusic = Playing;
            QueuedTime = bgmSource.time;

            Playing = ToPlay;
            bgmSource.clip = Music_1UP;
            LifeDuration = Music_1UP.length;
            LoopStart = 0f;
            LoopEnd = 0f;
            bgmSource.loop = false;
            bgmSource.time = 0f;
            bgmSource.Play();
            ToPlay = string.Empty;
        }
        if (Playing == "1-UP" && bgmSource.time >= (LifeDuration - 0.01f))
        {
            Fade = 1;
            FadeSpeed = 2f;
            bgmSource.volume = 0f;
            ToPlay = QueuedMusic;
        }
        if (ToPlay == "Clear")
        {
            Playing = ToPlay;
            bgmSource.clip = Music_Clear;
            LoopStart = 0f;
            LoopEnd = 0f;
            bgmSource.loop = false;
            bgmSource.Play();
            ToPlay = string.Empty;
        }
        if (ToPlay == "Boss")
        {
            StageMusic = Playing = ToPlay;
            bgmSource.clip = Music_Boss;
            LoopStart = Boss_LoopStart;
            LoopEnd = Boss_LoopEnd;
            bgmSource.loop = true;
            bgmSource.time = QueuedTime;
            bgmSource.Play();
            ToPlay = string.Empty;
        }
        if (ToPlay == "Stage")
        {
            StageMusic = Playing = ToPlay;
            bgmSource.clip = Music_Stage;
            LoopStart = Stage_LoopStart;
            LoopEnd = Stage_LoopEnd;
            bgmSource.loop = true;
            bgmSource.time = QueuedTime;
            bgmSource.Play();
            ToPlay = string.Empty;
        }
        if (ToPlay == "Drowning")
        {
            QueuedMusic = Playing;
            QueuedTime = 0f;

            Playing = ToPlay;
            bgmSource.clip = Music_Drowning;
            LoopStart = 0f;
            LoopEnd = 0f;
            bgmSource.loop = false;
            bgmSource.time = 0f;
            bgmSource.Play();
            ToPlay = string.Empty;
        }
        if (ToPlay == "Super")
        {
            StageMusic = Playing = ToPlay;
            bgmSource.clip = Music_SuperForm;
            LoopStart = 23270f;
            LoopEnd = 104720f;
            bgmSource.loop = true;
            bgmSource.time = QueuedTime;
            bgmSource.Play();
            ToPlay = string.Empty;
        }
        if (ToPlay == "Game Over")
        {
            Playing = ToPlay;
            bgmSource.clip = Music_GameOver;
            LoopStart = 0f;
            LoopEnd = 0f;
            bgmSource.loop = false;
            bgmSource.time = 0f;
            bgmSource.Play();
            ToPlay = string.Empty;
        }
    }
}
