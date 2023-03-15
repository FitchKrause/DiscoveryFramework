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
    private string charName;

    private void Awake()
    {
        RingBonus = TimeBonus = TotalBonus = EventTimer = 0;
        CameraController.CameraAction = 0;
        CameraController.CameraMinimumX = 0f;
        CameraController.CameraMinimumY = 0f;
        CameraController.CameraMaximumX = LevelController.CurrentLevel.Width;
        CameraController.CameraMaximumY = LevelController.CurrentLevel.Height;
    }

    private new void Start()
    {
        player = FindObjectOfType<PlayerPhysics>();

        base.Start();
    }

    private void FixedUpdate()
    {
        LevelController.Clear = Flag1;

        switch (player.Character)
        {
            case 0:
                charName = "Sonic";
                break;
            default:
                charName = "Sonic";
                break;
        }

        if (CameraController.CameraAction != 2 && player.XPosition >= XPosition && !Flag0)
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
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(charName))
            {
                animFrame = (int)(24f * animator.GetCurrentAnimatorStateInfo(0).normalizedTime) % 24;
            }
            LevelController.AllowTime = LevelController.AllowPause = false;
        }

        if (Trigger >= 250 && animFrame == 11 && !Flag1)
        {
            if (player.Character == 0)
            {
                animator.Play("Sonic (Static)");
                animator.SetBool("Spin!", false);
            }
            PlayerInput.IgnoreInput = true;
            PlayerInput.OverrideInput = true;
            PlayerInput.ClearInput = true;
            PlayerInput.KeyRight = true;
            if (player.XPosition >= SceneController.XRightFrame + 16f)
            {
                Flag1 = true;
                MusicController.ToPlay = "Clear";
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
