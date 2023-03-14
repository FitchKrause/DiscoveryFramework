using UnityEngine;
using System.Collections;

public class MovingPlatform : BaseObject
{
    [Header("Moving Platform values")]
    public float PlatformXSpeed;
    public float PlatformYSpeed;
    public float InitialX;
    public float InitialY;
    public float DistanceX;
    public float DistanceY;
    public float AngleX;
    public float AngleY;
    public float DifferenceX;
    public float DifferenceY;

    public float Radius;

    public float SinkCount;
    public bool Sinkable;

    private PlayerPhysics player;

    private new void Start()
    {
        player = FindObjectOfType<PlayerPhysics>();

        foreach (Attacher attacher in FindObjectsOfType<Attacher>())
        {
            if (Vector2.Distance(transform.position, attacher.transform.position) >= Radius)
            {
                continue;
            }
            else
            {
                attacher.Attached = true;
                attacher.LinkedPlatform = this;
            }
        }

        base.Start();

        InitialX = XPosition;
        InitialY = YPosition;
    }

    private void FixedUpdate()
    {
        DifferenceX = Mathf.Floor(XPosition);
        DifferenceY = Mathf.Floor(YPosition);

        AngleX += PlatformXSpeed * GameController.DeltaTime;
        AngleY += PlatformYSpeed * GameController.DeltaTime;

        XPosition = InitialX + (Mathf.Cos(AngleX * Mathf.Deg2Rad) * DistanceX);
        YPosition = InitialY + ((Mathf.Sin(AngleY * Mathf.Deg2Rad) * DistanceY) + (Mathf.Sin(SinkCount * Mathf.Deg2Rad) * -10f));

        DifferenceX -= Mathf.Floor(XPosition);
        DifferenceY -= Mathf.Floor(YPosition);

        if (Sinkable && SinkCount > 0f)
        {
            SinkCount = Mathf.Max(0, SinkCount - 6f);
        }

        if (player.Ground && player.ColliderFloor == ColliderBody)
        {
            if (Sinkable) SinkCount = Mathf.Min(90f, SinkCount + 9f);
            player.XPosition -= DifferenceX;
            player.YPosition -= DifferenceY;
            CameraController.CameraX -= DifferenceX;
            CameraController.CameraY -= DifferenceY;
        }
    }

    private new void LateUpdate()
    {
        transform.position = new Vector3(Mathf.Floor(XPosition), Mathf.Floor(YPosition), transform.position.z);
    }
}
