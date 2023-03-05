using UnityEngine;
using System.Collections;

public class Ring : BaseObject
{
    [HideInInspector]
    public HitBox Rect;

    [Header("Ring Values")]
    public bool MovementActivated;
    public int Life;

    public AudioClip Sound_Ring;

    private PlayerPhysics player;

    public override void ObjectCreated()
    {
        base.ObjectCreated();
        Life = 0;
    }

    private new void Start()
    {
        player = FindObjectOfType<PlayerPhysics>();

        base.Start();

        Rect.WidthRadius = 8f;
        Rect.HeightRadius = 8f;
    }

    public void FixedUpdate()
    {
        ProcessMovement();

        if (MovementActivated)
        {
            YSpeed -= 0.09375f;
            for (int i = 0; i < ObjectLoops; i++)
            {
                Vector2 top = new Vector2(XPosition, YPosition + Mathf.Max(YSpeed + 8f, 8f));
                Vector2 bottom = new Vector2(XPosition, YPosition + Mathf.Min(YSpeed - 8f, -8f));
                Vector2 left = new Vector2(XPosition + Mathf.Min(XSpeed - 8f, -8f), YPosition);
                Vector2 right = new Vector2(XPosition + Mathf.Max(XSpeed + 8f, 8f), YPosition);

                if (XSpeed >= 0f && StageController.PointCast(this, right))
                {
                    XSpeed = Mathf.Min(-XSpeed, -2f);
                }
                if (XSpeed <= 0f && StageController.PointCast(this, left))
                {
                    XSpeed = Mathf.Max(-XSpeed, 2f);
                }

                if (YSpeed <= 0f && StageController.PointCast(this, bottom))
                {
                    YSpeed = Mathf.Max(-YSpeed, 2f);
                    Ground = true;
                }
                if (!Ground && YSpeed >= 0f && StageController.PointCast(this, top))
                {
                    YSpeed = Mathf.Min(-YSpeed, -2f);
                }

                Ground = false;
            }

            Life++;

            if (Life >= 312)
            {
                MovementActivated = false;
                StageController.DestroyStageObject(this);
            }
        }

        Rect.XPosition = XPosition;
        Rect.YPosition = YPosition;

        if (player.Action != 8 && player.Action != 9 && StageController.AABB(Rect, player.Rect))
        {
            //StageController.Score += 10;
            SoundManager.PlaySFX(Sound_Ring);
            StageController.CurrentStage.Rings++;
            /*for (int i = 0; i < 3; i++)
            {
                float PosX = XPosition + (Random.Range(0f, 16f) - Random.Range(0f, 16f));
                float PosY = YPosition + (Random.Range(0f, 16f) - Random.Range(0f, 16f));
                RingSparkle sparkle = StageController.CreateStageObject("RingSparkle", PosX, PosY) as RingSparkle;
                sparkle.XPosition = PosX;
                sparkle.YPosition = PosY;
            }*/
            StageController.DestroyStageObject(this);
        }
    }
}
