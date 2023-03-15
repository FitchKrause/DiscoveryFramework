using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GoalSign : BaseObject
{
    public string NextStage;

    public AudioClip Sound_GoalPost;
    public AudioClip Sound_MenuBip;
    public AudioClip Sound_ScoreTally;

    private bool Flag0;
    private bool Flag1;
    private bool LimitFlag1;
    private int Trigger;
    private PlayerPhysics player;

    private int CountAction;
    private int CountInternalTimer;
    public static int EventTimer;
    private int EventTimer2;

    public static int RingBonus;
    public static int TimeBonus;
    public static int TotalBonus;

    private int TimeBonusTotal;
    private int AddRingBonus;
    private int AddTimeBonus;

    private int MaxAddRingBonus;
    private int MaxAddTimeBonus;

    private int animFrame;

    private new void Start()
    {
        RingBonus = TimeBonus = TotalBonus = 0;
        EventTimer = 0;

        player = FindObjectOfType<PlayerPhysics>();

        base.Start();
    }

    private void FixedUpdate()
    {
        if (CameraController.CameraAction < 2 && player.XPosition >= XPosition && !Flag0)
        {
            Flag0 = true;
            animator.SetInteger("Character", player.Character);
            animator.SetBool("Spin!", true);
            AudioController.PlaySFX(Sound_GoalPost);
            CameraController.CameraAction = 2;
        }

        if (Flag0)
        {
            player.SuperForm = false;
            player.InvincibilityTimer = 0;
            player.SpeedSneakersTimer = 0;
            if (Trigger < 250)
            {
                Trigger++;
            }
            animFrame = (int)(24f * animator.GetCurrentAnimatorStateInfo(0).normalizedTime) % 24;
            LevelController.AllowTime = LevelController.AllowPause = false;
        }

        if (Trigger >= 250 && animFrame == 11)
        {
            if (player.Character == 0)
            {
                animator.Play("Sonic (Static)");
            }
            PlayerInput.IgnoreInput = true;
            player.AllowInput = player.AllowDirection = false;
            player.Direction = 1;
            if (player.Ground) player.GroundSpeed += 0.1f;
            else player.XSpeed += 0.1f;

            if (player.XPosition >= SceneController.XRightFrame + 16f)
            {
                if (!Flag1)
                {
                    LevelController.StageClear = Flag1 = true;
                    MusicController.ToPlay = "Clear";
                }
            }
        }

        if (CountAction == 0)
        {
            RingBonus = LevelController.CurrentLevel.Rings * 100;
            TimeBonus = TimeBonusTotal;
        }

        if (Flag1)
        {
            EventTimer++;
            if (EventTimer == 200)
            {
                CountAction = 1;
            }

            if (LevelController.CurrentLevel.Minutes == 0)
            {
                if (LevelController.CurrentLevel.Seconds <= 30)
                {
                    TimeBonusTotal = 50000;
                }
                else if (LevelController.CurrentLevel.Seconds <= 45)
                {
                    TimeBonusTotal = 10000;
                }
                else if (LevelController.CurrentLevel.Seconds <= 59)
                {
                    TimeBonusTotal = 5000;
                }
            }
            else if (LevelController.CurrentLevel.Minutes == 1)
            {
                if (LevelController.CurrentLevel.Seconds <= 30)
                {
                    TimeBonusTotal = 4000;
                }
                else if (LevelController.CurrentLevel.Seconds <= 59)
                {
                    TimeBonusTotal = 3000;
                }
            }
            else if (LevelController.CurrentLevel.Minutes == 2)
            {
                TimeBonusTotal = 2000;
            }
            else if (LevelController.CurrentLevel.Minutes == 3)
            {
                TimeBonusTotal = 1000;
            }
            else if (LevelController.CurrentLevel.Minutes == 4)
            {
                TimeBonusTotal = 500;
            }
            else if (LevelController.CurrentLevel.Minutes >= 5)
            {
                TimeBonusTotal = 0;
            }
            else if (LevelController.CurrentLevel.Minutes == 9 && LevelController.CurrentLevel.Seconds == 59)
            {
                TimeBonusTotal = 100000;
            }

            //AddRingBonus = Mathf.Clamp(AddRingBonus, 0, MaxAddRingBonus);
            //AddTimeBonus = Mathf.Clamp(AddTimeBonus, 0, MaxAddTimeBonus);

            if (CountAction == 1)
            {
                if (!LimitFlag1)
                {
                    MaxAddRingBonus = RingBonus;
                    MaxAddTimeBonus = TimeBonus;
                    LimitFlag1 = true;
                }

                if (CountInternalTimer >= 60)
                {
                    if (InputManager.instance.KeyActionAPressed)
                    {
                        GameController.Score += RingBonus + TimeBonus;
                        TotalBonus += RingBonus + TimeBonus;
                        RingBonus = 0;
                        TimeBonus = 0;
                    }

                    if (LevelController.LevelTimer % 3f == 0f)
                    {
                        AudioController.PlaySFX(Sound_MenuBip);
                    }

                    if (LevelController.LevelTimer % 1f == 0f)
                    {
                        if (TimeBonus > 0)
                        {
                            GameController.Score += 100;
                            TimeBonus -= 100;
                        }
                        if (RingBonus > 0)
                        {
                            GameController.Score += 100;
                            RingBonus -= 100;
                        }
                    }

                    AddRingBonus = (RingBonus - MaxAddRingBonus) * -1;
                    AddTimeBonus = (TimeBonus - MaxAddTimeBonus) * -1;
                    TotalBonus = AddRingBonus + AddTimeBonus;
                }

                CountInternalTimer++;

                if (RingBonus == 0 && TimeBonus == 0)
                {
                    AudioController.PlaySFX(Sound_ScoreTally);
                    CountAction = 2;
                }
            }
            if (CountAction == 2)
            {
                EventTimer2++;

                if (EventTimer2 >= 250)
                {
                    LevelController.CheckPoint = false;
                    LevelController.CheckPointX = 0f;
                    LevelController.CheckPointY = 0f;
                    LevelController.CheckPointLevelTime = 0f;
                    LevelController.CheckPointGameTime = 0f;
                    SceneManager.LoadScene(NextStage);
                }
            }
        }
    }
}
