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

    private PlayerPhysics player;
    private Attacher attacher;

    public override void ObjectCreated()
    {
        Life = 0;
        base.ObjectCreated();
    }

    private new void Start()
    {
        player = FindObjectOfType<PlayerPhysics>();
        attacher = GetComponent<Attacher>();

        Acceleration = 0.7f;
        TopSpeed = Acceleration * 10f;

        base.Start();

        Rect.WidthRadius = 8f;
        Rect.HeightRadius = 8f;
    }

    public void FixedUpdate()
    {
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
                YSpeed += Acceleration * GameController.DeltaTime;
            }
            else if (YPosition > player.YPosition && YSpeed > -TopSpeed)
            {
                YSpeed -= Acceleration * GameController.DeltaTime;
            }

            if (XPosition > player.XPosition && XSpeed > -TopSpeed)
            {
                XSpeed -= Acceleration * GameController.DeltaTime;
            }
            else if (XPosition < player.XPosition && XSpeed < TopSpeed)
            {
                XSpeed += Acceleration * GameController.DeltaTime;
            }
        }

        if (Flag0 && player.Shield != 3)
        {
            Flag1 = true;
        }

        if (Flag1 && YSpeed > -TopSpeed * 2f)
        {
            XSpeed *= 0.95f / GameController.DeltaTime;
            YSpeed -= Acceleration * GameController.DeltaTime;
        }

        ProcessMovement();

        if (MovementActivated)
        {
            YSpeed -= 0.09375f * GameController.DeltaTime;
            for (int i = 0; i < ObjectLoops; i++)
            {
                Vector2 top = new Vector2(XPosition, YPosition + Mathf.Max(YSpeed + 8f, 8f));
                Vector2 bottom = new Vector2(XPosition, YPosition + Mathf.Min(YSpeed - 8f, -8f));
                Vector2 left = new Vector2(XPosition + Mathf.Min(XSpeed - 8f, -8f), YPosition);
                Vector2 right = new Vector2(XPosition + Mathf.Max(XSpeed + 8f, 8f), YPosition);

                if (XSpeed >= 0f && Utils.PointCast(this, right))
                {
                    XSpeed = Mathf.Min(-XSpeed, -2f);
                }
                if (XSpeed <= 0f && Utils.PointCast(this, left))
                {
                    XSpeed = Mathf.Max(-XSpeed, 2f);
                }

                if (YSpeed <= 0f && Utils.PointCast(this, bottom))
                {
                    YSpeed = Mathf.Max(-YSpeed, 2f);
                    Ground = true;
                }
                if (!Ground && YSpeed >= 0f && Utils.PointCast(this, top))
                {
                    YSpeed = Mathf.Min(-YSpeed, -2f);
                }

                Ground = false;
            }

            Life += GameController.DeltaTime;

            if (Life >= 312f)
            {
                MovementActivated = false;
                SceneController.DestroyStageObject(this);
            }
        }

        Rect.XPosition = XPosition;
        Rect.YPosition = YPosition;

        if (player.Action != 8 && player.Action != 9 && Utils.AABB(Rect, player.Rect))
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
