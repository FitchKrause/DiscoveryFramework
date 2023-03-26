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

    private new void Start()
    {
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
        DifferenceX = XPosition;
        DifferenceY = YPosition;

        AngleX += PlatformXSpeed * Time.timeScale;
        AngleY += PlatformYSpeed * Time.timeScale;

        XPosition = InitialX + (Mathf.Cos(AngleX * Mathf.Deg2Rad) * DistanceX);
        YPosition = InitialY + ((Mathf.Sin(AngleY * Mathf.Deg2Rad) * DistanceY) + (Mathf.Sin(SinkCount * Mathf.Deg2Rad) * -10f));

        DifferenceX -= XPosition;
        DifferenceY -= YPosition;

        if (Sinkable && SinkCount > 0f)
        {
            SinkCount = Mathf.Max(0, SinkCount - (6f * Time.timeScale));
        }

        PlayerPhysics player = SceneController.FindStageObject("PlayerPhysics") as PlayerPhysics;

        if (player.Ground && player.ColliderFloor == ColliderBody)
        {
            player.XPosition -= DifferenceX;
            player.YPosition -= DifferenceY;
            CameraController.CameraX -= DifferenceX;
            CameraController.CameraY -= DifferenceY;
            if (Sinkable) SinkCount = Mathf.Min(90f, SinkCount + (9f * Time.timeScale));
        }
    }
}
