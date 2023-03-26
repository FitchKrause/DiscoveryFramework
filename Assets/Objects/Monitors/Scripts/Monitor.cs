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

    private HitBox Rect;

    private SpriteRenderer IconObject;
    private SpriteRenderer StaticObject;
    private Animator IconAnimator;
    private Attacher attacher;

    private new void Start()
    {
        Destroyed = false;
        IconLife = 0f;

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
        PlayerPhysics player = SceneController.FindStageObject("PlayerPhysics") as PlayerPhysics;

        AllowCollision = !Ground || (Ground && Landed);

        if (AllowCollision)
        {
            ProcessMovement();
        }

        if (Landed && attacher != null && ColliderFloor != null)
        {
            attacher.LinkedPlatform = ColliderFloor.GetComponent<MovingPlatform>();
            attacher.Attached = attacher.LinkedPlatform != null;
        }

        Rect.XPosition = XPosition + (Ground ? GroundSpeed : XSpeed);
        Rect.YPosition = YPosition + (Ground ? 0f : YSpeed);

        if (!Ground)
        {
            YSpeed = Mathf.Max(YSpeed - (0.2f * Time.timeScale), -10f);
        }

        if (!Destroyed)
        {
            if (player.ColliderCeiling == ColliderBody)
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
                ColliderBody.enabled = false;
                if (attacher != null)
                {
                    attacher.Attached = false;
                    attacher.LinkedPlatform = null;
                }
                GameController.Score += 100;
                IconObject.transform.SetParent(null);
                SceneController.CreateStageObject("Monitor Explosion", XPosition, YPosition);
                render.sprite = MonitorBroken[Random.Range(0, 3)];
                for (int i = 0; i < 4; i++)
                {
                    Debris debris = SceneController.CreateStageObject("Monitor Debris", XPosition, YPosition) as Debris;
                    debris.XPosition = XPosition;
                    debris.YPosition = YPosition;
                    debris.XSpeed = Random.Range(-30f, 30f) / 10f;
                    debris.YSpeed = Random.Range(80f, 20f) / 10f;
                    debris.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 45f) * 8f);
                }
                if (player.Action == 1)
                {
                    player.LandingSpeed = player.YSpeed;
                    player.Ground = false;
                    player.JumpVariable = true;
                    player.YSpeed = player.LandingSpeed * -1.15f;
                }
                AudioController.PlaySFX(Sound_Destroy);
                YSpeed = 2f;
                Ground = false;
                Destroyed = true;
            }
        }
        else
        {
            StaticObject.gameObject.SetActive(false);
            IconLife += Time.timeScale;

            if (IconLife < 32f && GameController.Frame == 1)
            {
                IconObject.transform.position += new Vector3(0f, 1f, 0f);
                SceneController.CreateStageObject("Monitor Trail", IconObject.transform.position.x, IconObject.transform.position.y);
            }
            else if (IconLife >= 64f)
            {
                if (IconLife == 64f)
                {
                    switch (MonitorReward)
                    {
                        case Monitor_Rewards.Rings:
                            LevelController.CurrentLevel.Rings += 10;
                            AudioController.PlaySFX(Sound_Ring);
                            break;
                        case Monitor_Rewards.Shield:
                            player.Shield = 1;
                            AudioController.PlaySFX(Sound_BlueShieldGet);
                            break;
                        case Monitor_Rewards.FlameShield:
                            player.Shield = 2;
                            AudioController.PlaySFX(Sound_FlameShieldGet);
                            break;
                        case Monitor_Rewards.MagneticShield:
                            player.Shield = 3;
                            AudioController.PlaySFX(Sound_MagneticShieldGet);
                            break;
                        case Monitor_Rewards.AquaticShield:
                            player.Shield = 4;
                            AudioController.PlaySFX(Sound_AquaticShieldGet);
                            break;
                        case Monitor_Rewards.Invincibility:
                            if (!player.SuperForm)
                            {
                                player.Invincibility = 1;
                                player.InvincibilityTimer = 2000;
                                MusicController.ToPlay = "Invincible";
                            }
                            break;
                        case Monitor_Rewards.SpeedSneakers:
                            if (!player.SuperForm)
                            {
                                player.SpeedSneakers = true;
                                player.SpeedSneakersTimer = 2000;
                                MusicController.ToPlay = "Speed Up";
                            }
                            break;
                        case Monitor_Rewards.Eggman:
                            player.Hurt = 1;
                            break;
                        case Monitor_Rewards.Life:
                            GameController.Lives++;
                            MusicController.ToPlay = "1-UP";
                            break;
                        case Monitor_Rewards.SuperForm:
                            LevelController.CurrentLevel.Rings += 50;
                            if (!player.SuperForm)
                            {
                                player.SuperForm = true;
                                MusicController.QueuedTime = 0f;
                                MusicController.ToPlay = "Super";
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
