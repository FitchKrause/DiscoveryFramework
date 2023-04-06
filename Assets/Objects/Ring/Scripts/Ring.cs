using UnityEngine;
using System.Collections;

public class Ring : BaseObject
{
    [HideInInspector]
    public HitBox Rect;

    [Header("Ring Values")]
    public float Acceleration;
    public float TopSpeed;
    public bool Flag0;
    public bool Flag1;
    public bool MovementActivated;
    public float Life;

    public AudioClip Sound_Ring;

    private Attacher attacher;

    public override void ObjectCreated()
    {
        Life = 0;
        base.ObjectCreated();
    }

    private new void Start()
    {
        attacher = GetComponent<Attacher>();

        Acceleration = 0.7f;
        TopSpeed = Acceleration * 10f;

        base.Start();

        Rect.WidthRadius = WidthRadius = PushRadius = 8f;
        Rect.HeightRadius = HeightRadius = 8f;
    }

    public void FixedUpdate()
    {
        PlayerPhysics player = SceneController.FindStageObject("PlayerPhysics") as PlayerPhysics;

        if (player.Shield == 3 && !Flag0 &&
            player.XPosition > XPosition - 100f &&
            player.XPosition < XPosition + 100f &&
            player.YPosition > YPosition - 100f &&
            player.YPosition < YPosition + 100f)
        {
            Flag0 = true;
            if (attacher != null)
            {
                attacher.Attached = false;
                attacher.LinkedPlatform = null;
            }
        }

        if (Flag0 && !Flag1)
        {
            if (YPosition < player.YPosition && YSpeed < TopSpeed)
            {
                YSpeed += Acceleration * Time.timeScale;
            }
            else if (YPosition > player.YPosition && YSpeed > -TopSpeed)
            {
                YSpeed -= Acceleration * Time.timeScale;
            }

            if (XPosition > player.XPosition && XSpeed > -TopSpeed)
            {
                XSpeed -= Acceleration * Time.timeScale;
            }
            else if (XPosition < player.XPosition && XSpeed < TopSpeed)
            {
                XSpeed += Acceleration * Time.timeScale;
            }
        }

        if (Flag0 && player.Shield != 3)
        {
            Flag1 = true;
        }

        if (Flag1 && YSpeed > -TopSpeed * 2f)
        {
            XSpeed -= (XSpeed - (XSpeed * 0.95f)) * Time.timeScale;
            YSpeed -= Acceleration * Time.timeScale;
        }

        AllowCollision = MovementActivated;

        ProcessMovement();

        if (MovementActivated)
        {
            YSpeed -= 0.09375f * Time.timeScale;

            for (int i = 0; i < ObjectLoops; i++)
            {
                if (XSpeed >= 0f && ColliderWallRight)
                {
                    XSpeed = Mathf.Min(-XSpeed, -2f);
                }
                if (XSpeed <= 0f && ColliderWallLeft)
                {
                    XSpeed = Mathf.Max(-XSpeed, 2f);
                }

                if (YSpeed <= 0f && ColliderFloor)
                {
                    YSpeed = Mathf.Max(-YSpeed, 2f);
                }
                if (YSpeed >= 0f && ColliderCeiling)
                {
                    YSpeed = Mathf.Min(-YSpeed, -2f);
                }

                Ground = false;
            }

            Life += Time.timeScale;

            if (Life >= 312f)
            {
                MovementActivated = false;
                SceneController.DestroyStageObject(this);
            }
        }

        Rect.XPosition = XPosition;
        Rect.YPosition = YPosition;

        if (!(player.PlayerAction == new ObjectState(player.Action08_Hurt) || player.PlayerAction == new ObjectState(player.Action09_Die) || player.PlayerAction == new ObjectState(player.Action10_Drown)) && HitBox.AABB(Rect, player.Rect))
        {
            GameController.Score += 10;
            AudioController.PlaySFX(Sound_Ring);
            LevelController.CurrentLevel.Rings++;
            for (int i = 0; i < 3; i++)
            {
                float PosX = XPosition + (Random.Range(0f, 16f) - Random.Range(0f, 16f));
                float PosY = YPosition + (Random.Range(0f, 16f) - Random.Range(0f, 16f));
                Sparkle sparkle = SceneController.CreateStageObject("Ring Sparkle", PosX, PosY) as Sparkle;
                sparkle.XPosition = PosX;
                sparkle.YPosition = PosY;
            }
            SceneController.DestroyStageObject(this);
        }
    }
}
