﻿using UnityEngine;
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

    [HideInInspector] public int Hurt;
    [HideInInspector] public int Direction;
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
    
    [HideInInspector] public bool SuperForm;
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
    public AudioClip Sound_Charge;
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
    public GameObject SpindashDust;
    private AudioSource[] audioSources;
    private PlayerSkin[] Skins;
    private Shield[] Shields;
    #endregion
    #region Player Initialization
    private new void Start()
    {
        audioSources = new AudioSource[3];
        for (int i = 0; i < audioSources.Length; i++)
        {
            audioSources[i] = new GameObject(string.Format("AudioSource {0}", i)).AddComponent<AudioSource>();
            audioSources[i].volume = AudioController.SFX_VOLUME * AudioController.MASTER_VOLUME;
        }

        Shields = FindObjectsOfType<Shield>();
        Skins = FindObjectsOfType<PlayerSkin>();

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

        foreach (PlayerSkin skin in Skins)
        {
            skin.transform.SetParent(transform);
            skin.transform.position = transform.position;
            skin.render.sortingOrder = 0;

            if (skin.CharacterID != GameController.GameCharacter || skin.SuperSkin != SuperForm)
            {
                skin.render.enabled = false;
            }
            else
            {
                render = skin.render;
                animator = skin.animator;
            }
        }

        if (YPosition < GameObject.Find("Water Mark").transform.position.y)
        {
            Underwater = true;
        }

        AllowInput = AllowDirection = true;
        Direction = 1;
    }
    #endregion
    #region Player Update
    private void Update()
    {
        #region Stage Music
        if (!SuperForm && Invincibility == 0 && !SpeedSneakers && !(PlayerAction == new ObjectState(Action09_Die) || PlayerAction == new ObjectState(Action10_Drown)) &&
             MusicController.Playing != "Stage" && !(MusicController.Playing == "1-UP" || MusicController.Playing == "Drowning" || MusicController.Playing == "Clear"))
        {
            MusicController.ToPlay = "Stage";
        }
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
    }
    #endregion
    #region Player Update (Fixed)
    private void FixedUpdate()
    {
        #region Player Input
        int inpDir = (PlayerInput.KeyRight ? 1 : 0) - (PlayerInput.KeyLeft ? 1 : 0);
        #endregion
        #region Player Control (Pre)
        #region Control (Pre)
        if (!(PlayerAction == new ObjectState(Action09_Die) || PlayerAction == new ObjectState(Action10_Drown)))
        {
            #region Slope Factor
            if (AllowInput && Ground && PlayerAction != new ObjectState(Action06_Rolling))
            {
                GroundSpeed -= SlopeFactor * Mathf.Sin(GroundAngle * Mathf.Deg2Rad) * Time.timeScale;
            }
            #endregion
            #region X Control
            if (AllowInput && !(Ground && ControlLockTimer > 0f) && PlayerAction != new ObjectState(Action06_Rolling))
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
        #endregion
        #region Invincibility and Speed Sneakers
        if (!(PlayerAction == new ObjectState(Action09_Die) || PlayerAction == new ObjectState(Action10_Drown)))
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
        if (!(PlayerAction == new ObjectState(Action09_Die) || PlayerAction == new ObjectState(Action10_Drown)))
        {
            if (Hurt == 1)
            {
                if (Invincibility != 0)
                {
                    Hurt = 0;
                }
                else if (Shield != 0)
                {
                    XSpeed = -2.2f * (float)Direction;
                    YSpeed = 4.2f;
                    PlayerAction = Action08_Hurt;
                    Ground = false;
                    GroundAngle = 0f;
                    AllowInput = false;
                    Hurt = 0;
                    Invincibility = 2;
                    InvincibilityTimer = 250;
                    Shield = 0;
                    AudioController.PlaySFX(Sound_Hurt);
                }
                else if (LevelController.CurrentLevel.Rings > 0)
                {
                    XSpeed = -2.2f * (float)Direction;
                    YSpeed = 4.2f;
                    PlayerAction = Action08_Hurt;
                    Ground = false;
                    GroundAngle = 0f;
                    AllowInput = false;
                    Hurt = 0;
                    Invincibility = 2;
                    InvincibilityTimer = 250;
                    LevelController.RingLoss(LevelController.CurrentLevel.Rings, XPosition, YPosition);
                    LevelController.CurrentLevel.Rings = 0;
                    AudioController.PlaySFX(Sound_LoseRings);
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
                    if (objRef != this)
                    {
                        objRef.enabled = false;
                    }
                }

                foreach (Animator animator in FindObjectsOfType<Animator>())
                {
                    if (animator != this.animator)
                    {
                        animator.enabled = false;
                    }
                }

                CameraController.CameraMode = 3;
                Ground = false;
                XSpeed = 0f;
                YSpeed = 7.8f;
                PlayerAction = Action09_Die;
                Hurt = 0;
                CollisionLayer = 0;
                AudioController.PlaySFX(Sound_Hurt);
            }
            if (Hurt == 3)
            {
                LevelController.AllowTime = LevelController.AllowPause = false;

                foreach (BaseObject objRef in LevelController.CurrentLevel.ObjectList)
                {
                    if (objRef != this)
                    {
                        objRef.enabled = false;
                    }
                }

                foreach (Animator animator in FindObjectsOfType<Animator>())
                {
                    if (animator != this.animator)
                    {
                        animator.enabled = false;
                    }
                }

                CameraController.CameraMode = 3;
                Ground = false;
                XSpeed = 0f;
                YSpeed = 0f;
                PlayerAction = Action10_Drown;
                Hurt = 0;
                CollisionLayer = 0;
                AudioController.PlaySFX(Sound_Drown);
            }
        }
        #endregion
        #endregion
        #region Player Animations
        #region Change Character
        foreach (PlayerSkin skin in Skins)
        {
            if (skin.CharacterID != GameController.GameCharacter || skin.SuperSkin != SuperForm)
            {
                skin.render.enabled = false;
            }
            else
            {
                skin.render.enabled = true;
                render = skin.render;
                animator = skin.animator;
            }
        }
        #endregion
        #region Change Direction
        render.flipX = (Direction = (AllowDirection && inpDir != 0) ? inpDir : Direction) < 0;
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
        if (PlayerAction != new ObjectState(Action08_Hurt) && Invincibility == 2 && InvincibilityTimer > 0f)
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
        if (!Underwater && !(PlayerAction == new ObjectState(Action09_Die) || PlayerAction == new ObjectState(Action10_Drown)) && LevelController.CurrentLevel.Water && YPosition <= LevelController.CurrentLevel.WaterLevel)
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
        if (Underwater && !(PlayerAction == new ObjectState(Action09_Die) || PlayerAction == new ObjectState(Action10_Drown)) && LevelController.CurrentLevel.Water && YPosition >= LevelController.CurrentLevel.WaterLevel)
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

        if (PlayerAction == new ObjectState(Action09_Die) || PlayerAction == new ObjectState(Action10_Drown))
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
        SpindashDust.SetActive(PlayerAction == new ObjectState(Action05_Spindash));
        SpindashDust.GetComponent<Animator>().speed = this.animator.speed;
        SpindashDust.GetComponent<SpriteRenderer>().flipX = this.render.flipX;
        SpindashDust.GetComponent<SpriteRenderer>().sortingLayerName = this.render.sortingLayerName;
        SpindashDust.transform.position = new Vector3(XPosition - (Mathf.Sin(Angle * Mathf.Deg2Rad) * -13f), YPosition + (Mathf.Cos(Angle * Mathf.Deg2Rad) * -13f), 0f);
        SpindashDust.transform.rotation = Quaternion.Euler(0f, 0f, Angle);
        #endregion
        #region Skidding Dust
        if (PlayerAction == new ObjectState(Action04_Skidding) && LevelController.GlobalTimer % 4 == 0 && GameController.Frame == 1)
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
                PlayerAction = Action01_Jump;
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
        if (Ground)
        {
            if (Mathf.Abs(GroundSpeed) < Acceleration)
            {
                if (CurrentAnimation != "Stopped")
                {
                    CurrentAnimation = "Stopped";
                    animator.speed = 1f;
                    animator.Play("Stopped");
                }
            }
            else if (Mathf.Abs(GroundSpeed) < 4f)
            {
                CurrentAnimation = "Walking";
                animator.speed = 1f + (Mathf.Abs(GroundSpeed) * 0.125f);
                animator.Play("Walking");
            }
            else if (Mathf.Abs(GroundSpeed) < 6f)
            {
                CurrentAnimation = "Jog";
                animator.speed = 1f + (Mathf.Abs(GroundSpeed) * 0.125f);
                animator.Play("Jog");
            }
            else if (Mathf.Abs(GroundSpeed) < 12f)
            {
                CurrentAnimation = "Running";
                animator.Play("Running");
                animator.speed = 1f + (Mathf.Abs(GroundSpeed) * 0.125f);
            }
            else if (CurrentAnimation != "Dash")
            {
                CurrentAnimation = "Dash";
                animator.speed = 1f + (Mathf.Abs(GroundSpeed) * 0.125f);
                animator.Play("Dash");
            }
        }
        else if (CurrentAnimation == "Walking" || CurrentAnimation == "Jog")
        {
            CurrentAnimation = "Air Walking";
            animator.speed = 1f + (Mathf.Abs(GroundSpeed) * 0.125f);
            animator.Play("Air Walking");
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
                Direction = 1;
                CurrentAnimation = "Skidding";
                animator.speed = 1f;
                animator.Play("Skidding");
                AudioController.PlaySFX(Sound_Skidding);
                PlayerAction = Action04_Skidding;
            }
            if (GroundSpeed <= -4f && PlayerInput.KeyRight)
            {
                SkidTimer = 20;
                AllowInput = true;
                AllowDirection = false;
                Direction = -1;
                CurrentAnimation = "Skidding";
                animator.speed = 1f;
                animator.Play("Skidding");
                AudioController.PlaySFX(Sound_Skidding);
                PlayerAction = Action04_Skidding;
            }
        }

        if (Ground && Mathf.Abs(GroundSpeed) >= 0.2f && PlayerInput.KeyDown && !(PlayerInput.KeyLeft || PlayerInput.KeyRight))
        {
            AllowInput = false;
            AllowDirection = true;
            CurrentAnimation = "Jumping";
            animator.speed = 1f + (Mathf.Abs(GroundSpeed) * 0.25f);
            animator.Play("Jumping");
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
            CurrentAnimation = "Jumping";
            animator.speed = 1f + (Mathf.Abs(GroundSpeed) * 0.25f);
            animator.Play("Jumping");
            AudioController.PlaySFX(Sound_Jump);
            PlayerAction = Action01_Jump;
        }
    }
    #endregion
    #region Actions - [01] Jump
    public void Action01_Jump()
    {
        Attacking = true;

        CurrentAnimation = "Jumping";
        animator.speed = 1f + (Mathf.Abs(GroundSpeed) * 0.25f);
        animator.Play("Jumping");

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
        if (PlayerInput.KeyUp)
        {
            animator.Play("Stand Up");
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Stand Up") &&
                 animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            animator.Play("Stand Up (Reverse)");
        }
        CurrentAnimation = "Stand Up";

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
        if (PlayerInput.KeyDown)
        {
            animator.Play("Crouch Down");
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Crouch Down") &&
                 animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            animator.Play("Crouch Down (Reverse)");
        }
        CurrentAnimation = "Crouch Down";

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
            audioSources[0].PlayOneShot(Sound_Charge);
            PlayerAction = Action05_Spindash;
        }
    }
    #endregion
    #region Actions - [04] Skidding
    public void Action04_Skidding()
    {
        if (SkidTimer > 0)
        {
            SkidTimer -= GameController.Frame;
        }

        if (SkidTimer == 0f && (Mathf.Abs(GroundSpeed) >= 0f && !(PlayerInput.KeyLeft || PlayerInput.KeyRight) ||
            CurrentAnimation == "Skidding" && Mathf.Abs(GroundSpeed) > 1.5f &&
           (PlayerInput.KeyLeft && GroundSpeed <= 0f || PlayerInput.KeyRight && GroundSpeed >= 0f)) ||
            !Ground)
        {
            CurrentAnimation = "Walking";
            animator.speed = 1f + (Mathf.Abs(GroundSpeed) * 0.125f);
            animator.Play("Walking");
            AllowDirection = true;
            AllowInput = true;
            PlayerAction = Action00_Common;
        }

        if ((animator.GetCurrentAnimatorStateInfo(0).IsName("Skidding") ||
             animator.GetCurrentAnimatorStateInfo(0).IsName("Skidding (Loop)")) &&
            (PlayerInput.KeyLeft || PlayerInput.KeyRight) && Ground && Mathf.Abs(GroundSpeed) <= 1.5f)
        {
            Direction *= -1;
            CurrentAnimation = "Skid Turn";
            animator.speed = 1f;
            animator.Play("Skidding (Turn)");
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
            CurrentAnimation = "Jumping";
            animator.speed = 1f + (Mathf.Abs(GroundSpeed) * 0.25f);
            animator.Play("Jumping");
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
            CurrentAnimation = "Jumping";
            animator.speed = 1f + (Mathf.Abs(GroundSpeed) * 0.25f);
            animator.Play("Jumping");
            AudioController.PlaySFX(Sound_Jump);
            PlayerAction = Action01_Jump;
        }
    }
    #endregion
    #region Actions - [05] Spindash
    public void Action05_Spindash()
    {
        Attacking = true;

        CurrentAnimation = "Spindash";
        animator.speed = 1f + (Mathf.Abs(SpindashRev) * 0.125f);
        animator.Play("Spindash");

        SpindashRev -= ((SpindashRev / 0.125f) / 256f) * Time.timeScale;

        if (SpindashRev <= 0.1f)
        {
            SpindashRev = 0f;
        }

        if (PlayerInput.KeyActionAPressed)
        {
            animator.Play("Spindash", -1, 0f);
            SpindashRev += 2f;
            audioSources[0].Stop();
            audioSources[0].PlayOneShot(Sound_Charge);
        }

        if (!PlayerInput.KeyDown)
        {
            GroundSpeed += (8f + Mathf.Floor(SpindashRev / 2f)) * (float)Direction;
            AllowDirection = false;
            AllowInput = false;
            animator.Play("Jumping");
            CurrentAnimation = "Jumping";
            CameraController.LagTimer = 16f;
            AudioController.PlaySFX(Sound_Release);
            PlayerAction = Action06_Rolling;
        }
    }
    #endregion
    #region Actions - [06] Rolling
    public void Action06_Rolling()
    {
        Attacking = true;

        SpindashRev = 0f;

        CurrentAnimation = "Jumping";
        animator.speed = 1f + (Mathf.Abs(GroundSpeed) * 0.125f);
        animator.Play("Jumping");

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
            ControlLockTimer = 0;
            AllowInput = true;
            AllowDirection = true;
            JumpVariable = true;
            CurrentAnimation = "Jumping";
            animator.speed = 1f + (Mathf.Abs(GroundSpeed) * 0.25f);
            animator.Play("Jumping");
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
        CurrentAnimation = "Breathe";
        animator.speed = 1f;
        animator.Play("Breathe");

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
        CurrentAnimation = "Hurt";
        animator.speed = 1f;
        animator.Play("Hurt");

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
        AllowCollision = AllowDirection = AllowInput = false;

        CurrentAnimation = "Die";
        animator.speed = 1f;
        animator.Play("Die");

        if (YPosition < GameController.YBottomFrame - 48f)
        {
            GameController.Lives--;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    #endregion
    #region Actions - [10] Drown
    public void Action10_Drown()
    {
        AllowCollision = AllowDirection = AllowInput = false;

        CurrentAnimation = "Drown";
        animator.speed = 1f;
        animator.Play("Drown");

        if (YPosition < GameController.YBottomFrame - 48f)
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
        CurrentAnimation = (YSpeed > 0f) ? "Springs" : "Air Walking";
        animator.speed = 1f;
        animator.Play((YSpeed > 0f) ? "Springs" : "Air Walking");
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
