using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GoalSign : BaseObject
{
    public string NextStage;

    public AudioClip Sound_GoalPost;
    public AudioClip Sound_MenuBip;
    public AudioClip Sound_ScoreTally;

    public Sprite[] GUI_CharacterNames;
    private SpriteRenderer GUI_CharacterGot;

    private bool Flag0;
    private bool Flag1;
    private bool LimitFlag1;
    private int Trigger;

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
        RingBonus = TimeBonus = TotalBonus = 0;
        EventTimer = 0;
        CameraController.CameraAction = 0;
        CameraController.CameraMinimumX = 0f;
        CameraController.CameraMinimumY = 0f;
        CameraController.CameraMaximumX = LevelController.CurrentLevel.Width;
        CameraController.CameraMaximumY = LevelController.CurrentLevel.Height;

        GUI_CharacterGot = GameObject.Find("GUI_CharacterName").GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        PlayerPhysics player = SceneController.FindStageObject("PlayerPhysics") as PlayerPhysics;

        LevelController.Clear = Flag1;

        switch (GameController.GameCharacter)
        {
            case 0:
                charName = "Sonic";
                break;
            case 1:
                charName = "Tails";
                break;
            default:
                charName = "Sonic";
                break;
        }

        GUI_CharacterGot.sprite = GUI_CharacterNames[GameController.GameCharacter];

        if (CameraController.CameraAction != 2 && player.XPosition >= XPosition && !Flag0)
        {
            Flag0 = true;
            animator.SetInteger("Character", GameController.GameCharacter);
            animator.SetBool("Spin!", true);
            AudioController.PlaySFX(Sound_GoalPost);
            CameraController.CameraAction = 2;
            GameController.SetGameSpeed(1f);
        }

        if (Flag0)
        {
            player.SuperForm = false;
            player.InvincibilityTimer = 0;
            player.SpeedSneakersTimer = 0;
            if (Trigger < 250)
            {
                Trigger += GameController.Frame;
            }
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(charName))
            {
                animFrame = (int)(24f * animator.GetCurrentAnimatorStateInfo(0).normalizedTime) % 24;
            }
            LevelController.AllowTime = LevelController.AllowPause = false;
        }

        if (Trigger >= 250 && animFrame == 11 && !Flag1)
        {
            animator.Play(charName + " (Static)");
            animator.SetBool("Spin!", false);
            if (!PlayerInput.OverrideInput)
            {
                PlayerInput.ClearInput = true;
                PlayerInput.OverrideInput = true;
            }
            PlayerInput.KeyRight = true;
            if (player.XPosition >= GameController.XRightFrame + 16f)
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
            EventTimer += GameController.Frame;
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
                    if (InputManager.KeyActionAPressed)
                    {
                        GameController.Score += RingBonus + TimeBonus;
                        TotalBonus += RingBonus + TimeBonus;
                        RingBonus = 0;
                        TimeBonus = 0;
                    }

                    if (LevelController.LevelTimer % 3f == 0f && GameController.Frame == 1)
                    {
                        AudioController.PlaySFX(Sound_MenuBip);
                    }

                    if (LevelController.LevelTimer % 1f == 0f && GameController.Frame == 1)
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

                CountInternalTimer += GameController.Frame;

                if (RingBonus == 0 && TimeBonus == 0)
                {
                    AudioController.PlaySFX(Sound_ScoreTally);
                    CountAction = 2;
                }
            }
            if (CountAction == 2)
            {
                EventTimer2 += GameController.Frame;

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
