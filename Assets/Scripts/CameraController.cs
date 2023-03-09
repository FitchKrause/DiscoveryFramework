using UnityEngine;
using System.Collections;

public class CameraController : PixelCamera
{
    public static float CameraX;
    public static float CameraY;
    public float CameraShiftX;
    public float CameraShiftY;
    public float CameraShiftXSpeed;
    public float CameraShiftYSpeed;
    public static float LagTimer;
    public static float ShakeTimer;
    public static int CameraMode;
    public static int CameraAction;
    public static float CameraMinimumX;
    public static float CameraMinimumY;
    public static float CameraMaximumX;
    public static float CameraMaximumY;
    public static float HardShakeTimer;
    public float LookUpTimer;
    public float CrouchDownTimer;

    private PlayerPhysics player;
    private bool init;

    private void Awake()
    {
        CameraMode = CameraAction = 0;
    }

    private new void Start()
    {
        player = FindObjectOfType<PlayerPhysics>();

        base.Start();

        CameraMaximumX = StageController.CurrentStage.Width;
        CameraMaximumY = StageController.CurrentStage.Height;
    }

    private void FixedUpdate()
    {
        if (!init && player.enabled)
        {
            CameraX = player.XPosition;
            CameraY = -player.YPosition;
            CameraX += 0.1f * (player.XPosition - CameraX);
            CameraY += 0.1f * (-player.YPosition - CameraY);
            init = true;
        }

        if (CameraAction == 0)
        {
            CameraMinimumX = 0f;
            CameraMinimumY = 0f;
            CameraMaximumX = StageController.CurrentStage.Width;
            CameraMaximumY = StageController.CurrentStage.Height;
        }
        if (CameraAction == 3)
        {
            CameraShiftX = CameraShiftY = 0f;
        }

        if (StageController.Paused) return;

        if (LagTimer == 0f && CameraMode == 0)
        {
            if (CameraX < CameraMaximumX - WindowMidWidth && player.XPosition > CameraX + 8f - CameraShiftX - CameraShiftXSpeed)
            {
                CameraX += Mathf.Min(16f, player.XPosition - CameraX - 8f + CameraShiftXSpeed + CameraShiftX);
            }
            else if (CameraX > CameraMinimumX + WindowMidWidth && player.XPosition < CameraX - 8f - CameraShiftX - CameraShiftXSpeed)
            {
                CameraX += Mathf.Max(-16f, player.XPosition - CameraX + 8f + CameraShiftXSpeed + CameraShiftX);
            }
        }

        if (CameraMode == 0)
        {
            if (player.Ground)
            {
                if (CameraY > CameraMinimumY + WindowMidHeight && -player.YPosition < CameraY + CameraShiftY + CameraShiftYSpeed)
                {
                    CameraY += Mathf.Max(-Mathf.Max(3f, Mathf.Abs(player.GroundSpeed * Mathf.Sin(player.GroundAngle * Mathf.Deg2Rad))) - 1f, -player.YPosition - CameraY - CameraShiftY - CameraShiftYSpeed);
                }
                else if (CameraY < CameraMaximumY - WindowMidHeight && -player.YPosition > CameraY + CameraShiftY + CameraShiftYSpeed)
                {
                    CameraY += Mathf.Min(Mathf.Max(3f, Mathf.Abs(player.GroundSpeed * Mathf.Sin(player.GroundAngle * Mathf.Deg2Rad) * 2f)) - 1f, -player.YPosition - CameraY - CameraShiftY - CameraShiftYSpeed);
                }
            }
            else
            {
                if (CameraY > CameraMinimumY + WindowMidHeight && -player.YPosition < CameraY - 48f + CameraShiftY + CameraShiftYSpeed)
                {
                    CameraY += Mathf.Max(-16f, -player.YPosition - CameraY + 48f - CameraShiftY - CameraShiftYSpeed);
                }
            }
            if ((player.Landed || !player.Ground && -player.YPosition > CameraY + CameraShiftY + CameraShiftYSpeed + 24f) && CameraY < CameraMaximumY - WindowMidHeight)
            {
                CameraY += Mathf.Max(0f, Mathf.Min(16f, -player.YPosition - CameraY - CameraShiftY - 24f) - CameraShiftYSpeed);
            }

            if (player.Action == 2 && LookUpTimer < 110f)
            {
                LookUpTimer += 1f;
            }

            if (LookUpTimer == 0f && CameraShiftYSpeed >= 2f)
            {
                CameraShiftYSpeed -= 2f;
            }
            if (LookUpTimer >= 110f)
            {
                CameraShiftYSpeed = Mathf.Min(88f, CameraShiftYSpeed + 2f);
            }

            if (player.Action != 2)
            {
                LookUpTimer = 0f;
            }

            if (player.Action == 3 && CrouchDownTimer < 110f)
            {
                CrouchDownTimer += 1f;
            }

            if (CrouchDownTimer == 0f && CameraShiftYSpeed <= -2f)
            {
                CameraShiftYSpeed += 2f;
            }
            if (CrouchDownTimer >= 110f)
            {
                CameraShiftYSpeed = Mathf.Max(-88f, CameraShiftYSpeed - 2f);
            }

            if (player.Action != 3)
            {
                CrouchDownTimer = 0f;
            }
        }

        if (LagTimer <= 0f)
        {
            LagTimer = 0f;
        }

        if (LagTimer > 0f && CameraMode == 0)
        {
            LagTimer -= 1f;
        }
        else if (CameraMode > 0)
        {
            LagTimer = 0f;
        }

        if (CameraX < CameraMinimumX + WindowMidWidth)
        {
            CameraX = Mathf.Min(CameraX + 2f, CameraMinimumX + WindowMidWidth);
        }
        else if (CameraX > CameraMaximumX - WindowMidWidth)
        {
            CameraX = Mathf.Max(CameraX - 2f, CameraMaximumX - WindowMidWidth);
        }

        if (CameraY < CameraMinimumY + WindowMidHeight)
        {
            CameraY = Mathf.Min(CameraY + 2f, CameraMinimumY + WindowMidHeight);
        }
        else if (CameraY > CameraMaximumY - WindowMidHeight)
        {
            CameraY = Mathf.Max(CameraY - 2f, CameraMaximumY - WindowMidHeight);
        }

        Camera.main.transform.position = new Vector3(CameraX, -CameraY, -10f);

        if (CameraAction == 0 && ShakeTimer > 0f)
        {
            CameraX += Random.Range(-4f, 4f);
            CameraY += Random.Range(-4f, 4f);
            ShakeTimer -= 1f;
        }

        if (CameraAction == 0 && HardShakeTimer > 0f)
        {
            CameraX += Random.Range(0, (int)HardShakeTimer) - Random.Range(0, (int)HardShakeTimer);
            CameraY += Random.Range(0, (int)HardShakeTimer) - Random.Range(0, (int)HardShakeTimer);
            HardShakeTimer -= 1f;
        }
    }

    private new void LateUpdate()
    {
        Camera.main.transform.position = new Vector3()
        {
            x = Mathf.Clamp(Mathf.Floor(Camera.main.transform.position.x), WindowMidWidth, StageController.CurrentStage.Width - WindowMidWidth),
            y = Mathf.Clamp(Mathf.Floor(Camera.main.transform.position.y), -StageController.CurrentStage.Height + WindowMidHeight, -WindowMidHeight),
            z = -10f
        };

        base.LateUpdate();
    }
}
