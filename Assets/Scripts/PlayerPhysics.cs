using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerPhysics : BaseObject
{
    #region Player Values
    #region Player movement values
    [HideInInspector] public int Quadrant;
    [HideInInspector] public float PreviousX;
    [HideInInspector] public float PreviousY;
    [HideInInspector] public float OldXSpeed;
    [HideInInspector] public float OldYSpeed;
    [HideInInspector] public int Action;
    [HideInInspector] public int ControlLock;
    [HideInInspector] public bool Landed;
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
    [HideInInspector] public int Hurt;
    [HideInInspector] public int Direction;
    [HideInInspector] public float Animation;
    [HideInInspector] public float SmoothAngle;
    #endregion
    #region Player input values
    [HideInInspector] public bool KeyUp;
    [HideInInspector] public bool KeyUpPressed;
    [HideInInspector] public bool KeyDown;
    [HideInInspector] public bool KeyDownPressed;
    [HideInInspector] public bool KeyLeft;
    [HideInInspector] public bool KeyLeftPressed;
    [HideInInspector] public bool KeyRight;
    [HideInInspector] public bool KeyRightPressed;
    [HideInInspector] public bool KeyActionA;
    [HideInInspector] public bool KeyActionAPressed;
    #endregion
    #region Player sounds
    [Header("Player Sound Effects")]
    public AudioClip Sound_Jump;
    public AudioClip Sound_Skidding;
    public AudioClip Sound_Rolling;
    public AudioClip Sound_Release;
    public AudioClip Sound_Hurt;
    public AudioClip Sound_LoseRings;
    public AudioClip Sound_FireDash;
    public AudioClip Sound_LightingJump;
    public AudioClip Sound_BubbleBounce;
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
        Shields = FindObjectsOfType<Shield>();

        base.Start();

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

        AllowInput = AllowX = AllowY = AllowLanding = AllowFalling = AllowDirection = true;
    }
    #endregion
    #region Player Update
    private void FixedUpdate()
    {
        #region Player Input
        //Inputs
        KeyUpPressed = Input.GetKey(KeyCode.UpArrow) && !KeyUp;
        KeyUp = Input.GetKey(KeyCode.UpArrow);

        KeyDownPressed = Input.GetKey(KeyCode.DownArrow) && !KeyDown;
        KeyDown = Input.GetKey(KeyCode.DownArrow);

        KeyLeftPressed = Input.GetKey(KeyCode.LeftArrow) && !KeyLeft;
        KeyLeft = Input.GetKey(KeyCode.LeftArrow);

        KeyRightPressed = Input.GetKey(KeyCode.RightArrow) && !KeyRight;
        KeyRight = Input.GetKey(KeyCode.RightArrow);

        KeyActionAPressed = Input.GetKey(KeyCode.Z) && !KeyActionA;
        KeyActionA = Input.GetKey(KeyCode.Z);

        int inpDir = (KeyRight ? 1 : 0) - (KeyLeft ? 1 : 0);
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
            Acceleration = 0.09375f;
            Friction = 0.09375f;
            TopSpeed = 12f;
            AirAcceleration = 0.1875f;
            RollFriction = 0.046875f;
        }
        if (SuperForm)
        {
            Acceleration = 0.1875f;
            Deceleration = 1f;
            TopSpeed = 10f;
            AirAcceleration = 0.375f;
            JumpForce = 8f;
        }
        #endregion
        #region Player Control (Pre)
        #region Control (Pre)
        if (Action != 9)
        {
            #region Slope Factor
            if (AllowInput && Ground && Action != 6)
            {
                //Slip down on the slopes
                GroundSpeed -= SlopeFactor * Mathf.Sin(GroundAngle * Mathf.Deg2Rad);
            }
            #endregion
            #region X Control
            //If the player is on the ground and the input lock is deactivated
            if (AllowInput && !(Ground && ControlLock > 0) && Action != 6)
            {
                //If the player is pressing left
                if (inpDir < 0)
                {
                    //If the player is on the ground
                    if (Ground)
                    {
                        //If moving to the right
                        if (GroundSpeed > 0f)
                        {
                            //Decelerate
                            GroundSpeed -= Deceleration;

                            if (GroundSpeed <= 0f)
                            {
                                //Emulate deceleration quirk
                                GroundSpeed = -0.5f;
                            }
                        }
                        //If moving to the left
                        else if (GroundSpeed > -TopSpeed)
                        {
                            //Accelerate
                            GroundSpeed -= Acceleration;

                            if (GroundSpeed <= -TopSpeed)
                            {
                                //Impose top speed limit
                                GroundSpeed = -TopSpeed;
                            }
                        }
                    }
                    //If moving to the left (while in air)
                    else if (XSpeed > -TopSpeed)
                    {
                        //Accelerate
                        XSpeed -= AirAcceleration;

                        if (XSpeed <= -TopSpeed)
                        {
                            //Impose top speed limit
                            XSpeed = -TopSpeed;
                        }
                    }
                }

                //If the player is pressing right
                if (inpDir > 0)
                {
                    //If the player is on the ground
                    if (Ground)
                    {
                        //If moving to the left
                        if (GroundSpeed < 0f)
                        {
                            //Decelerate
                            GroundSpeed += Deceleration;

                            if (GroundSpeed >= 0f)
                            {
                                //Emulate deceleration quirk
                                GroundSpeed = 0.5f;
                            }
                        }
                        //If moving to the right
                        else if (GroundSpeed < TopSpeed)
                        {
                            //Accelerate
                            GroundSpeed += Acceleration;

                            if (GroundSpeed >= TopSpeed)
                            {
                                //Impose top speed limit
                                GroundSpeed = TopSpeed;
                            }
                        }
                    }
                    //If moving to the right (while in air)
                    else if (XSpeed < TopSpeed)
                    {
                        //Accelerate 
                        XSpeed += AirAcceleration;

                        if (XSpeed >= TopSpeed)
                        {
                            //Impose top speed limit
                            XSpeed = TopSpeed;
                        }
                    }
                }

                //If the player is not pressing left or right
                if (Ground && inpDir == 0)
                {
                    //Decelerate
                    GroundSpeed -= Mathf.Min(Mathf.Abs(GroundSpeed), Friction) * Mathf.Sign(GroundSpeed);

                    //Set speed to 0 when it's very low
                    if (Mathf.Abs(GroundSpeed) < Friction)
                    {
                        GroundSpeed = 0f;
                    }
                }
            }

            //Keep the player inside the level boundaries
            if ((Ground ? GroundSpeed : XSpeed) < 0f && XPosition <= CameraController.CameraMinimumX + 16f)
            {
                if (Ground) GroundSpeed = 0f;
                else XSpeed = 0f;
                XPosition = CameraController.CameraMinimumX + 16f;
            }

            if ((Ground ? GroundSpeed : XSpeed) > 0f && XPosition >= CameraController.CameraMaximumX - 16f)
            {
                if (Ground) GroundSpeed = 0f;
                else XSpeed = 0f;
                XPosition = CameraController.CameraMaximumX - 16f;
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
                            Ground = false;
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
                    ControlLock--;
                }
            }
            #endregion
        }
        #endregion
        #endregion
        #region Player Movement
        #region Start Movement
        //ColliderFloor = ColliderCeiling = ColliderWallLeft = ColliderWallRight = null;

        //Speed limits
        GroundSpeed = Mathf.Clamp(GroundSpeed, -MaxXSpeed, MaxXSpeed);
        XSpeed = Mathf.Clamp(XSpeed, -MaxXSpeed, MaxXSpeed);
        YSpeed = Mathf.Clamp(YSpeed, -MaxYSpeed, MaxYSpeed);

        //Reset land flag
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

        //Move along slopes
        float AddX = 0f;
        float AddY = 0f;

        if (AllowX && Ground)
        {
            XSpeed = GroundSpeed * Mathf.Cos(GroundAngle * Mathf.Deg2Rad);
            YSpeed = GroundSpeed * Mathf.Sin(GroundAngle * Mathf.Deg2Rad);

            AddX = XSpeed;
            AddY = YSpeed;
        }
        else if (!Ground)
        {
            if (AllowX) AddX = XSpeed;
            if (AllowY) AddY = YSpeed;
        }

        PreviousX = XPosition;
        PreviousY = YPosition;

        if (Ground) OldXSpeed = GroundSpeed;
        else OldYSpeed = YSpeed;

        ObjectLoops = (int)(Mathf.Sqrt((XSpeed * XSpeed) + (YSpeed * YSpeed)) + 1f);

        AddX /= ObjectLoops;
        AddY /= ObjectLoops;

        for (int i = 0; i < ObjectLoops; i++)
        {
            //Move
            XPosition += AddX;
            YPosition += AddY;
            #endregion
            #region Wall Collisions
            if (AllowX)
            {
                //Wall Collisions
                RaycastHit2D wallLeftHit = SensorCast(new Vector2(0f, WallShift), Vector2.left, WidthRadius);
                RaycastHit2D wallRightHit = SensorCast(new Vector2(0f, WallShift), Vector2.right, WidthRadius);

                //Left side
                if (wallLeftHit)
                {
                    Vector2 vector = new Vector2(-Mathf.Cos(GroundAngle * Mathf.Deg2Rad), -Mathf.Sin(GroundAngle * Mathf.Deg2Rad)) * (wallLeftHit.distance - WidthRadius);
                    XPosition += vector.x;
                    YPosition += vector.y;
                    if (Ground) GroundSpeed = 0f;
                    else XSpeed = 0f;
                }

                //Right side
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
            #region Floor and Ceiling Collisions / Landing
            if (AllowY && !Ground)
            {
                #region Floor and Ceiling Collisions
                //Ceiling and Floor collisions
                RaycastHit2D floorLeftHit = SensorCast(new Vector2(-WidthRadius + 2f, 0f), Vector2.down, HeightRadius);
                RaycastHit2D floorRightHit = SensorCast(new Vector2(WidthRadius - 2f, 0f), Vector2.down, HeightRadius);
                RaycastHit2D floorHit = default(RaycastHit2D);

                RaycastHit2D ceilingLeftHit = SensorCast(new Vector2(-WidthRadius + 2f, 0f), Vector2.up, HeightRadius);
                RaycastHit2D ceilingRightHit = SensorCast(new Vector2(WidthRadius - 2f, 0f), Vector2.up, HeightRadius);
                RaycastHit2D ceilingHit = default(RaycastHit2D);

                #region Ceiling Collisions
                //If "C" and "D" are colliding
                if (ceilingLeftHit && ceilingRightHit)
                {
                    //If distance of "C" is less than distance of "D"
                    if (ceilingLeftHit.distance < ceilingRightHit.distance)
                    {
                        ceilingHit = ceilingLeftHit;
                    }
                    else
                    {
                        ceilingHit = ceilingRightHit;
                    }
                }
                //If "C" is not colliding but "D" is
                else if (!ceilingLeftHit && ceilingRightHit)
                {
                    ceilingHit = ceilingRightHit;
                }
                //If "C" is colliding but "D" is not
                else if (ceilingLeftHit && !ceilingRightHit)
                {
                    ceilingHit = ceilingLeftHit;
                }

                //If "sensor with shortest distance" is colliding
                if (ceilingHit)
                {
                    YPosition += ceilingHit.distance - HeightRadius;
                    if (CeilingLand == -1)
                    {
                        YSpeed = 0f;
                    }
                }
                #endregion
                #region Floor Collisions
                //If "A" and "B" are colliding
                if (floorLeftHit && floorRightHit)
                {
                    //If distance of "A" is less than distance of "B"
                    if (floorLeftHit.distance < floorRightHit.distance)
                    {
                        floorHit = floorLeftHit;
                    }
                    else
                    {
                        floorHit = floorRightHit;
                    }
                }
                //If "A" is not colliding but "B" is
                else if (!floorLeftHit && floorRightHit)
                {
                    floorHit = floorRightHit;
                }
                //If "A" is colliding but "B" is not
                else if (floorLeftHit && !floorRightHit)
                {
                    floorHit = floorLeftHit;
                }

                //If "sensor with shortest distance" is colliding
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
                    //Detect angle
                    DetectAngle(true);

                    //When landing, add an additional speed.
                    if (GroundAngle >= 45f && GroundAngle <= 315f)
                    {
                        XSpeed += Mathf.Sin(GroundAngle * Mathf.Deg2Rad) * YSpeed;
                    }
                    else if (GroundAngle >= 22.5f && GroundAngle <= 337.5f)
                    {
                        XSpeed += Mathf.Sin(GroundAngle * Mathf.Deg2Rad) * (YSpeed * 0.5f);
                    }

                    //Switch to "Grounded mode"
                    CeilingLand = 0;
                    GroundSpeed = XSpeed;
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
                                CeilingLand = 0;
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
                                    JumpVariable = false;
                                    GroundSpeed = XSpeed;
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
                RaycastHit2D floorLeftHit = SensorCast(new Vector2(-WidthRadius + 2f, 0f), Vector2.down, 32f);
                RaycastHit2D floorRightHit = SensorCast(new Vector2(WidthRadius - 2f, 0f), Vector2.down, 32f);
                RaycastHit2D floorHit = default(RaycastHit2D);

                //If "A" and "B" are colliding
                if (floorLeftHit && floorRightHit)
                {
                    //If distance of "A" is less than distance of "B"
                    if (floorLeftHit.distance < floorRightHit.distance)
                    {
                        floorHit = floorLeftHit;
                    }
                    else
                    {
                        floorHit = floorRightHit;
                    }
                }
                //If "A" is not colliding but "B" is
                else if (!floorLeftHit && floorRightHit)
                {
                    floorHit = floorRightHit;
                }
                //If "A" is colliding but "B" is not
                else if (floorLeftHit && !floorRightHit)
                {
                    floorHit = floorLeftHit;
                }

                //If "sensor with shortest distance" is colliding
                if (floorHit)
                {
                    Vector2 vector = new Vector2(Mathf.Sin(GroundAngle * Mathf.Deg2Rad), -Mathf.Cos(GroundAngle * Mathf.Deg2Rad)) * (floorHit.distance - HeightRadius);
                    XPosition += vector.x;
                    YPosition += vector.y;
                }
                //Fall when not in ground anymore
                else if (AllowFalling)
                {
                    CeilingLand = -1;
                    XSpeed = GroundSpeed * Mathf.Cos(GroundAngle * Mathf.Deg2Rad);
                    YSpeed = GroundSpeed * Mathf.Sin(GroundAngle * Mathf.Deg2Rad);
                    GroundAngle = 0f;
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
        }

        ColliderFloor = BoxCast(new Vector2(0f, -HeightRadius + Mathf.Min(Ground ? 0f : YSpeed, 0f)), new Vector2((WidthRadius - 2f) * 2f, WidthRadius));
        ColliderCeiling = BoxCast(new Vector2(0f, HeightRadius + Mathf.Max(Ground ? 0f : YSpeed, 0f)), new Vector2((WidthRadius - 2f) * 2f, WidthRadius));
        ColliderWallLeft = BoxCast(new Vector2(-WidthRadius + Mathf.Min(Ground ? GroundSpeed : XSpeed, 0f), WallShift), new Vector2(WidthRadius, WidthRadius / 2f));
        ColliderWallRight = BoxCast(new Vector2(WidthRadius + Mathf.Max(Ground ? GroundSpeed : XSpeed, 0f), WallShift), new Vector2(WidthRadius, WidthRadius / 2f));

        if (YPosition < -StageController.CurrentStage.Height)
        {
            Hurt = 2;
        }
        #endregion
        #endregion
        #region Player Control
        #region Control
        #region Y Control
        if (!Ground)
        {
            //Gravity
            YSpeed -= GravityForce;

            //Air drag
            if (YSpeed > 0f && YSpeed < 4f && Mathf.Abs(XSpeed) >= 0.125f)
            {
                XSpeed *= AirDrag;
            }
        }
        #endregion
        #endregion
        #region Actions
        #region Manage Actions
        if (CurrentAction != null)
        {
            Attacking = false;
            CurrentAction();
        }

        if (Action == 0)
        {
            AllowDirection = true;
            AllowInput = true;
            CurrentAction = Action00_Common;
        }
        if (Action == 1)
        {
            Attacking = true;
            AllowDirection = true;
            AllowInput = true;
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
            AllowInput = true;
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
            AllowDirection = true;
            AllowInput = false;
            CurrentAction = Action06_Rolling;
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
        #endregion
        #endregion
        #region Invincibility and Speed Sneakers
        if (Action != 9)
        {
            if (SuperForm)
            {
                Invincibility = 1;
                SpeedSneakers = true;
            }
            else
            {
                if (Invincibility > 0 && InvincibilityTimer > 0)
                {
                    InvincibilityTimer--;
                }
                if (Invincibility > 0 && InvincibilityTimer <= 0)
                {
                    InvincibilityTimer = 0;
                    Invincibility = 0;
                }

                if (SpeedSneakers && SpeedSneakersTimer > 0)
                {
                    SpeedSneakersTimer--;
                }
                if (SpeedSneakers && SpeedSneakersTimer <= 0)
                {
                    SpeedSneakersTimer = 0;
                    SpeedSneakers = false;
                }
            }
        }
        #endregion
        #region Check for Being Hurt
        if (Action != 9)
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
                    SoundManager.PlaySFX(Sound_Hurt);
                    CurrentAction = Action08_Hurt;
                }
                else if (StageController.CurrentStage.Rings > 0)
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
                    StageController.RingLoss(StageController.CurrentStage.Rings, XPosition, YPosition);
                    StageController.CurrentStage.Rings = 0;
                    SoundManager.PlaySFX(Sound_LoseRings);
                    CurrentAction = Action08_Hurt;
                }
                else if (StageController.CurrentStage.Rings == 0)
                {
                    Hurt = 2;
                }
            }
            if (Hurt == 2)
            {
                StageController.STOP = true;
                foreach (BaseObject objRef in StageController.CurrentStage.ObjectList)
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
                SoundManager.PlaySFX(Sound_Hurt);
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
            //Right
            if (inpDir > 0)
            {
                Direction = 1;
            }
            //Left
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
            if (KeyUp)
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
            if (KeyDown)
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
                SmoothAngle = ((720f + SmoothAngle) % 360f) + ((((GroundAngle - SmoothAngle + 540f) % 360f) - 180f) * Mathf.Max(0.165f, Mathf.Abs(XSpeed) / MaxXSpeed * 0.8f));
            }
            else if (Mathf.Abs(((GroundAngle - SmoothAngle + 540f) % 360f) - 180f) < 60f && Mathf.Abs(((0f - GroundAngle + 540f) % 360f) - 180f) < 40f)
            {
                SmoothAngle = ((720f + SmoothAngle) % 360f) + ((((0f - SmoothAngle + 540f) % 360f) - 180f) * Mathf.Max(0.165f, Mathf.Abs(XSpeed) / MaxXSpeed * 0.8f));
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
                AnimationAngle = Mathf.Max(AnimationAngle - 4f, 0f);
            }
            else
            {
                AnimationAngle = Mathf.Min(AnimationAngle + 4f, 360f) % 360f;
            }
        }
        #endregion
        #region Flash!
        if (Action != 8 && Invincibility == 2 && InvincibilityTimer > 0f)
        {
            render.enabled = (StageController.LevelTimer % 6f) < 3f;
        }
        else if (Invincibility != 2)
        {
            render.enabled = true;
        }
        #endregion
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
        if (Action == 4 && StageController.GlobalTimer % 4 == 0)
        {
            Dust dust = StageController.CreateStageObject("Skid Dust",
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
            if (KeyActionAPressed && ShieldStatus == 1 && JumpVariable)
            {
                SoundManager.PlaySFX(Sound_FireDash);
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
            if (KeyActionAPressed && ShieldStatus == 1 && JumpVariable)
            {
                ShieldStatus = 2;
                SoundManager.PlaySFX(Sound_LightingJump);
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
            if (KeyActionAPressed && !Ground && ShieldStatus == 1)
            {
                ShieldStatus = 2;
                XSpeed *= 0.1f;
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
            if (KeyActionAPressed && JumpVariable && !Ground && ShieldStatus == 0 || ShieldStatus == 2 && !Ground && YSpeed > 0f)
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
                SoundManager.PlaySFX(Sound_BubbleBounce);
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
        //Get quadrant
        Quadrant = (int)(((GroundAngle + (360f + 45f)) % 360f) / 90f);

        RaycastHit2D angleLeftHit = SensorCast(new Vector2(-WidthRadius + 2f, 0f), Vector2.down, 32f);
        RaycastHit2D angleRightHit = SensorCast(new Vector2(WidthRadius - 2f, 0f), Vector2.down, 32f);

        //If "A" and "B" are colliding
        if (angleLeftHit && angleRightHit)
        {
            //Calculate angle
            float numX = angleRightHit.point.x - angleLeftHit.point.x;
            float numY = angleRightHit.point.y - angleLeftHit.point.y;
            float numR = Mathf.Atan2(numY, numX) * Mathf.Rad2Deg;

            GroundAngle = (720f + numR) % 360f;
        }
        else if (useNormal)
        {
            if (!angleLeftHit && angleRightHit)
            {
                //Calculate angle
                float numX = angleRightHit.normal.x;
                float numY = angleRightHit.normal.y;
                float numR = Mathf.Atan2(numX, numY) * -Mathf.Rad2Deg;

                GroundAngle = (720f + numR) % 360f;
            }
            else if (angleLeftHit && !angleRightHit)
            {
                //Calculate angle
                float numX = angleLeftHit.normal.x;
                float numY = angleLeftHit.normal.y;
                float numR = Mathf.Atan2(numX, numY) * -Mathf.Rad2Deg;

                GroundAngle = (720f + numR) % 360f;
            }
        }
        //If "A" and/or "B" is not colliding, change the angle based on the quadrant
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

        if (Ground && Mathf.Abs(GroundSpeed) == 0f && !(KeyLeft || KeyRight))
        {
            if (KeyUp)
            {
                Action = 2;
                CurrentAction = Action02_StandUp;
            }
            if (KeyDown)
            {
                Action = 3;
                CurrentAction = Action03_CrouchDown;
            }
        }

        if (Quadrant == 0 && Ground && ControlLock == 0)
        {
            if (GroundSpeed >= 4f && KeyLeft)
            {
                SkidTimer = 20;
                Action = 4;
                AllowDirection = false;
                Direction = 1;
                Animation = 6;
                SoundManager.PlaySFX(Sound_Skidding);
                CurrentAction = Action04_Skidding;
            }
            if (GroundSpeed <= -4f && KeyRight)
            {
                SkidTimer = 20;
                Action = 4;
                AllowDirection = false;
                Direction = -1;
                Animation = 6;
                SoundManager.PlaySFX(Sound_Skidding);
                CurrentAction = Action04_Skidding;
            }
        }

        if (Ground && Mathf.Abs(GroundSpeed) >= 0.2f && (KeyDown || Landed && KeyDown) && !(KeyLeft || KeyRight))
        {
            Action = 6;
            AllowInput = false;
            Animation = 8;
            SoundManager.PlaySFX(Sound_Rolling);
            CurrentAction = Action06_Rolling;
        }

        if (Ground && KeyActionAPressed)
        {
            XSpeed = (Mathf.Sin(GroundAngle * Mathf.Deg2Rad) * -JumpForce) + (Mathf.Cos(GroundAngle * Mathf.Deg2Rad) * GroundSpeed);
            YSpeed = (Mathf.Cos(GroundAngle * Mathf.Deg2Rad) * JumpForce) + (Mathf.Sin(GroundAngle * Mathf.Deg2Rad) * GroundSpeed);
            Ground = false;
            GroundAngle = 0f;
            Action = 1;
            ControlLock = 0;
            JumpVariable = true;
            SoundManager.PlaySFX(Sound_Jump);
            CurrentAction = Action01_Jump;
        }
    }
    #endregion
    #region Actions - [01] Jump
    public void Action01_Jump()
    {
        Animation = 3;

        if (!Input.GetKey(KeyCode.Z) && YSpeed > JumpReleaseForce && JumpVariable)
        {
            YSpeed = JumpReleaseForce;
        }

        if (Ground)
        {
            Action = 0;
            AllowInput = true;
            CurrentAction = Action00_Common;
        }
    }
    #endregion
    #region Actions - [02] Stand Up
    public void Action02_StandUp()
    {
        Animation = 4;

        if (!Ground || GroundSpeed != 0f || !KeyUp &&
            animator.GetCurrentAnimatorStateInfo(0).IsName("Stand Up (Reverse)") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            Action = 0;
            CurrentAction = Action00_Common;
        }
    }
    #endregion
    #region Actions - [03] Crouch Down
    public void Action03_CrouchDown()
    {
        Animation = 5;

        if (!Ground || GroundSpeed != 0f || !KeyDown &&
            animator.GetCurrentAnimatorStateInfo(0).IsName("Crouch Down (Reverse)") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            Action = 0;
            CurrentAction = Action00_Common;
        }

        if (KeyActionAPressed)
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
        if (SkidTimer > 0) SkidTimer--;

        if (SkidTimer == 0 && (Mathf.Abs(GroundSpeed) >= 0f && !(KeyLeft || KeyRight) ||
            Animation == 6 && Mathf.Abs(GroundSpeed) > 1.5f && (KeyLeft && GroundSpeed <= 0f || KeyRight && GroundSpeed >= 0f)) ||
            !Ground)
        {
            Action = 0;
            Animation = 1;
            AllowInput = true;
        }

        if (Animation == 6 && (KeyLeft || KeyRight) && Ground && Mathf.Abs(GroundSpeed) <= 1.5f)
        {
            Direction *= -1;
            Animation = 6.5f;
        }

        if (Animation == 6.5f &&
            animator.GetCurrentAnimatorStateInfo(0).IsName("Skidding (Turn)") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            Action = 0;
            AllowInput = true;
            CurrentAction = Action00_Common;
        }

        if (Ground && Mathf.Abs(GroundSpeed) >= 0.2f && (KeyDownPressed || Landed && KeyDown))
        {
            Action = 6;
            AllowInput = false;
            Animation = 8;
            SoundManager.PlaySFX(Sound_Rolling);
            CurrentAction = Action06_Rolling;
        }

        if (Ground && KeyActionAPressed)
        {
            XSpeed = (Mathf.Sin(GroundAngle * Mathf.Deg2Rad) * -JumpForce) + (Mathf.Cos(GroundAngle * Mathf.Deg2Rad) * GroundSpeed);
            YSpeed = (Mathf.Cos(GroundAngle * Mathf.Deg2Rad) * JumpForce) + (Mathf.Sin(GroundAngle * Mathf.Deg2Rad) * GroundSpeed);
            Ground = false;
            GroundAngle = 0f;
            Action = 1;
            ControlLock = 0;
            AllowInput = true;
            JumpVariable = true;
            SoundManager.PlaySFX(Sound_Jump);
            CurrentAction = Action01_Jump;
        }
    }
    #endregion
    #region Actions - [05] Spindash
    public void Action05_Spindash()
    {
        Animation = 7;
        SpindashRev -= ((SpindashRev / 0.125f) / 512f);

        if (SpindashRev <= 0.1f)
        {
            SpindashRev = 0f;
        }

        if (KeyActionAPressed)
        {
            animator.Play("Spindash", -1, 0f);
            SpindashRev += 2f;
            audioSource.Stop();
            audioSource.Play();
        }

        if (!KeyDown)
        {
            Action = 6;
            GroundSpeed += (8f + Mathf.Floor(SpindashRev / 2f)) * Direction;
            AllowInput = false;
            Animation = 8;
            CameraController.LagTimer = 16f;
            SoundManager.PlaySFX(Sound_Release);
            CurrentAction = Action06_Rolling;
        }
    }
    #endregion
    #region Actions - [06] Rolling
    public void Action06_Rolling()
    {
        SpindashRev = 0f;

        Animation = 8;

        GroundSpeed = Mathf.Max(Mathf.Abs(GroundSpeed) - RollFriction, 0f) * Mathf.Sign(GroundSpeed);

        if (GroundSpeed < 0f && KeyRight ||
            GroundSpeed > 0f && KeyLeft)
        {
            GroundSpeed = Mathf.Max(Mathf.Abs(GroundSpeed) - RollDeceleration, 0f) * Mathf.Sign(GroundSpeed);
        }

        if (Ground)
        {
            if (Mathf.Sign(GroundSpeed) == Mathf.Sign(Mathf.Sin(GroundAngle * Mathf.Deg2Rad)))
            {
                GroundSpeed -= SlopeRollUpFactor * Mathf.Sin(GroundAngle * Mathf.Deg2Rad);
            }
            else
            {
                GroundSpeed -= SlopeRollDownFactor * Mathf.Sin(GroundAngle * Mathf.Deg2Rad);
            }
        }

        if (!Ground)
        {
            Action = 1;
            AllowInput = true;
            JumpVariable = false;
            CurrentAction = Action01_Jump;
        }

        if (Ground && KeyActionAPressed)
        {
            XSpeed = (Mathf.Sin(GroundAngle * Mathf.Deg2Rad) * -JumpForce) + (Mathf.Cos(GroundAngle * Mathf.Deg2Rad) * GroundSpeed);
            YSpeed = (Mathf.Cos(GroundAngle * Mathf.Deg2Rad) * JumpForce) + (Mathf.Sin(GroundAngle * Mathf.Deg2Rad) * GroundSpeed);
            Ground = false;
            GroundAngle = 0f;
            Action = 1;
            AllowInput = false;
            ControlLock = 0;
            JumpVariable = true;
            Animation = 3;
            SoundManager.PlaySFX(Sound_Jump);
            CurrentAction = Action01_Jump;
        }

        if (Mathf.Abs(GroundSpeed) < RollDeceleration)
        {
            Action = 0;
            AllowInput = true;
            CurrentAction = Action00_Common;
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
            CurrentAction = Action00_Common;
        }
    }
    #endregion
    #region Actions - [09] Die
    public void Action09_Die()
    {
        Animation = 10;

        if (YPosition < PixelCamera.YBottomFrame - 32f)
        {
            GameController.Lives--;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
