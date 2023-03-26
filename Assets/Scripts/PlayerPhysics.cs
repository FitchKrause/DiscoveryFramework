using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerPhysics : BaseObject
{
    #region Player Values
    public enum RotationMethods
    {
        Full,
        Partial,
        Legacy,
        Sonic1
    }
    public RotationMethods RotationMethod;
    public float RotationSnap = 45f;

    [HideInInspector] public int Action;
    [HideInInspector] public int Hurt;
    [HideInInspector] public int CeilingLand;
    [HideInInspector] public float SmoothAngle;

    [HideInInspector] public bool Attacking;
    [HideInInspector] public bool JumpVariable;
    [HideInInspector] public float SpindashRev;
    [HideInInspector] public int ShieldStatus;

    [HideInInspector] public int ControlLockTimer;
    [HideInInspector] public int SkidTimer;
    [HideInInspector] public int InvincibilityTimer;
    [HideInInspector] public int SpeedSneakersTimer;
    [HideInInspector] public int Air;

    [HideInInspector] public int Character;
    [HideInInspector] public bool SuperForm;
    [HideInInspector] public bool SuperFlag;
    [HideInInspector] public int Shield;
    [HideInInspector] public int Invincibility;
    [HideInInspector] public bool SpeedSneakers;
    [HideInInspector] public bool Underwater;

    [HideInInspector] public bool AllowInput;
    [HideInInspector] public bool AllowDirection;
    #endregion
    #region Player Sounds
    [Header("Player Sound Effects")]
    public AudioClip Sound_Jump;
    public AudioClip Sound_Skidding;
    public AudioClip Sound_Rolling;
    public AudioClip Sound_Release;
    public AudioClip Sound_Hurt;
    public AudioClip Sound_Drown;
    public AudioClip Sound_LoseRings;
    public AudioClip Sound_FireDash;
    public AudioClip Sound_LightingJump;
    public AudioClip Sound_BubbleBounce;
    public AudioClip Sound_Splash;
    #endregion
    #region Player Constants
    [Header("Player Constants")]
    public float Acceleration = 0.046875f;
    public float AirAcceleration = 0.09375f;
    public float Friction = 0.046875f;
    public float Deceleration = 0.5f;
    public float TopSpeed = 6f;
    public float GravityForce = 0.21875f;
    public float JumpForce = 6.78125f;
    public float JumpReleaseForce = 4f;
    public float AirDrag = 0.96875f;
    public float RollFriction = 0.0234375f;
    public float RollDeceleration = 0.125f;
    public float SlopeFactor = 0.125f;
    public float SlopeRollUpFactor = 0.078125f;
    public float SlopeRollDownFactor = 0.3125f;
    public float MaxXSpeed = 20f;
    public float MaxYSpeed = 20f;
    #endregion
    #region Player Components
    public ObjectState PlayerAction;
    public HitBox Rect;
    [Header("Player Components")]
    public GameObject[] Skins;
    public GameObject[] SuperSkins;
    public GameObject SpindashDust;
    private AudioSource audioSource;
    private Shield[] Shields;
    #endregion
    #region Player Initialization
    private new void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = (AudioController.SFX_VOLUME * (AudioController.MASTER_VOLUME / 100f)) / 100f;
        Shields = FindObjectsOfType<Shield>();

        base.Start();

        PlayerAction = Action00_Common;

        if (!LevelController.CheckPoint)
        {
            XPosition = transform.position.x;
            YPosition = transform.position.y;
            LevelController.LevelTimer = 0f;
            LevelController.CurrentLevel.GameTimer = 0f;
        }
        else
        {
            XPosition = LevelController.CheckPointX;
            YPosition = LevelController.CheckPointY;
            LevelController.LevelTimer = LevelController.CheckPointLevelTime;
            LevelController.CurrentLevel.GameTimer = LevelController.CheckPointGameTime;
        }

        render.enabled = false;

        for (int i = 0; i < Skins.Length; i++)
        {
            Skins[i].transform.SetParent(transform);
            Skins[i].transform.position = transform.position;
            Skins[i].GetComponent<SpriteRenderer>().sortingOrder = 0;
            Skins[i].SetActive(i == Character && !SuperForm);
        }
        for (int i = 0; i < SuperSkins.Length; i++)
        {
            SuperSkins[i].transform.SetParent(transform);
            SuperSkins[i].transform.position = transform.position;
            SuperSkins[i].GetComponent<SpriteRenderer>().sortingOrder = 0;
            SuperSkins[i].SetActive(i == Character && SuperForm);
        }

        if (SuperForm)
        {
            animator = SuperSkins[Character].GetComponent<Animator>();
            render = SuperSkins[Character].GetComponent<SpriteRenderer>();
        }
        else
        {
            animator = Skins[Character].GetComponent<Animator>();
            render = Skins[Character].GetComponent<SpriteRenderer>();
        }

        if (YPosition < GameObject.Find("Water Mark").transform.position.y)
        {
            Underwater = true;
        }

        AllowInput = AllowDirection = true;
    }
    #endregion
    #region Player Update
    private void FixedUpdate()
    {
        #region Player Input
        int inpDir = (PlayerInput.KeyRight ? 1 : 0) - (PlayerInput.KeyLeft ? 1 : 0);
        #endregion
        #region Player Physics
        Acceleration = 0.046875f;
        AirAcceleration = 0.09375f;
        Friction = 0.046875f;
        Deceleration = 0.5f;
        TopSpeed = 6f;
        GravityForce = 0.21875f;
        JumpForce = 6.78125f;
        JumpReleaseForce = 4f;
        AirDrag = 0.96875f;
        RollFriction = 0.0234375f;
        RollDeceleration = 0.125f;
        SlopeFactor = 0.125f;
        SlopeRollUpFactor = 0.078125f;
        SlopeRollDownFactor = 0.3125f;
        MaxXSpeed = 20f;
        MaxYSpeed = 20f;

        if (SpeedSneakers)
        {
            Acceleration *= 2f;
            Friction *= 2f;
            TopSpeed *= 2f;
            AirAcceleration *= 2f;
            RollFriction *= 2f;
        }
        if (SuperForm)
        {
            Acceleration *= 2f;
            Deceleration *= 2f;
            Friction /= 2f;
            TopSpeed -= 2f;
            AirAcceleration *= 2f;
            JumpForce += 1.5f;
            RollFriction /= 2f;
        }

        if (Underwater)
        {
            Acceleration /= 2f;
            Deceleration /= 2f;
            Friction /= 2f;
            TopSpeed /= 2f;
            AirAcceleration /= 2f;
            RollFriction /= 2f;
            GravityForce /= 3.5f;
            SlopeFactor /= 2f;
            JumpForce -= 3f;
            JumpReleaseForce /= 2f;
        }
        #endregion
        #region Player Control (Pre)
        #region Control (Pre)
        if (Action != 9 && Action != 10)
        {
            #region Slope Factor
            if (AllowInput && Ground && Action != 6)
            {
                GroundSpeed -= SlopeFactor * Mathf.Sin(GroundAngle * Mathf.Deg2Rad) * Time.timeScale;
            }
            #endregion
            #region X Control
            if (AllowInput && !(Ground && ControlLockTimer > 0f || Action == 6))
            {
                if (inpDir < 0)
                {
                    if (Ground)
                    {
                        if (GroundSpeed > 0f)
                        {
                            GroundSpeed -= Deceleration * Time.timeScale;

                            if (GroundSpeed <= 0f)
                            {
                                GroundSpeed = -0.5f;
                            }
                        }
                        else if (GroundSpeed > -TopSpeed)
                        {
                            GroundSpeed -= Acceleration * Time.timeScale;

                            if (GroundSpeed <= -TopSpeed)
                            {
                                GroundSpeed = -TopSpeed;
                            }
                        }
                    }
                    else if (XSpeed > -TopSpeed)
                    {
                        XSpeed -= AirAcceleration * Time.timeScale;

                        if (XSpeed <= -TopSpeed)
                        {
                            XSpeed = -TopSpeed;
                        }
                    }
                }
                
                if (inpDir > 0)
                {
                    if (Ground)
                    {
                        if (GroundSpeed < 0f)
                        {
                            GroundSpeed += Deceleration * Time.timeScale;

                            if (GroundSpeed >= 0f)
                            {
                                GroundSpeed = 0.5f;
                            }
                        }
                        else if (GroundSpeed < TopSpeed)
                        {
                            GroundSpeed += Acceleration * Time.timeScale;

                            if (GroundSpeed >= TopSpeed)
                            {
                                GroundSpeed = TopSpeed;
                            }
                        }
                    }
                    else if (XSpeed < TopSpeed)
                    {
                        XSpeed += AirAcceleration * Time.timeScale;

                        if (XSpeed >= TopSpeed)
                        {
                            XSpeed = TopSpeed;
                        }
                    }
                }
                
                if (Ground && inpDir == 0)
                {
                    GroundSpeed -= Mathf.Min(Mathf.Abs(GroundSpeed), Friction) * Mathf.Sign(GroundSpeed) * Time.timeScale;
                    
                    if (Mathf.Abs(GroundSpeed) < Deceleration)
                    {
                        GroundSpeed = 0f;
                    }
                }
            }
            #endregion
            #region Control Lock
            if (Ground)
            {
                if (ControlLockTimer == 0)
                {
                    if (Mathf.Abs(GroundSpeed) < 2.5f && GroundAngle > 35f && GroundAngle < 325f)
                    {
                        ControlLockTimer = 30;

                        if (GroundAngle >= 75f && GroundAngle <= 285f)
                        {
                            XSpeed = GroundSpeed * Mathf.Cos(GroundAngle * Mathf.Deg2Rad);
                            YSpeed = GroundSpeed * Mathf.Sin(GroundAngle * Mathf.Deg2Rad);
                            GroundAngle = 0f;
                            Ground = false;
                            Fell = true;
                        }
                        else
                        {
                            if (GroundAngle < 180f)
                            {
                                GroundSpeed -= 0.5f;
                            }
                            else
                            {
                                GroundSpeed += 0.5f;
                            }
                        }
                    }
                }
                else
                {
                    ControlLockTimer -= GameController.Frame;
                }
            }
            #endregion
        }
        #endregion
        #endregion
        #region Player Movement
        GroundSpeed = Mathf.Clamp(GroundSpeed, -MaxXSpeed, MaxXSpeed);
        XSpeed = Mathf.Clamp(XSpeed, -MaxXSpeed, MaxXSpeed);
        YSpeed = Mathf.Clamp(YSpeed, -MaxYSpeed, MaxYSpeed);

        if (Landed)
        {
            JumpVariable = false;
        }

        ProcessMovement();

        if (PlayerAction != null)
        {
            Attacking = false;
            PlayerAction();
        }

        Rect.XPosition = XPosition + XSpeed;
        Rect.YPosition = YPosition + YSpeed;
        Rect.WidthRadius = WidthRadius;
        Rect.HeightRadius = HeightRadius;

        if (ColliderCeiling && CeilingLand == 0 && Fell == false)
        {
            CeilingLand = 1;
            GroundAngle = 180f;
            SmoothAngle = 180f;
            SensorHit floorLeft = SensorCast(new Vector2(-WidthRadius, 0f), Vector2.down, HeightRadius, true, 12f);
            SensorHit floorRight = SensorCast(new Vector2(WidthRadius, 0f), Vector2.down, HeightRadius, true, 12f);
            if (CeilingLand == 1 && (floorLeft.Collision || floorRight.Collision))
            {
                CeilingLand = 2;
                DetectAngle(true);
                if (GroundAngle >= 170f && GroundAngle <= 190f)
                {
                    GroundAngle = 0f;
                    SmoothAngle = 0f;
                }
                else
                {
                    CeilingLand = 3;
                    XSpeed += YSpeed * Mathf.Sin(GroundAngle * Mathf.Deg2Rad);
                    if (CeilingLand == 3)
                    {
                        CeilingLand = 0;
                        GroundSpeed = XSpeed;
                        LandingSpeed = YSpeed;
                        Ground = true;
                        Landed = true;
                        PlayerAction = Action01_Jump;
                    }
                }
            }
        }

        if (CeilingLand > 0 && CeilingLand <= 3 && !Ground)
        {
            CeilingLand = 0;
            GroundAngle = 0f;
            SmoothAngle = 0f;
        }
        #endregion
        #region Player Control
        #region Control
        #region Y Control
        if (!Ground)
        {
            YSpeed -= GravityForce * Time.timeScale;
            
            if (YSpeed > 0f && YSpeed < 4f && Mathf.Abs(XSpeed) >= 0.125f)
            {
                XSpeed -= (XSpeed - (XSpeed * AirDrag)) * Time.timeScale;
            }
        }
        #endregion
        #region X Control (Post)
        if ((Ground ? GroundSpeed : XSpeed) <= 0f && XPosition <= CameraController.CameraMinimumX + 16f)
        {
            if (Ground) GroundSpeed = 0f;
            else XSpeed = 0f;
            XPosition = CameraController.CameraMinimumX + 16f;
        }

        if ((Ground ? GroundSpeed : XSpeed) >= 0f && XPosition >= CameraController.CameraMaximumX + (CameraController.CameraAction != 2 ? -16f : 32f))
        {
            if (Ground) GroundSpeed = 0f;
            else XSpeed = 0f;
            XPosition = CameraController.CameraMaximumX + (CameraController.CameraAction != 2 ? -16f : 32f);
        }
        #endregion
        #endregion
        #region Invincibility and Speed Sneakers
        if (Action != 9 && Action != 10)
        {
            if (SuperForm)
            {
                Invincibility = 1;
                SpeedSneakers = true;
                if (LevelController.LevelTimer % 60f == 0f && GameController.Frame == 1 && LevelController.CurrentLevel.Rings > 0)
                {
                    LevelController.CurrentLevel.Rings--;
                }
                else if (LevelController.CurrentLevel.Rings <= 0)
                {
                    SuperForm = false;
                    MusicController.QueuedTime = 0f;
                    MusicController.ToPlay = "Stage";
                }
            }
            else
            {
                if (Invincibility > 0 && InvincibilityTimer > 0f)
                {
                    InvincibilityTimer -= GameController.Frame;
                }
                if (Invincibility > 0 && InvincibilityTimer <= 0f)
                {
                    if (Invincibility == 1)
                    {
                        MusicController.ToPlay = MusicController.StageMusic;
                    }
                    InvincibilityTimer = 0;
                    Invincibility = 0;
                }

                if (SpeedSneakers && SpeedSneakersTimer > 0f)
                {
                    SpeedSneakersTimer -= GameController.Frame;
                }
                if (SpeedSneakers && SpeedSneakersTimer <= 0f)
                {
                    MusicController.ToPlay = MusicController.StageMusic;
                    SpeedSneakersTimer = 0;
                    SpeedSneakers = false;
                }
            }
        }
        #endregion
        #region Check for Being Hurt
        if (Action != 9 && Action != 10)
        {
            if (Hurt == 1)
            {
                if (Invincibility != 0 || Action == 9)
                {
                    Hurt = 0;
                }
                else if (Shield != 0)
                {
                    XSpeed = -2.2f * (float)Direction;
                    YSpeed = 4.2f;
                    Action = 8;
                    Ground = false;
                    GroundAngle = 0f;
                    AllowInput = false;
                    Hurt = 0;
                    Invincibility = 2;
                    InvincibilityTimer = 250;
                    Shield = 0;
                    AudioController.PlaySFX(Sound_Hurt);
                    PlayerAction = Action08_Hurt;
                }
                else if (LevelController.CurrentLevel.Rings > 0)
                {
                    XSpeed = -2.2f * (float)Direction;
                    YSpeed = 4.2f;
                    Action = 8;
                    Ground = false;
                    GroundAngle = 0f;
                    AllowInput = false;
                    Hurt = 0;
                    Invincibility = 2;
                    InvincibilityTimer = 250;
                    LevelController.RingLoss(LevelController.CurrentLevel.Rings, XPosition, YPosition);
                    LevelController.CurrentLevel.Rings = 0;
                    AudioController.PlaySFX(Sound_LoseRings);
                    PlayerAction = Action08_Hurt;
                }
                else if (LevelController.CurrentLevel.Rings == 0)
                {
                    Hurt = 2;
                }
            }
            if (YPosition < -LevelController.CurrentLevel.Height - 32f)
            {
                Hurt = 2;
            }
            if (Hurt == 2)
            {
                LevelController.AllowTime = LevelController.AllowPause = false;

                foreach (BaseObject objRef in LevelController.CurrentLevel.ObjectList)
                {
                    if (objRef == this) continue;
                    objRef.enabled = false;
                }

                foreach (Animator animator in FindObjectsOfType<Animator>())
                {
                    if (animator == this.animator) continue;
                    animator.enabled = false;
                }

                CameraController.CameraMode = 3;
                Ground = false;
                XSpeed = 0f;
                YSpeed = 7.8f;
                Action = 9;
                Hurt = 0;
                CollisionLayer = 0;
                PlayerAction = Action09_Die;
                AudioController.PlaySFX(Sound_Hurt);
            }
            if (Hurt == 3)
            {
                LevelController.AllowTime = LevelController.AllowPause = false;

                foreach (BaseObject objRef in LevelController.CurrentLevel.ObjectList)
                {
                    if (objRef == this) continue;
                    objRef.enabled = false;
                }

                foreach (Animator animator in FindObjectsOfType<Animator>())
                {
                    if (animator == this.animator) continue;
                    animator.enabled = false;
                }

                CameraController.CameraMode = 3;
                Ground = false;
                XSpeed = 0f;
                YSpeed = 0f;
                Action = 10;
                Hurt = 0;
                CollisionLayer = 0;
                PlayerAction = Action09_Die;
                AudioController.PlaySFX(Sound_Drown);
            }
        }
        #endregion
        #endregion
        #region Player Animations
        #region Change Character
        if (SuperFlag != SuperForm)
        {
            for (int i = 0; i < SuperSkins.Length; i++)
            {
                SuperSkins[i].SetActive(i == Character && SuperForm);
            }

            for (int i = 0; i < Skins.Length; i++)
            {
                Skins[i].SetActive(i == Character && !SuperForm);
            }

            if (SuperForm)
            {
                animator = SuperSkins[Character].GetComponent<Animator>();
                render = SuperSkins[Character].GetComponent<SpriteRenderer>();
            }
            else
            {
                animator = Skins[Character].GetComponent<Animator>();
                render = Skins[Character].GetComponent<SpriteRenderer>();
            }

            SuperFlag = SuperForm;
        }
        #endregion
        #region Change Direction
        if (AllowDirection)
        {
            if (inpDir > 0)
            {
                Direction = Directions.Right;
            }
            if (inpDir < 0)
            {
                Direction = Directions.Left;
            }
        }

        render.flipX = Direction < 0;
        #endregion
        #region Change Animation Speed
        if (AnimationName == "Stopped" ||
            AnimationName == "Die" ||
            AnimationName == "Drown")
        {
            AnimationRate = 5;
        }
        else if (AnimationName == "Breathe" ||
                 AnimationName == "Hurt")
        {
            AnimationRate = 15;
        }
        else if (AnimationName == "Walking" ||
                 AnimationName == "Air Walking" ||
                 AnimationName == "Jog" ||
                 AnimationName == "Jumping" ||
                 AnimationName == "Skid Turn" ||
                 AnimationName == "Springs")
        {
            AnimationRate = 20;
        }
        else if (AnimationName == "Running" ||
                 AnimationName == "Skidding" ||
                 AnimationName == "Stand Up" ||
                 AnimationName == "Crouch Down" ||
                 AnimationName == "Spindash")
        {
            AnimationRate = 30;
        }
        else if (AnimationName == "Dash")
        {
            AnimationRate = 40;
        }

        this.animator.speed = 1f;

        if (AnimationName == "Walking" ||
            AnimationName == "Air Walking" ||
            AnimationName == "Jog" ||
            AnimationName == "Running" ||
            AnimationName == "Dash" ||
            AnimationName == "Jumping" ||
            AnimationName == "Spindash")
        {
            if (Ground)
            {
                if (AnimationName == "Walking" ||
                    AnimationName == "Jog")
                {
                    animator.speed = 20f + (Mathf.Abs(GroundSpeed) * 3f);
                }
                else if (AnimationName == "Running")
                {
                    animator.speed = 20f + (Mathf.Abs(GroundSpeed) * 4f);
                }
                else if (AnimationName == "Dash")
                {
                    animator.speed = 30f + (Mathf.Abs(GroundSpeed) * 4f);
                }
            }
            else if (AnimationName == "Air Walking" ||
                     AnimationName == "Running" ||
                     AnimationName == "Dash")
            {
                animator.speed = 20f + (Mathf.Abs(GroundSpeed) * 3f);
                //animator.speed = 20f + (Mathf.Abs((Action != 40) ? GroundSpeed : XSpeed) * 3f);
            }

            if (Action == 1 && AnimationName == "Jumping")
            {
                animator.speed = 25f + (Mathf.Abs(GroundSpeed) * 10f);
            }
            if (Action == 5 && AnimationName == "Spindash")
            {
                animator.speed = 20f + (Mathf.Abs(SpindashRev) * 10f);
            }
            if (Action == 6 && AnimationName == "Jumping")
            {
                animator.speed = 25f + (Mathf.Abs(GroundSpeed) * 10f);
            }

            animator.speed /= AnimationRate;
        }
        #endregion
        #region Change Angle
        if (Ground)
        {
            if (RotationMethod == RotationMethods.Partial || RotationMethod == RotationMethods.Legacy)
            {
                if (Mathf.Abs(((GroundAngle - SmoothAngle + 540f) % 360f) - 180f) < 60f && Mathf.Abs(((0f - GroundAngle + 540f) % 360f) - 180f) >= 40f)
                {
                    SmoothAngle = ((720f + SmoothAngle) % 360f) + ((((GroundAngle - SmoothAngle + 540f) % 360f) - 180f) * Mathf.Max(0.165f, Mathf.Abs(XSpeed) / MaxXSpeed * 0.8f) * Time.timeScale);
                }
                else if (Mathf.Abs(((GroundAngle - SmoothAngle + 540f) % 360f) - 180f) < 60f && Mathf.Abs(((0f - GroundAngle + 540f) % 360f) - 180f) < 40f)
                {
                    SmoothAngle = ((720f + SmoothAngle) % 360f) + ((((0f - SmoothAngle + 540f) % 360f) - 180f) * Mathf.Max(0.165f, Mathf.Abs(XSpeed) / MaxXSpeed * 0.8f) * Time.timeScale);
                }
                else if (Mathf.Abs(((GroundAngle - SmoothAngle + 540f) % 360f) - 180f) >= 60f)
                {
                    SmoothAngle = (720f + GroundAngle) % 360f;
                }
            }
            else
            {
                SmoothAngle = ((720f + SmoothAngle) % 360f) + ((((GroundAngle - SmoothAngle + 540f) % 360f) - 180f) * Mathf.Max(0.165f, Mathf.Abs(XSpeed) / MaxXSpeed * 0.8f) * Time.timeScale);
                SmoothAngle = ((720f + SmoothAngle) % 360f) + ((((0f - SmoothAngle + 540f) % 360f) - 180f) * Mathf.Max(0.165f, Mathf.Abs(XSpeed) / MaxXSpeed * 0.8f) * Time.timeScale);
                SmoothAngle = (720f + GroundAngle) % 360f;
            }

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Jumping"))
            {
                Angle = 0f;
            }
            else
            {
                Angle = SmoothAngle;
            }
        }
        else
        {
            SmoothAngle = 0f;

            if (Angle < 180f)
            {
                Angle = Mathf.Max(Angle - (4f * Time.timeScale), 0f);
            }
            else
            {
                Angle = Mathf.Min(Angle + (4f * Time.timeScale), 360f) % 360f;
            }
        }
        #endregion
        #region Flash!
        if (Action != 8 && Invincibility == 2 && InvincibilityTimer > 0f)
        {
            render.enabled = (LevelController.LevelTimer % 6f) < 3f;
        }
        else if (Invincibility != 2)
        {
            render.enabled = true;
        }
        #endregion
        #endregion
        #region Water
        if (!Underwater && Action != 9 && Action != 10 && LevelController.CurrentLevel.Water && YPosition <= LevelController.CurrentLevel.WaterLevel)
        {
            Underwater = true;
            if (!Ground)
            {
                XSpeed -= (XSpeed - (XSpeed * 0.2f)) * Time.timeScale;
                YSpeed -= (YSpeed - (YSpeed * 0.5f)) * Time.timeScale;
            }
            AudioController.PlaySFX(Sound_Splash);
            Splash waterSplash = SceneController.CreateStageObject("Water Splash", XPosition, YPosition) as Splash;
            waterSplash.XPosition = XPosition;
            waterSplash.render.sortingLayerName = render.sortingLayerName;
        }
        if (Underwater && Action != 9 && Action != 10 && LevelController.CurrentLevel.Water && YPosition >= LevelController.CurrentLevel.WaterLevel)
        {
            Underwater = false;
            if (!Ground)
            {
                YSpeed -= (YSpeed - (YSpeed * 1.3f)) * Time.timeScale;
            }
            AudioController.PlaySFX(Sound_Splash);
            Splash waterSplash = SceneController.CreateStageObject("Water Splash", XPosition, YPosition) as Splash;
            waterSplash.XPosition = XPosition;
            waterSplash.render.sortingLayerName = render.sortingLayerName;
        }

        if (Action == 9 || Action == 10)
        {
            Air = 0;
            Underwater = false;
        }

        if (Underwater)
        {
            if (Shield != 4)
            {
                if (LevelController.GlobalTimer % 60f == 0f && GameController.Frame == 1)
                {
                    Air++;
                }
            }
            else
            {
                Air = 0;
            }

            if (Air == 20 && MusicController.Playing != "Drowning")
            {
                MusicController.ToPlay = "Drowning";
            }
            if (Air < 20 && MusicController.Playing == "Drowning")
            {
                MusicController.ToPlay = MusicController.QueuedMusic;
            }
            if (Air == 32)
            {
                Hurt = 3;
            }
        }
        else
        {
            Air = 0;
        }
        #endregion
        #region Effects
        #region Spindash Dust
        SpindashDust.SetActive(Action == 5);
        SpindashDust.GetComponent<Animator>().speed = this.animator.speed;
        SpindashDust.GetComponent<SpriteRenderer>().flipX = this.render.flipX;
        SpindashDust.GetComponent<SpriteRenderer>().sortingLayerName = this.render.sortingLayerName;
        SpindashDust.transform.position = new Vector3(XPosition - (Mathf.Sin(Angle * Mathf.Deg2Rad) * -13f), YPosition + (Mathf.Cos(Angle * Mathf.Deg2Rad) * -13f), 0f);
        SpindashDust.transform.rotation = Quaternion.Euler(0f, 0f, Angle);
        #endregion
        #region Skidding Dust
        if (Action == 4 && LevelController.GlobalTimer % 4 == 0 && GameController.Frame == 1)
        {
            Dust dust = SceneController.CreateStageObject("Skid Dust",
                XPosition - (Mathf.Sin(Angle * Mathf.Deg2Rad) * -13f),
                YPosition + (Mathf.Cos(Angle * Mathf.Deg2Rad) * -13f)) as Dust;
            dust.render.sortingLayerName = render.sortingLayerName;
        }
        #endregion
        #endregion
        #region Shields
        if (Shield <= 1)
        {
            ShieldStatus = 0;
        }
        #region Flame Shield
        if (Shield == 2)
        {
            if (PlayerInput.KeyActionAPressed && ShieldStatus == 1 && JumpVariable)
            {
                AudioController.PlaySFX(Sound_FireDash);
                YSpeed = -1f;
                XSpeed = (float)Direction * 10f;
                foreach (Shield shield in Shields)
                {
                    if (shield.ShieldType != Shield_Types.Flame)
                    {
                        continue;
                    }
                    shield.GetComponent<Animator>().Play("Walking");
                    shield.GetComponent<SpriteRenderer>().flipX = render.flipX;
                }
                ShieldStatus = 2;
            }
            if (JumpVariable && ShieldStatus == 0)
            {
                ShieldStatus = 1;
            }
            if (ShieldStatus != 0 && Ground)
            {
                ShieldStatus = 0;
                foreach (Shield shield in Shields)
                {
                    if (shield.ShieldType != Shield_Types.Flame)
                    {
                        continue;
                    }
                    shield.GetComponent<Animator>().Play("Stopped");
                    shield.GetComponent<SpriteRenderer>().flipX = false;
                }
            }
        }
        #endregion
        #region Magnetic Shield
        if (Shield == 3)
        {
            if (PlayerInput.KeyActionAPressed && ShieldStatus == 1 && JumpVariable)
            {
                ShieldStatus = 2;
                AudioController.PlaySFX(Sound_LightingJump);
                YSpeed = 7f;
            }
            if (JumpVariable && ShieldStatus == 0)
            {
                ShieldStatus = 1;
            }
            if (ShieldStatus != 0 && Ground)
            {
                ShieldStatus = 0;
            }
        }
        #endregion
        #region Aquatic Shield
        if (Shield == 4)
        {
            if (PlayerInput.KeyActionAPressed && !Ground && ShieldStatus == 1)
            {
                ShieldStatus = 2;
                XSpeed *= (YSpeed - (YSpeed * 0.1f)) * Time.timeScale;
                YSpeed = Mathf.Min(YSpeed, -8f);
                foreach (Shield shield in Shields)
                {
                    if (shield.ShieldType != Shield_Types.Aquatic)
                    {
                        continue;
                    }
                    shield.GetComponent<Animator>().Play("Walking");
                }
            }
            if (PlayerInput.KeyActionAPressed && JumpVariable && !Ground && ShieldStatus == 0 || ShieldStatus == 2 && !Ground && YSpeed > 0f)
            {
                ShieldStatus = 1;
            }
            if (Ground && ShieldStatus == 2 && Hurt == 0)
            {
                XSpeed = (Mathf.Sin(GroundAngle * Mathf.Deg2Rad) * -8f) + (Mathf.Cos(GroundAngle * Mathf.Deg2Rad) * GroundSpeed);
                YSpeed = (Mathf.Cos(GroundAngle * Mathf.Deg2Rad) * 8f) + (Mathf.Sin(GroundAngle * Mathf.Deg2Rad) * GroundSpeed);
                Ground = false;
                GroundAngle = 0f;
                Action = 1;
                JumpVariable = true;
                AudioController.PlaySFX(Sound_BubbleBounce);
                ShieldStatus = 1;
                AllowInput = true;
                foreach (Shield shield in Shields)
                {
                    if (shield.ShieldType != Shield_Types.Aquatic)
                    {
                        continue;
                    }
                    shield.GetComponent<Animator>().Play("Running");
                }
                PlayerAction = Action01_Jump;
            }
            if (Ground)
            {
                ShieldStatus = 0;
                foreach (Shield shield in Shields)
                {
                    if (shield.ShieldType != Shield_Types.Aquatic)
                    {
                        continue;
                    }
                    shield.GetComponent<Animator>().Play("Stopped");
                }
            }
        }
        #endregion
        #endregion
    }
    #endregion
    #region Player Update (Post)
    private new void LateUpdate()
    {
        base.LateUpdate();

        if (RotationMethod == RotationMethods.Legacy || RotationMethod == RotationMethods.Sonic1)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Round(Angle / RotationSnap) * RotationSnap);
        }
    }
    #endregion
    #region Player Actions
    #region Common Actions
    #region Actions - [00] Common
    public void Action00_Common()
    {
        Action = 0;

        if (Ground)
        {
            if (Mathf.Abs(GroundSpeed) == 0f)
            {
                if (!(animator.GetCurrentAnimatorStateInfo(0).IsName("Stopped") ||
                      animator.GetCurrentAnimatorStateInfo(0).IsName("Bored")))
                {
                    animator.Play("Stopped");
                    AnimationName = "Stopped";
                }
            }
            else if (Mathf.Abs(GroundSpeed) > 0f && Mathf.Abs(GroundSpeed) < 4f)
            {
                animator.Play("Walking");
                AnimationName = "Walking";
            }
            else if (Mathf.Abs(GroundSpeed) >= 4f && Mathf.Abs(GroundSpeed) < 6f)
            {
                animator.Play("Jog");
                AnimationName = "Jog";
            }
            else if (Mathf.Abs(GroundSpeed) < 12f)
            {
                animator.Play("Running");
                AnimationName = "Running";
            }
            else if (!(animator.GetCurrentAnimatorStateInfo(0).IsName("Dash") ||
                       animator.GetCurrentAnimatorStateInfo(0).IsName("Dash (Loop)")))
            {
                animator.Play("Dash");
                AnimationName = "Dash";
            }
        }
        else if (AnimationName == "Walking" || AnimationName == "Jog")
        {
            animator.Play("Air Walking");
            AnimationName = "Air Walking";
        }

        if (Ground && Mathf.Abs(GroundSpeed) == 0f && !(PlayerInput.KeyLeft || PlayerInput.KeyRight))
        {
            if (PlayerInput.KeyUp)
            {
                AllowInput = false;
                AllowDirection = false;
                PlayerAction = Action02_StandUp;
            }
            if (PlayerInput.KeyDown)
            {
                AllowInput = false;
                AllowDirection = false;
                PlayerAction = Action03_CrouchDown;
            }
        }

        if (Quadrant == 0 && Ground && ControlLockTimer == 0f)
        {
            if (GroundSpeed >= 4f && PlayerInput.KeyLeft)
            {
                SkidTimer = 20;
                AllowInput = true;
                AllowDirection = false;
                Direction = Directions.Right;
                animator.Play("Skidding");
                AnimationName = "Skidding";
                AudioController.PlaySFX(Sound_Skidding);
                PlayerAction = Action04_Skidding;
            }
            if (GroundSpeed <= -4f && PlayerInput.KeyRight)
            {
                SkidTimer = 20;
                AllowInput = true;
                AllowDirection = false;
                Direction = Directions.Left;
                animator.Play("Skidding");
                AnimationName = "Skidding";
                AudioController.PlaySFX(Sound_Skidding);
                PlayerAction = Action04_Skidding;
            }
        }

        if (Ground && Mathf.Abs(GroundSpeed) >= 0.2f && PlayerInput.KeyDown && !(PlayerInput.KeyLeft || PlayerInput.KeyRight))
        {
            AllowInput = false;
            AllowDirection = true;
            animator.Play("Jumping");
            AnimationName = "Jumping";
            AudioController.PlaySFX(Sound_Rolling);
            PlayerAction = Action06_Rolling;
        }

        if (Ground && PlayerInput.KeyActionAPressed)
        {
            XSpeed = (Mathf.Sin(GroundAngle * Mathf.Deg2Rad) * -JumpForce) + (Mathf.Cos(GroundAngle * Mathf.Deg2Rad) * GroundSpeed);
            YSpeed = (Mathf.Cos(GroundAngle * Mathf.Deg2Rad) * JumpForce) + (Mathf.Sin(GroundAngle * Mathf.Deg2Rad) * GroundSpeed);
            Ground = false;
            GroundAngle = 0f;
            ControlLockTimer = 0;
            AllowInput = true;
            AllowDirection = true;
            JumpVariable = true;
            animator.Play("Jumping");
            AnimationName = "Jumping";
            AudioController.PlaySFX(Sound_Jump);
            PlayerAction = Action01_Jump;
        }
    }
    #endregion
    #region Actions - [01] Jump
    public void Action01_Jump()
    {
        Action = 1;
        Attacking = true;

        animator.Play("Jumping");
        AnimationName = "Jumping";

        if (!PlayerInput.KeyActionA && YSpeed > JumpReleaseForce && JumpVariable)
        {
            YSpeed = JumpReleaseForce;
        }

        if (Ground)
        {
            AllowInput = true;
            AllowDirection = true;
            PlayerAction = Action00_Common;
        }
    }
    #endregion
    #region Actions - [02] Stand Up
    public void Action02_StandUp()
    {
        Action = 2;

        if (PlayerInput.KeyUp)
        {
            animator.Play("Stand Up");
        }
        else
        {
            animator.Play("Stand Up (Reverse)");
        }
        AnimationName = "Stand Up";

        if (!Ground || GroundSpeed != 0f ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("Stand Up (Reverse)") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            AllowInput = true;
            AllowDirection = true;
            PlayerAction = Action00_Common;
        }
    }
    #endregion
    #region Actions - [03] Crouch Down
    public void Action03_CrouchDown()
    {
        Action = 3;

        if (PlayerInput.KeyDown)
        {
            animator.Play("Crouch Down");
        }
        else
        {
            animator.Play("Crouch Down (Reverse)");
        }
        AnimationName = "Crouch Down";

        if (!Ground || GroundSpeed != 0f ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("Crouch Down (Reverse)") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            AllowInput = true;
            AllowDirection = true;
            PlayerAction = Action00_Common;
        }

        if (PlayerInput.KeyActionAPressed)
        {
            audioSource.Play();
            PlayerAction = Action05_Spindash;
        }
    }
    #endregion
    #region Actions - [04] Skidding
    public void Action04_Skidding()
    {
        Action = 4;

        if (SkidTimer > 0)
        {
            SkidTimer -= GameController.Frame;
        }

        if (SkidTimer == 0f && (Mathf.Abs(GroundSpeed) >= 0f && !(PlayerInput.KeyLeft || PlayerInput.KeyRight) ||
            AnimationName == "Skidding" && Mathf.Abs(GroundSpeed) > 1.5f &&
           (PlayerInput.KeyLeft && GroundSpeed <= 0f || PlayerInput.KeyRight && GroundSpeed >= 0f)) ||
            !Ground)
        {
            animator.Play("Walking");
            AnimationName = "Walking";
            AllowDirection = true;
            AllowInput = true;
            PlayerAction = Action00_Common;
        }

        if ((animator.GetCurrentAnimatorStateInfo(0).IsName("Skidding") ||
             animator.GetCurrentAnimatorStateInfo(0).IsName("Skidding (Loop)")) &&
            (PlayerInput.KeyLeft || PlayerInput.KeyRight) && Ground && Mathf.Abs(GroundSpeed) <= 1.5f)
        {
            Direction = (Directions)((int)Direction * -1);
            animator.Play("Skidding (Turn)");
            AnimationName = "Skid Turn";
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Skidding (Turn)") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            AllowInput = true;
            AllowDirection = true;
            PlayerAction = Action00_Common;
        }

        if (Ground && Mathf.Abs(GroundSpeed) >= 0.2f && PlayerInput.KeyDown && !(PlayerInput.KeyLeft || PlayerInput.KeyRight))
        {
            AllowDirection = false;
            AllowInput = false;
            animator.Play("Jumping");
            AnimationName = "Jumping";
            AudioController.PlaySFX(Sound_Rolling);
            PlayerAction = Action06_Rolling;
        }

        if (Ground && PlayerInput.KeyActionAPressed)
        {
            XSpeed = (Mathf.Sin(GroundAngle * Mathf.Deg2Rad) * -JumpForce) + (Mathf.Cos(GroundAngle * Mathf.Deg2Rad) * GroundSpeed);
            YSpeed = (Mathf.Cos(GroundAngle * Mathf.Deg2Rad) * JumpForce) + (Mathf.Sin(GroundAngle * Mathf.Deg2Rad) * GroundSpeed);
            Ground = false;
            GroundAngle = 0f;
            ControlLockTimer = 0;
            AllowInput = true;
            AllowDirection = true;
            JumpVariable = true;
            animator.Play("Jumping");
            AnimationName = "Jumping";
            AudioController.PlaySFX(Sound_Jump);
            PlayerAction = Action01_Jump;
        }
    }
    #endregion
    #region Actions - [05] Spindash
    public void Action05_Spindash()
    {
        Action = 5;
        Attacking = true;

        animator.Play("Spindash");
        AnimationName = "Spindash";

        SpindashRev -= ((SpindashRev / 0.125f) / 256f) * Time.timeScale;

        if (SpindashRev <= 0.1f)
        {
            SpindashRev = 0f;
        }

        if (PlayerInput.KeyActionAPressed)
        {
            animator.Play("Spindash", -1, 0f);
            SpindashRev += 2f;
            audioSource.Stop();
            audioSource.Play();
        }

        if (!PlayerInput.KeyDown)
        {
            GroundSpeed += (8f + Mathf.Floor(SpindashRev / 2f)) * (float)Direction;
            AllowDirection = false;
            AllowInput = false;
            animator.Play("Jumping");
            AnimationName = "Jumping";
            CameraController.LagTimer = 16f;
            AudioController.PlaySFX(Sound_Release);
            PlayerAction = Action06_Rolling;
        }
    }
    #endregion
    #region Actions - [06] Rolling
    public void Action06_Rolling()
    {
        Action = 6;
        Attacking = true;

        SpindashRev = 0f;

        animator.Play("Jumping");
        AnimationName = "Jumping";

        GroundSpeed = Mathf.Max(Mathf.Abs(GroundSpeed) - (RollFriction * Time.timeScale), 0f) * Mathf.Sign(GroundSpeed);

        if (GroundSpeed < 0f && PlayerInput.KeyRight ||
            GroundSpeed > 0f && PlayerInput.KeyLeft)
        {
            GroundSpeed = Mathf.Max(Mathf.Abs(GroundSpeed) - (RollDeceleration * Time.timeScale), 0f) * Mathf.Sign(GroundSpeed);
        }

        if (Ground)
        {
            if (Mathf.Sign(GroundSpeed) == Mathf.Sign(Mathf.Sin(GroundAngle * Mathf.Deg2Rad)))
            {
                GroundSpeed -= SlopeRollUpFactor * Mathf.Sin(GroundAngle * Mathf.Deg2Rad) * Time.timeScale;
            }
            else
            {
                GroundSpeed -= SlopeRollDownFactor * Mathf.Sin(GroundAngle * Mathf.Deg2Rad) * Time.timeScale;
            }
        }

        if (!Ground)
        {
            AllowInput = true;
            AllowDirection = true;
            JumpVariable = false;
            PlayerAction = Action01_Jump;
        }

        if (Ground && PlayerInput.KeyActionAPressed)
        {
            XSpeed = (Mathf.Sin(GroundAngle * Mathf.Deg2Rad) * -JumpForce) + (Mathf.Cos(GroundAngle * Mathf.Deg2Rad) * GroundSpeed);
            YSpeed = (Mathf.Cos(GroundAngle * Mathf.Deg2Rad) * JumpForce) + (Mathf.Sin(GroundAngle * Mathf.Deg2Rad) * GroundSpeed);
            Ground = false;
            GroundAngle = 0f;
            Action = 1;
            ControlLockTimer = 0;
            AllowInput = true;
            AllowDirection = true;
            JumpVariable = true;
            animator.Play("Jumping");
            AnimationName = "Jumping";
            AudioController.PlaySFX(Sound_Jump);
            PlayerAction = Action01_Jump;
        }

        if (Mathf.Abs(GroundSpeed) < RollDeceleration)
        {
            AllowInput = true;
            AllowDirection = true;
            PlayerAction = Action00_Common;
        }
    }
    #endregion
    #region Actions - [07] Breathe
    public void Action07_Breathe()
    {
        Action = 7;

        animator.Play("Breathe");
        AnimationName = "Breathe";

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Breathe") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && Ground)
        {
            AllowInput = true;
            AllowDirection = true;
            PlayerAction = Action00_Common;
        }
    }
    #endregion
    #region Actions - [08] Hurt
    public void Action08_Hurt()
    {
        Action = 8;

        animator.Play("Hurt");
        AnimationName = "Hurt";

        if (Ground)
        {
            GroundSpeed = 0f;
            AllowInput = true;
            AllowDirection = true;
            PlayerAction = Action00_Common;
        }
    }
    #endregion
    #region Actions - [09] Die
    public void Action09_Die()
    {
        Action = 9;
        AllowCollision = AllowDirection = AllowInput = AllowPhysics = false;

        animator.Play("Die");
        AnimationName = "Die";

        if (YPosition < SceneController.YBottomFrame - 48f)
        {
            GameController.Lives--;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    #endregion
    #region Actions - [10] Drown
    public void Action10_Drown()
    {
        Action = 10;
        AllowCollision = AllowDirection = AllowInput = AllowPhysics = false;

        animator.Play("Drown");
        AnimationName = "Drown";

        if (YPosition < SceneController.YBottomFrame - 48f)
        {
            GameController.Lives--;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    #endregion
    #endregion
    #region Other Actions
    #region Actions - [40] Springs
    public void Action40_Springs()
    {
        Action = 40;
        animator.Play((YSpeed > 0f) ? "Springs" : "Air Walking");
        AnimationName = (YSpeed > 0f) ? "Springs" : "Air Walking";
        AllowInput = true;
        AllowDirection = true;
        if (Ground)
        {
            PlayerAction = Action00_Common;
        }
    }
    #endregion
    #endregion
    #endregion
}
