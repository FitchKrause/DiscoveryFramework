using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerPhysics : BaseObject
{
    #region Player Values
    #region Player movement values
    [HideInInspector] public int Quadrant;
    [HideInInspector] public int Action;
    [HideInInspector] public int ControlLock;
    [HideInInspector] public bool Landed;
    [HideInInspector] public float LandingSpeed;
    [HideInInspector] public int LandFrame;
    [HideInInspector] public int CeilingLand;
    [HideInInspector] public bool Attacking;
    [HideInInspector] public bool AllowInput;
    [HideInInspector] public bool AllowX;
    [HideInInspector] public bool AllowY;
    [HideInInspector] public bool AllowLanding;
    [HideInInspector] public bool AllowFalling;
    [HideInInspector] public bool AllowDirection;
    #endregion
    #region Player action values
    [HideInInspector] public bool JumpVariable;
    [HideInInspector] public float SpindashRev;
    [HideInInspector] public int SkidTimer;
    #endregion
    #region Player miscellaneous values
    [HideInInspector] public int Character;
    [HideInInspector] public bool SuperForm;
    [HideInInspector] public int Shield;
    [HideInInspector] public int ShieldStatus;
    [HideInInspector] public int Invincibility;
    [HideInInspector] public int InvincibilityTimer;
    [HideInInspector] public bool SpeedSneakers;
    [HideInInspector] public int SpeedSneakersTimer;
    [HideInInspector] public bool Underwater;
    [HideInInspector] public int Air;
    [HideInInspector] public int Hurt;
    [HideInInspector] public int Direction;
    [HideInInspector] public float Animation;
    [HideInInspector] public float AnimationAngle;
    [HideInInspector] public float SmoothAngle;
    #endregion
    #region Player sounds
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
    #region Player constants
    [Header("Player constants")]
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
    #region Player components
    public delegate void PlayerAction();
    public PlayerAction CurrentAction;
    public HitBox Rect;
    public GameObject[] Skins;
    public GameObject[] SuperSkins;
    public GameObject SpindashDust;
    [HideInInspector] public Collider2D ColliderFloor;
    [HideInInspector] public Collider2D ColliderCeiling;
    [HideInInspector] public Collider2D ColliderWallLeft;
    [HideInInspector] public Collider2D ColliderWallRight;
    private AudioSource audioSource;
    private Shield[] Shields;
    #endregion
    #endregion
    #region Player Initialization
    private new void Start()
    {
        Direction = 1;

        audioSource = GetComponent<AudioSource>();
        audioSource.volume = (AudioController.SFX_VOLUME * (AudioController.MASTER_VOLUME / 100f)) / 100f;
        Shields = FindObjectsOfType<Shield>();

        base.Start();

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

        animator = Skins[0].GetComponent<Animator>();
        render = Skins[0].GetComponent<SpriteRenderer>();

        for (int i = 0; i < Skins.Length; i++)
        {
            Skins[i].transform.SetParent(transform);
            Skins[i].transform.position = transform.position;
            Skins[i].GetComponent<SpriteRenderer>().sortingOrder = 0;
        }
        for (int i = 0; i < SuperSkins.Length; i++)
        {
            SuperSkins[i].transform.SetParent(transform);
            SuperSkins[i].transform.position = transform.position;
            SuperSkins[i].GetComponent<SpriteRenderer>().sortingOrder = 0;
        }

        if (YPosition < GameObject.Find("Water Mark").transform.position.y)
        {
            Underwater = true;
        }

        AllowInput = AllowX = AllowY = AllowLanding = AllowFalling = AllowDirection = true;
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
        JumpForce = 6.5f;
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
            GravityForce = 0.0625f;
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
                GroundSpeed -= SlopeFactor * Mathf.Sin(GroundAngle * Mathf.Deg2Rad) * GameController.DeltaTime;
            }
            #endregion
            #region X Control
            if (AllowInput && !(Ground && ControlLock > 0 || Action == 6))
            {
                if (inpDir < 0)
                {
                    if (Ground)
                    {
                        if (GroundSpeed > 0f)
                        {
                            GroundSpeed -= Deceleration * GameController.DeltaTime;

                            if (GroundSpeed <= 0f)
                            {
                                GroundSpeed = -0.5f;
                            }
                        }
                        else if (GroundSpeed > -TopSpeed)
                        {
                            GroundSpeed -= Acceleration * GameController.DeltaTime;

                            if (GroundSpeed <= -TopSpeed)
                            {
                                GroundSpeed = -TopSpeed;
                            }
                        }
                    }
                    else if (XSpeed > -TopSpeed)
                    {
                        XSpeed -= AirAcceleration * GameController.DeltaTime;

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
                            GroundSpeed += Deceleration * GameController.DeltaTime;

                            if (GroundSpeed >= 0f)
                            {
                                GroundSpeed = 0.5f;
                            }
                        }
                        else if (GroundSpeed < TopSpeed)
                        {
                            GroundSpeed += Acceleration * GameController.DeltaTime;

                            if (GroundSpeed >= TopSpeed)
                            {
                                GroundSpeed = TopSpeed;
                            }
                        }
                    }
                    else if (XSpeed < TopSpeed)
                    {
                        XSpeed += AirAcceleration * GameController.DeltaTime;

                        if (XSpeed >= TopSpeed)
                        {
                            XSpeed = TopSpeed;
                        }
                    }
                }
                
                if (Ground && inpDir == 0)
                {
                    GroundSpeed -= Mathf.Min(Mathf.Abs(GroundSpeed), Friction * GameController.DeltaTime) * Mathf.Sign(GroundSpeed);
                    
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
                if (ControlLock == 0)
                {
                    if (Mathf.Abs(GroundSpeed) < 2.5f && GroundAngle > 35f && GroundAngle < 325f)
                    {
                        ControlLock = 30;
                        if (AllowFalling && GroundAngle >= 75f && GroundAngle <= 285f)
                        {
                            CeilingLand = -1;
                            XSpeed = GroundSpeed * Mathf.Cos(GroundAngle * Mathf.Deg2Rad);
                            YSpeed = GroundSpeed * Mathf.Sin(GroundAngle * Mathf.Deg2Rad);
                            GroundAngle = 0f;
                            SmoothAngle = 0f;
                            Ground = false;
                        }
                        else if (AllowFalling)
                        {
                            if (GroundAngle < 180f)
                            {
                                GroundSpeed -= 0.5f * GameController.DeltaTime;
                            }
                            else
                            {
                                GroundSpeed += 0.5f * GameController.DeltaTime;
                            }
                        }
                    }
                }
                else
                {
                    ControlLock--;
                }
            }
            #endregion
        }
        #endregion
        #endregion
        #region Player Movement
        #region Start Movement
        GroundSpeed = Mathf.Clamp(GroundSpeed, -MaxXSpeed, MaxXSpeed);
        XSpeed = Mathf.Clamp(XSpeed, -MaxXSpeed, MaxXSpeed);
        YSpeed = Mathf.Clamp(YSpeed, -MaxYSpeed, MaxYSpeed);
        
        if (Landed)
        {
            LandFrame++;
            if (LandFrame > 1)
            {
                Landed = false;
            }
        }
        else
        {
            LandFrame = 0;
        }
        
        float AddX = 0f;
        float AddY = 0f;

        if (AllowX && Ground)
        {
            XSpeed = GroundSpeed * Mathf.Cos(GroundAngle * Mathf.Deg2Rad);
            YSpeed = GroundSpeed * Mathf.Sin(GroundAngle * Mathf.Deg2Rad);

            AddX = XSpeed * GameController.DeltaTime;
            AddY = YSpeed * GameController.DeltaTime;
        }
        else if (!Ground)
        {
            if (AllowX) AddX = XSpeed * GameController.DeltaTime;
            if (AllowY) AddY = YSpeed * GameController.DeltaTime;
        }

        ObjectLoops = (int)(Mathf.Sqrt((XSpeed * XSpeed) + (YSpeed * YSpeed)) + 1f);

        AddX /= ObjectLoops;
        AddY /= ObjectLoops;

        for (int i = 0; i < ObjectLoops; i++)
        {
            XPosition += AddX;
            YPosition += AddY;
            #endregion
            #region Horizontal Collisions
            if (AllowX)
            {
                RaycastHit2D wallLeftHit = SensorCast(new Vector2(0f, WallShift), Vector2.left, WidthRadius);
                RaycastHit2D wallRightHit = SensorCast(new Vector2(0f, WallShift), Vector2.right, WidthRadius);
                
                if (wallLeftHit)
                {
                    Vector2 vector = new Vector2(-Mathf.Cos(GroundAngle * Mathf.Deg2Rad), -Mathf.Sin(GroundAngle * Mathf.Deg2Rad)) * (wallLeftHit.distance - WidthRadius);
                    XPosition += vector.x;
                    YPosition += vector.y;
                    if (Ground) GroundSpeed = 0f;
                    else XSpeed = 0f;
                }
                
                if (wallRightHit)
                {
                    Vector2 vector = new Vector2(Mathf.Cos(GroundAngle * Mathf.Deg2Rad), Mathf.Sin(GroundAngle * Mathf.Deg2Rad)) * (wallRightHit.distance - WidthRadius);
                    XPosition += vector.x;
                    YPosition += vector.y;
                    if (Ground) GroundSpeed = 0f;
                    else XSpeed = 0f;
                }
            }
            #endregion
            #region Vertical Collisions
            if (AllowY && !Ground)
            {
                #region Floor and Ceiling Collisions
                RaycastHit2D floorLeftHit = SensorCast(new Vector2(-WidthRadius + 2f, 0f), Vector2.down, HeightRadius);
                RaycastHit2D floorRightHit = SensorCast(new Vector2(WidthRadius - 2f, 0f), Vector2.down, HeightRadius);
                RaycastHit2D floorHit = default(RaycastHit2D);

                RaycastHit2D ceilingLeftHit = SensorCast(new Vector2(-WidthRadius + 2f, 0f), Vector2.up, HeightRadius);
                RaycastHit2D ceilingRightHit = SensorCast(new Vector2(WidthRadius - 2f, 0f), Vector2.up, HeightRadius);
                RaycastHit2D ceilingHit = default(RaycastHit2D);

                #region Ceiling Collisions
                if (ceilingLeftHit && ceilingRightHit)
                {
                    if (ceilingLeftHit.distance < ceilingRightHit.distance)
                    {
                        ceilingHit = ceilingLeftHit;
                    }
                    else
                    {
                        ceilingHit = ceilingRightHit;
                    }
                }
                else if (!ceilingLeftHit && ceilingRightHit)
                {
                    ceilingHit = ceilingRightHit;
                }
                else if (ceilingLeftHit && !ceilingRightHit)
                {
                    ceilingHit = ceilingLeftHit;
                }
                
                if (ceilingHit)
                {
                    YPosition += ceilingHit.distance - HeightRadius;
                }
                #endregion
                #region Floor Collisions
                if (floorLeftHit && floorRightHit)
                {
                    if (floorLeftHit.distance < floorRightHit.distance)
                    {
                        floorHit = floorLeftHit;
                    }
                    else
                    {
                        floorHit = floorRightHit;
                    }
                }
                else if (!floorLeftHit && floorRightHit)
                {
                    floorHit = floorRightHit;
                }
                else if (floorLeftHit && !floorRightHit)
                {
                    floorHit = floorLeftHit;
                }
                
                if (floorHit)
                {
                    YPosition -= floorHit.distance - HeightRadius;
                }
                #endregion
                #endregion
                #region Landing
                #region Floor Landing
                if (AllowLanding && YSpeed <= 0f && floorHit)
                {
                    DetectAngle(true);
                    
                    if (GroundAngle >= 45f && GroundAngle <= 315f)
                    {
                        XSpeed += Mathf.Sin(GroundAngle * Mathf.Deg2Rad) * YSpeed;
                    }
                    else if (GroundAngle >= 22.5f && GroundAngle <= 337.5f)
                    {
                        XSpeed += Mathf.Sin(GroundAngle * Mathf.Deg2Rad) * (YSpeed * 0.5f);
                    }
                    
                    CeilingLand = 0;
                    GroundSpeed = XSpeed;
                    LandingSpeed = YSpeed;
                    Ground = true;
                    Landed = true;
                    JumpVariable = false;
                }
                #endregion
                #region Ceiling Landing
                if (AllowLanding && YSpeed > 0f && ceilingHit && CeilingLand == 0)
                {
                    CeilingLand = 1;
                    GroundAngle = 180f;
                    SmoothAngle = 180f;
                    floorLeftHit = SensorCast(new Vector2(-WidthRadius + 2f, 0f), Vector2.down, HeightRadius);
                    floorRightHit = SensorCast(new Vector2(WidthRadius - 2f, 0f), Vector2.down, HeightRadius);
                    if (CeilingLand == 1 && (floorLeftHit || floorRightHit))
                    {
                        CeilingLand = 2;
                        if (CeilingLand == 2)
                        {
                            DetectAngle(true);
                            if (GroundAngle >= 170f && GroundAngle <= 190f)
                            {
                                YSpeed = 0f;
                                CeilingLand = -1;
                                GroundAngle = 0f;
                                SmoothAngle = 0f;
                            }
                            else
                            {
                                CeilingLand = 3;
                                XSpeed += Mathf.Sin(GroundAngle * Mathf.Deg2Rad) * YSpeed;
                                if (CeilingLand == 3)
                                {
                                    CeilingLand = 0;
                                    GroundSpeed = XSpeed;
                                    LandingSpeed = YSpeed;
                                    Ground = true;
                                    Landed = true;
                                    Action = 1;
                                }
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
                #endregion
            }
            #endregion
            #region Slopes
            if (Ground)
            {
                #region Stick to Ground
                RaycastHit2D floorLeftHit = SensorCast(new Vector2(-WidthRadius + 2f, 0f), Vector2.down, HeightRadius + 12f);
                RaycastHit2D floorRightHit = SensorCast(new Vector2(WidthRadius - 2f, 0f), Vector2.down, HeightRadius + 12f);
                RaycastHit2D floorHit = default(RaycastHit2D);

                if (floorLeftHit && floorRightHit)
                {
                    if (floorLeftHit.distance < floorRightHit.distance)
                    {
                        floorHit = floorLeftHit;
                    }
                    else
                    {
                        floorHit = floorRightHit;
                    }
                }
                else if (!floorLeftHit && floorRightHit)
                {
                    floorHit = floorRightHit;
                }
                else if (floorLeftHit && !floorRightHit)
                {
                    floorHit = floorLeftHit;
                }

                if (floorHit)
                {
                    Vector2 vector = new Vector2(Mathf.Sin(GroundAngle * Mathf.Deg2Rad), -Mathf.Cos(GroundAngle * Mathf.Deg2Rad)) * (floorHit.distance - HeightRadius);
                    XPosition += vector.x;
                    YPosition += vector.y;
                }
                else
                {
                    CeilingLand = -1;
                    XSpeed = GroundSpeed * Mathf.Cos(GroundAngle * Mathf.Deg2Rad);
                    YSpeed = GroundSpeed * Mathf.Sin(GroundAngle * Mathf.Deg2Rad);
                    GroundAngle = 0f;
                    SmoothAngle = 0f;
                    Ground = false;
                }
                #endregion
                #region Detect angle
                DetectAngle();
            }
            else
            {
                GroundAngle = 0f;
                #endregion
            }
            #endregion
            #region End Movement
            Rect.XPosition = XPosition;
            Rect.YPosition = YPosition;
            Rect.WidthRadius = WidthRadius;
            Rect.HeightRadius = HeightRadius;

            ColliderFloor = BoxCast(new Vector2(0f, -HeightRadius + Mathf.Min(Ground ? 0f : YSpeed, 0f)), new Vector2((WidthRadius - 2f) * 2f, WidthRadius));
            ColliderCeiling = BoxCast(new Vector2(0f, HeightRadius + Mathf.Max(Ground ? 0f : YSpeed, 0f)), new Vector2((WidthRadius - 2f) * 2f, WidthRadius));
            ColliderWallLeft = BoxCast(new Vector2(-WidthRadius + Mathf.Min(Ground ? GroundSpeed : XSpeed, 0f), WallShift), new Vector2(WidthRadius, WidthRadius / 2f));
            ColliderWallRight = BoxCast(new Vector2(WidthRadius + Mathf.Max(Ground ? GroundSpeed : XSpeed, 0f), WallShift), new Vector2(WidthRadius, WidthRadius / 2f));
        }
        #endregion
        #endregion
        #region Player Control
        #region Control
        #region Y Control
        if (!Ground)
        {
            YSpeed -= GravityForce * GameController.DeltaTime;
            
            if (YSpeed > 0f && YSpeed < 4f && Mathf.Abs(XSpeed) >= 0.125f)
            {
                XSpeed *= AirDrag / GameController.DeltaTime;
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
        #region Actions
        if (CurrentAction != null)
        {
            Attacking = false;
            CurrentAction();
        }

        #region Manage Actions
        if (Action == 0)
        {
            CurrentAction = Action00_Common;
        }
        if (Action == 1)
        {
            Attacking = true;
            CurrentAction = Action01_Jump;
        }
        if (Action == 2)
        {
            AllowDirection = false;
            AllowInput = false;
            CurrentAction = Action02_StandUp;
        }
        if (Action == 3)
        {
            AllowDirection = false;
            AllowInput = false;
            CurrentAction = Action03_CrouchDown;
        }
        if (Action == 4)
        {
            AllowDirection = false;
            CurrentAction = Action04_Skidding;
        }
        if (Action == 5)
        {
            Attacking = true;
            AllowDirection = false;
            AllowInput = false;
            CurrentAction = Action05_Spindash;
        }
        if (Action == 6)
        {
            Attacking = true;
            AllowInput = false;
            CurrentAction = Action06_Rolling;
        }
        if (Action == 7)
        {
            CurrentAction = Action07_Breathe;
        }
        if (Action == 8)
        {
            AllowDirection = false;
            AllowInput = false;
            CurrentAction = Action08_Hurt;
        }
        if (Action == 9)
        {
            AllowDirection = false;
            AllowInput = false;
            CurrentAction = Action09_Die;
        }
        if (Action == 10)
        {
            AllowDirection = false;
            AllowInput = false;
            CurrentAction = Action10_Drown;
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
                if (LevelController.LevelTimer % 60 == 0 && LevelController.CurrentLevel.Rings > 0)
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
                if (Invincibility > 0 && InvincibilityTimer > 0)
                {
                    InvincibilityTimer--;
                }
                if (Invincibility > 0 && InvincibilityTimer <= 0)
                {
                    if (Invincibility == 1)
                    {
                        MusicController.ToPlay = MusicController.StageMusic;
                    }
                    InvincibilityTimer = 0;
                    Invincibility = 0;
                }

                if (SpeedSneakers && SpeedSneakersTimer > 0)
                {
                    SpeedSneakersTimer--;
                }
                if (SpeedSneakers && SpeedSneakersTimer <= 0)
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
                    XSpeed = -2.2f * Direction;
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
                    CurrentAction = Action08_Hurt;
                }
                else if (LevelController.CurrentLevel.Rings > 0)
                {
                    XSpeed = -2.2f * Direction;
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
                    CurrentAction = Action08_Hurt;
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
                CurrentAction = Action09_Die;
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
                CurrentAction = Action09_Die;
                AudioController.PlaySFX(Sound_Drown);
            }
        }
        #endregion
        #endregion
        #region Player Animations
        #region Change Character
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
        #endregion
        #region Change Direction
        if (AllowDirection)
        {
            if (inpDir > 0)
            {
                Direction = 1;
            }
            if (inpDir < 0)
            {
                Direction = -1;
            }
        }

        render.flipX = Direction < 0;
        #endregion
        #region Change Animation
        if (Animation == 0)
        {
            animator.Play("Stopped");
        }
        if (Animation == 1)
        {
            animator.Play("Walking");
        }
        if (Animation == 1.25f)
        {
            animator.Play("Air Walking");
        }
        if (Animation == 1.5f)
        {
            animator.Play("Jog");
        }
        if (Animation == 2)
        {
            if (Mathf.Abs(GroundSpeed) < 12f)
            {
                animator.Play("Running");
            }
            else if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Dash") &&
                     !animator.GetCurrentAnimatorStateInfo(0).IsName("Dash (Loop)"))
            {
                animator.Play("Dash");
            }
        }
        if (Animation == 3)
        {
            animator.Play("Jumping");
        }
        if (Animation == 4)
        {
            if (PlayerInput.KeyUp)
            {
                animator.Play("Stand Up");
            }
            else
            {
                animator.Play("Stand Up (Reverse)");
            }
        }
        if (Animation == 5)
        {
            if (PlayerInput.KeyDown)
            {
                animator.Play("Crouch Down");
            }
            else
            {
                animator.Play("Crouch Down (Reverse)");
            }
        }
        if (Animation == 6)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Skidding") &&
                !animator.GetCurrentAnimatorStateInfo(0).IsName("Skidding (Loop)"))
            {
                animator.Play("Skidding");
            }
        }
        if (Animation == 6.5f)
        {
            animator.Play("Skidding (Turn)");
        }
        if (Animation == 7)
        {
            animator.Play("Spindash");
        }
        if (Animation == 8)
        {
            animator.Play("Jumping");
        }
        if (Animation == 9)
        {
            animator.Play("Hurt");
        }
        if (Animation == 10)
        {
            animator.Play("Die");
        }
        if (Animation == 11)
        {
            animator.Play("Drown");
        }
        if (Animation == 12)
        {
            animator.Play("Breathe");
        }
        if (Animation == 40)
        {
            if (YSpeed > 0f)
            {
                animator.Play("Springs");
            }
            else
            {
                animator.Play("Air Walking");
            }
        }
        #endregion
        #region Change Animation Speed
        if (Animation != 1 &&
            Animation != 1.25f &&
            Animation != 1.5f &&
            Animation != 2 &&
            Animation != 3 &&
            Animation != 7 &&
            Animation != 8 &&
          !(Animation == 40 && YSpeed <= 0f))
        {
            animator.speed = 1f;
        }
        else
        {
            if (Ground)
            {
                if (Animation == 1 || Animation == 1.5f)
                {
                    animator.speed = (20f + (Mathf.Abs(GroundSpeed) * 3f)) / 20;
                }
                else if (Animation == 2)
                {
                    if (Mathf.Abs(GroundSpeed) < 14f)
                    {
                        animator.speed = (20f + (Mathf.Abs(GroundSpeed) * 4f)) / 30;
                    }
                    else
                    {
                        animator.speed = (30f + (Mathf.Abs(GroundSpeed) * 4f)) / 40;
                    }
                }
            }
            else if (Animation == 1.25f || Animation == 40 && YSpeed <= 0f)
            {
                animator.speed = (20f + (Mathf.Abs(!(Animation == 40 && YSpeed <= 0f) ? GroundSpeed : XSpeed) * 3f)) / 20;
            }

            if (Action == 1 && Animation == 3)
            {
                animator.speed = (25f + (Mathf.Abs(GroundSpeed) * 10f)) / 20;
            }
            if (Action == 5 && Animation == 7)
            {
                animator.speed = (20f + (Mathf.Abs(SpindashRev) * 10f)) / 30;
            }
            if (Action == 6 && Animation == 8)
            {
                animator.speed = (25f + (Mathf.Abs(GroundSpeed) * 10f)) / 20;
            }
        }
        #endregion
        #region Change Angle
        if (Ground)
        {
            if (Mathf.Abs(((GroundAngle - SmoothAngle + 540f) % 360f) - 180f) < 60f && Mathf.Abs(((0f - GroundAngle + 540f) % 360f) - 180f) >= 40f)
            {
                SmoothAngle = ((720f + SmoothAngle) % 360f) + (((((GroundAngle - SmoothAngle + 540f) % 360f) - 180f) * Mathf.Max(0.165f, Mathf.Abs(XSpeed) / MaxXSpeed * 0.8f)) * GameController.DeltaTime);
            }
            else if (Mathf.Abs(((GroundAngle - SmoothAngle + 540f) % 360f) - 180f) < 60f && Mathf.Abs(((0f - GroundAngle + 540f) % 360f) - 180f) < 40f)
            {
                SmoothAngle = ((720f + SmoothAngle) % 360f) + (((((0f - SmoothAngle + 540f) % 360f) - 180f) * Mathf.Max(0.165f, Mathf.Abs(XSpeed) / MaxXSpeed * 0.8f)) * GameController.DeltaTime);
            }
            else if (Mathf.Abs(((GroundAngle - SmoothAngle + 540f) % 360f) - 180f) >= 60f)
            {
                SmoothAngle = (720f + GroundAngle) % 360f;
            }

            AnimationAngle = 0f;

            if (Animation != 8)
            {
                AnimationAngle = SmoothAngle;
            }
        }
        else
        {
            SmoothAngle = 0f;

            if (AnimationAngle < 180f)
            {
                AnimationAngle = Mathf.Max(AnimationAngle - (4f * GameController.DeltaTime), 0f);
            }
            else
            {
                AnimationAngle = Mathf.Min(AnimationAngle + (4f * GameController.DeltaTime), 360f) % 360f;
            }
        }

        Angle = AnimationAngle;
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
                YSpeed *= 0.2f / GameController.DeltaTime;
                XSpeed *= 0.5f / GameController.DeltaTime;
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
                YSpeed *= 1.3f / GameController.DeltaTime;
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
                if (LevelController.GlobalTimer % 60f == 0f)
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
        SpindashDust.transform.position = new Vector3(XPosition - (Mathf.Sin(AnimationAngle * Mathf.Deg2Rad) * -13f), YPosition + (Mathf.Cos(AnimationAngle * Mathf.Deg2Rad) * -13f), 0f);
        SpindashDust.transform.rotation = Quaternion.Euler(0f, 0f, AnimationAngle);
        #endregion
        #region Skidding Dust
        if (Action == 4 && LevelController.GlobalTimer % 4 == 0)
        {
            Dust dust = SceneController.CreateStageObject("Skid Dust",
                XPosition - (Mathf.Sin(AnimationAngle * Mathf.Deg2Rad) * -13f),
                YPosition + (Mathf.Cos(AnimationAngle * Mathf.Deg2Rad) * -13f)) as Dust;
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
                XSpeed = Direction * 10f;
                foreach (Shield shield in Shields)
                {
                    if (shield.ShieldType != global::Shield.Shield_Types.Flame)
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
                    if (shield.ShieldType != global::Shield.Shield_Types.Flame)
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
                XSpeed *= 0.1f / GameController.DeltaTime;
                YSpeed = Mathf.Min(YSpeed, -8f);
                foreach (Shield shield in Shields)
                {
                    if (shield.ShieldType != global::Shield.Shield_Types.Aquatic)
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
                    if (shield.ShieldType != global::Shield.Shield_Types.Aquatic)
                    {
                        continue;
                    }
                    shield.GetComponent<Animator>().Play("Running");
                }
                CurrentAction = Action01_Jump;
            }
            if (Ground)
            {
                ShieldStatus = 0;
                foreach (Shield shield in Shields)
                {
                    if (shield.ShieldType != global::Shield.Shield_Types.Aquatic)
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
    #region Player Tools
    #region Detect Angle
    private void DetectAngle(bool useNormal = false)
    {
        Quadrant = (int)(((GroundAngle + (360f + 45f)) % 360f) / 90f);

        RaycastHit2D angleLeftHit = SensorCast(new Vector2(-WidthRadius + 2f, 0f), Vector2.down, HeightRadius + 12f);
        RaycastHit2D angleRightHit = SensorCast(new Vector2(WidthRadius - 2f, 0f), Vector2.down, HeightRadius + 12f);

        if (angleLeftHit && angleRightHit)
        {
            float numX = angleRightHit.point.x - angleLeftHit.point.x;
            float numY = angleRightHit.point.y - angleLeftHit.point.y;
            float numR = Mathf.Atan2(numY, numX) * Mathf.Rad2Deg;

            GroundAngle = (720f + numR) % 360f;
        }
        else if (useNormal)
        {
            if (!angleLeftHit && angleRightHit)
            {
                float numX = angleRightHit.normal.x;
                float numY = angleRightHit.normal.y;
                float numR = Mathf.Atan2(numX, numY) * -Mathf.Rad2Deg;

                GroundAngle = (720f + numR) % 360f;
            }
            else if (angleLeftHit && !angleRightHit)
            {
                float numX = angleLeftHit.normal.x;
                float numY = angleLeftHit.normal.y;
                float numR = Mathf.Atan2(numX, numY) * -Mathf.Rad2Deg;

                GroundAngle = (720f + numR) % 360f;
            }
        }
        else
        {
            GroundAngle = 90f * Quadrant;
        }
    }
    #endregion
    #endregion
    #region Player Actions
    #region Common Actions
    #region Actions - [00] Common
    public void Action00_Common()
    {
        if (Ground)
        {
            if (Mathf.Abs(GroundSpeed) == 0f)
            {
                Animation = 0;
            }
            else if (Mathf.Abs(GroundSpeed) > 0f && Mathf.Abs(GroundSpeed) < 4f)
            {
                Animation = 1;
            }
            else if (Mathf.Abs(GroundSpeed) >= 4f && Mathf.Abs(GroundSpeed) < 6f)
            {
                Animation = 1.5f;
            }
            else if (Mathf.Abs(GroundSpeed) >= 6f)
            {
                Animation = 2;
            }
        }
        else if (Animation == 1 || Animation == 1.5f)
        {
            Animation = 1.25f;
        }

        if (Ground && Mathf.Abs(GroundSpeed) == 0f && !(PlayerInput.KeyLeft || PlayerInput.KeyRight))
        {
            if (PlayerInput.KeyUp)
            {
                Action = 2;
                CurrentAction = Action02_StandUp;
            }
            if (PlayerInput.KeyDown)
            {
                Action = 3;
                CurrentAction = Action03_CrouchDown;
            }
        }

        if (Quadrant == 0 && Ground && ControlLock == 0)
        {
            if (GroundSpeed >= 4f && PlayerInput.KeyLeft)
            {
                SkidTimer = 20;
                Action = 4;
                AllowDirection = false;
                AllowInput = true;
                Direction = 1;
                Animation = 6;
                AudioController.PlaySFX(Sound_Skidding);
                CurrentAction = Action04_Skidding;
            }
            if (GroundSpeed <= -4f && PlayerInput.KeyRight)
            {
                SkidTimer = 20;
                Action = 4;
                AllowDirection = false;
                AllowInput = true;
                Direction = -1;
                Animation = 6;
                AudioController.PlaySFX(Sound_Skidding);
                CurrentAction = Action04_Skidding;
            }
        }

        if (Ground && Mathf.Abs(GroundSpeed) >= 0.2f && PlayerInput.KeyDown && !(PlayerInput.KeyLeft || PlayerInput.KeyRight))
        {
            Action = 6;
            AllowInput = false;
            Animation = 8;
            AudioController.PlaySFX(Sound_Rolling);
            CurrentAction = Action06_Rolling;
        }

        if (Ground && PlayerInput.KeyActionAPressed)
        {
            XSpeed = (Mathf.Sin(GroundAngle * Mathf.Deg2Rad) * -JumpForce) + (Mathf.Cos(GroundAngle * Mathf.Deg2Rad) * GroundSpeed);
            YSpeed = (Mathf.Cos(GroundAngle * Mathf.Deg2Rad) * JumpForce) + (Mathf.Sin(GroundAngle * Mathf.Deg2Rad) * GroundSpeed);
            Ground = false;
            GroundAngle = 0f;
            Action = 1;
            ControlLock = 0;
            JumpVariable = true;
            AudioController.PlaySFX(Sound_Jump);
            CurrentAction = Action01_Jump;
        }
    }
    #endregion
    #region Actions - [01] Jump
    public void Action01_Jump()
    {
        Animation = 3;

        if (!PlayerInput.KeyActionA && YSpeed > JumpReleaseForce && JumpVariable)
        {
            YSpeed = JumpReleaseForce;
        }

        if (Ground)
        {
            Action = 0;
            AllowDirection = true;
            AllowInput = true;
            CurrentAction = Action00_Common;
        }
    }
    #endregion
    #region Actions - [02] Stand Up
    public void Action02_StandUp()
    {
        Animation = 4;

        if (!Ground || GroundSpeed != 0f || !PlayerInput.KeyUp &&
            animator.GetCurrentAnimatorStateInfo(0).IsName("Stand Up (Reverse)") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            Action = 0;
            AllowDirection = true;
            AllowInput = true;
            CurrentAction = Action00_Common;
        }
    }
    #endregion
    #region Actions - [03] Crouch Down
    public void Action03_CrouchDown()
    {
        Animation = 5;

        if (!Ground || GroundSpeed != 0f || !PlayerInput.KeyDown &&
            animator.GetCurrentAnimatorStateInfo(0).IsName("Crouch Down (Reverse)") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            Action = 0;
            AllowDirection = true;
            AllowInput = true;
            CurrentAction = Action00_Common;
        }

        if (PlayerInput.KeyActionAPressed)
        {
            Action = 5;
            audioSource.Play();
            CurrentAction = Action05_Spindash;
        }
    }
    #endregion
    #region Actions - [04] Skidding
    public void Action04_Skidding()
    {
        if (SkidTimer > 0)
        {
            SkidTimer--;
        }

        if (SkidTimer == 0 && (Mathf.Abs(GroundSpeed) >= 0f && !(PlayerInput.KeyLeft || PlayerInput.KeyRight) ||
            Animation == 6 && Mathf.Abs(GroundSpeed) > 1.5f && (PlayerInput.KeyLeft && GroundSpeed <= 0f || PlayerInput.KeyRight && GroundSpeed >= 0f)) ||
            !Ground)
        {
            Action = 0;
            Animation = 1;
            AllowDirection = true;
            AllowInput = true;
        }

        if (Animation == 6 && (PlayerInput.KeyLeft || PlayerInput.KeyRight) && Ground && Mathf.Abs(GroundSpeed) <= 1.5f)
        {
            Direction *= -1;
            Animation = 6.5f;
        }

        if (Animation == 6.5f &&
            animator.GetCurrentAnimatorStateInfo(0).IsName("Skidding (Turn)") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            Action = 0;
            AllowDirection = true;
            AllowInput = true;
            CurrentAction = Action00_Common;
        }

        if (Ground && Mathf.Abs(GroundSpeed) >= 0.2f && PlayerInput.KeyDown && !(PlayerInput.KeyLeft || PlayerInput.KeyRight))
        {
            Action = 6;
            AllowDirection = false;
            AllowInput = false;
            Animation = 8;
            AudioController.PlaySFX(Sound_Rolling);
            CurrentAction = Action06_Rolling;
        }

        if (Ground && PlayerInput.KeyActionAPressed)
        {
            XSpeed = (Mathf.Sin(GroundAngle * Mathf.Deg2Rad) * -JumpForce) + (Mathf.Cos(GroundAngle * Mathf.Deg2Rad) * GroundSpeed);
            YSpeed = (Mathf.Cos(GroundAngle * Mathf.Deg2Rad) * JumpForce) + (Mathf.Sin(GroundAngle * Mathf.Deg2Rad) * GroundSpeed);
            Ground = false;
            GroundAngle = 0f;
            Action = 1;
            ControlLock = 0;
            AllowInput = true;
            JumpVariable = true;
            AudioController.PlaySFX(Sound_Jump);
            CurrentAction = Action01_Jump;
        }
    }
    #endregion
    #region Actions - [05] Spindash
    public void Action05_Spindash()
    {
        Animation = 7;
        SpindashRev -= ((SpindashRev / 0.125f) / 512f) * GameController.DeltaTime;

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
            Action = 6;
            GroundSpeed += (8f + Mathf.Floor(SpindashRev / 2f)) * Direction;
            AllowDirection = false;
            AllowInput = false;
            Animation = 8;
            CameraController.LagTimer = 16f;
            AudioController.PlaySFX(Sound_Release);
            CurrentAction = Action06_Rolling;
        }
    }
    #endregion
    #region Actions - [06] Rolling
    public void Action06_Rolling()
    {
        SpindashRev = 0f;

        Animation = 8;

        GroundSpeed = Mathf.Max(Mathf.Abs(GroundSpeed) - (RollFriction * GameController.DeltaTime), 0f) * Mathf.Sign(GroundSpeed);

        if (GroundSpeed < 0f && PlayerInput.KeyRight ||
            GroundSpeed > 0f && PlayerInput.KeyLeft)
        {
            GroundSpeed = Mathf.Max(Mathf.Abs(GroundSpeed) - (RollDeceleration * GameController.DeltaTime), 0f) * Mathf.Sign(GroundSpeed);
        }

        if (Ground)
        {
            if (Mathf.Sign(GroundSpeed) == Mathf.Sign(Mathf.Sin(GroundAngle * Mathf.Deg2Rad)))
            {
                GroundSpeed -= SlopeRollUpFactor * Mathf.Sin(GroundAngle * Mathf.Deg2Rad) * GameController.DeltaTime;
            }
            else
            {
                GroundSpeed -= SlopeRollDownFactor * Mathf.Sin(GroundAngle * Mathf.Deg2Rad) * GameController.DeltaTime;
            }
        }

        if (!Ground)
        {
            Action = 1;
            AllowInput = true;
            AllowDirection = true;
            JumpVariable = false;
            CurrentAction = Action01_Jump;
        }

        if (Ground && PlayerInput.KeyActionAPressed)
        {
            XSpeed = (Mathf.Sin(GroundAngle * Mathf.Deg2Rad) * -JumpForce) + (Mathf.Cos(GroundAngle * Mathf.Deg2Rad) * GroundSpeed);
            YSpeed = (Mathf.Cos(GroundAngle * Mathf.Deg2Rad) * JumpForce) + (Mathf.Sin(GroundAngle * Mathf.Deg2Rad) * GroundSpeed);
            Ground = false;
            GroundAngle = 0f;
            Action = 1;
            AllowInput = true;
            AllowDirection = true;
            ControlLock = 0;
            JumpVariable = true;
            Animation = 3;
            AudioController.PlaySFX(Sound_Jump);
            CurrentAction = Action01_Jump;
        }

        if (Mathf.Abs(GroundSpeed) < RollDeceleration)
        {
            Action = 0;
            AllowInput = true;
            AllowDirection = true;
            CurrentAction = Action00_Common;
        }
    }
    #endregion
    #region Actions - [07] Breathe
    public void Action07_Breathe()
    {
        Animation = 12;

        if (Animation == 12 &&
            animator.GetCurrentAnimatorStateInfo(0).IsName("Breathe") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && Ground)
        {
            Action = 0;
            AllowInput = true;
            AllowDirection = true;
        }
    }
    #endregion
    #region Actions - [08] Hurt
    public void Action08_Hurt()
    {
        Animation = 9;

        if (Ground)
        {
            GroundSpeed = 0f;
            Action = 0;
            AllowInput = true;
            AllowDirection = true;
            CurrentAction = Action00_Common;
        }
    }
    #endregion
    #region Actions - [09] Die
    public void Action09_Die()
    {
        Animation = 10;

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
        Animation = 11;

        if (YPosition < SceneController.YBottomFrame - 48f)
        {
            if (!GameController.Preload)
            {
                GameController.Lives--;
                GameController.instance.LoadLevel(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            }
        }
    }
    #endregion
    #endregion
    #region Other Actions
    #region Actions - [40] Springs
    public void Action40_Springs()
    {
        Animation = 40;
        AllowInput = true;
        if (Ground)
        {
            Action = 0;
        }
    }
    #endregion
    #endregion
    #endregion
}
