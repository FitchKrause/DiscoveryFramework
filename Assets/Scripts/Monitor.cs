using UnityEngine;
using System.Collections;

public class Monitor : BaseObject
{
    [Header("Monitor Values")]
    public Sprite[] MonitorBroken;
    public AudioClip Sound_Destroy;

    public enum Monitor_Rewards
    {
        Rings,
        Shield,
        FlameShield,
        MagneticShield,
        AquaticShield,
        Invincibility,
        SpeedSneakers,
        Eggman,
        Life,
        SuperForm
    }

    public Monitor_Rewards MonitorReward;
    public AudioClip Sound_Ring;
    public AudioClip Sound_BlueShieldGet;
    public AudioClip Sound_FlameShieldGet;
    public AudioClip Sound_MagneticShieldGet;
    public AudioClip Sound_AquaticShieldGet;

    private float IconLife = 0f;
    private bool Destroyed = false;

    private PlayerPhysics player;
    private HitBox Rect;

    private SpriteRenderer IconObject;
    private SpriteRenderer StaticObject;
    private Animator IconAnimator;
    private Attacher attacher;

    private new void Start()
    {
        Destroyed = false;
        IconLife = 0f;

        player = FindObjectOfType<PlayerPhysics>();

        StaticObject = transform.GetChild(0).GetComponent<SpriteRenderer>();
        IconObject = transform.GetChild(1).GetComponent<SpriteRenderer>();
        IconAnimator = IconObject.GetComponent<Animator>();
        attacher = GetComponent<Attacher>();

        base.Start();

        Ground = true;

        WidthRadius = GetComponent<BoxCollider2D>().size.x / 2f;
        HeightRadius = GetComponent<BoxCollider2D>().size.y / 2f;

        Rect.WidthRadius = WidthRadius;
        Rect.HeightRadius = HeightRadius;
    }

    private void FixedUpdate()
    {
        if (!Ground)
        {
            ProcessMovement();
        }

        Rect.XPosition = XPosition + (Ground ? GroundSpeed : XSpeed);
        Rect.YPosition = YPosition + (Ground ? 0f : YSpeed);
        
        if (!Ground)
        {
            for (int i = 0; i < ObjectLoops; i++)
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
                    YSpeed = 0f;
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
                    if (attacher != null)
                    {
                        attacher.LinkedPlatform = floorHit.collider.GetComponent<MovingPlatform>();
                        attacher.Attached = attacher.LinkedPlatform != null;
                    }
                    Ground = true;
                }
                #endregion
                #endregion
            }

            YSpeed = Mathf.Max(YSpeed - 0.2f, -10f);
        }

        if (!Destroyed)
        {
            if (player.ColliderCeiling == ColliderBody && StageController.AABB(Rect, player.Rect))
            {
                if (attacher != null)
                {
                    attacher.Attached = false;
                    attacher.LinkedPlatform = null;
                }
                YSpeed = 2f;
                Ground = false;
            }

            if ((player.Action == 6 &&
                (player.GroundSpeed >= 0f && player.ColliderWallRight == ColliderBody ||
                 player.GroundSpeed <= 0f && player.ColliderWallLeft == ColliderBody)) ||
                 player.Action == 1 && player.YSpeed <= 0f && player.ColliderFloor == ColliderBody)
            {
                if (attacher != null)
                {
                    attacher.Attached = false;
                    attacher.LinkedPlatform = null;
                }
                GameController.Score += 100;
                IconObject.transform.SetParent(null);
                StageController.CreateStageObject("Monitor Explosion", XPosition, YPosition);
                ColliderBody.enabled = false;
                render.sprite = MonitorBroken[Random.Range(0, 3)];
                for (int i = 0; i < 4; i++)
                {
                    Debris debris = StageController.CreateStageObject("Monitor Debris", XPosition, YPosition) as Debris;
                    debris.XPosition = XPosition;
                    debris.YPosition = YPosition;
                    debris.XSpeed = Random.Range(-30f, 30f) / 10f;
                    debris.YSpeed = Random.Range(80f, 20f) / 10f;
                    debris.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 45f) * 8f);
                }
                if (player.Action == 1)
                {
                    player.Ground = false;
                    player.JumpVariable = true;
                    player.YSpeed *= -1.15f;
                }
                SoundManager.PlaySFX(Sound_Destroy);
                YSpeed = 2f;
                Ground = false;
                Destroyed = true;
            }
        }
        else
        {
            StaticObject.gameObject.SetActive(false);
            IconLife += 1f;

            if (IconLife < 32f)
            {
                IconObject.transform.position += new Vector3(0f, 1f, 0f);
                StageController.CreateStageObject("Monitor Trail", IconObject.transform.position.x, IconObject.transform.position.y);
            }
            else if (IconLife >= 64f)
            {
                if (IconLife == 64f)
                {
                    switch (MonitorReward)
                    {
                        case Monitor_Rewards.Rings:
                            StageController.CurrentStage.Rings += 10;
                            SoundManager.PlaySFX(Sound_Ring);
                            break;
                        case Monitor_Rewards.Shield:
                            player.Shield = 1;
                            SoundManager.PlaySFX(Sound_BlueShieldGet);
                            break;
                        case Monitor_Rewards.FlameShield:
                            player.Shield = 2;
                            SoundManager.PlaySFX(Sound_FlameShieldGet);
                            break;
                        case Monitor_Rewards.MagneticShield:
                            player.Shield = 3;
                            SoundManager.PlaySFX(Sound_MagneticShieldGet);
                            break;
                        case Monitor_Rewards.AquaticShield:
                            player.Shield = 4;
                            SoundManager.PlaySFX(Sound_AquaticShieldGet);
                            break;
                        case Monitor_Rewards.Invincibility:
                            if (!player.SuperForm)
                            {
                                player.Invincibility = 1;
                                player.InvincibilityTimer = 2000;
                            }
                            break;
                        case Monitor_Rewards.SpeedSneakers:
                            if (!player.SuperForm)
                            {
                                player.SpeedSneakers = true;
                                player.SpeedSneakersTimer = 2000;
                            }
                            break;
                        case Monitor_Rewards.Eggman:
                            player.Hurt = 1;
                            break;
                        case Monitor_Rewards.Life:
                            GameController.Lives++;
                            /*SoundManager.QueuedMusic = SoundManager.IsPlaying;
                            SoundManager.ToPlay = "1-UP";*/
                            break;
                        case Monitor_Rewards.SuperForm:
                            StageController.CurrentStage.Rings += 50;
                            if (!player.SuperForm)
                            {
                                player.SuperForm = true;
                                /*SoundManager.QueuedMusic = SoundManager.IsPlaying;
                                SoundManager.FadeSpeed = 2f;
                                SoundManager.FadeStop = true;
                                SoundManager.FadeStopCount = 0f;
                                SoundManager.NextMusic = "Super";
                                SoundManager.QueuedPosition = 0f;*/
                            }
                            break;
                    }

                    IconAnimator.Play("Destroy");
                }

                if (IconAnimator.GetCurrentAnimatorStateInfo(0).IsName("Destroy") &&
                    IconAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                {
                    IconObject.gameObject.SetActive(false);
                    IconObject.transform.SetParent(transform);
                }
            }
        }
    }
}
