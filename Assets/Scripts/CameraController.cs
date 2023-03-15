using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
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
    private GoalSign GoalPost;

    private void Awake()
    {
        CameraMode = CameraAction = 0;
    }

    private void Start()
    {
        player = FindObjectOfType<PlayerPhysics>();
        GoalPost = FindObjectOfType<GoalSign>();

        CameraMaximumX = LevelController.CurrentLevel.Width;
        CameraMaximumY = LevelController.CurrentLevel.Height;

        CameraX = player.XPosition;
        CameraY = -player.YPosition;
        CameraX += 0.1f * (player.XPosition - CameraX);
        CameraY += 0.1f * (-player.YPosition - CameraY);
    }

    private void FixedUpdate()
    {
        if (CameraAction == 0)
        {
            CameraMinimumX = 0f;
            CameraMinimumY = 0f;
            CameraMaximumX = LevelController.CurrentLevel.Width;
            CameraMaximumY = LevelController.CurrentLevel.Height;
        }
        if (CameraAction == 2)
        {
            CameraMinimumX = GoalPost.XPosition - SceneController.WindowMidWidth;
            CameraMinimumY = -GoalPost.YPosition - 160;
            CameraMaximumX = GoalPost.XPosition + SceneController.WindowMidWidth;
            CameraMaximumY = -GoalPost.YPosition + 80;
        }
        if (CameraAction == 3)
        {
            CameraShiftX = CameraShiftY = 0f;
        }

        if (LevelController.Paused) return;

        if (LagTimer == 0f && CameraMode == 0)
        {
            if (CameraX < CameraMaximumX - SceneController.WindowMidWidth && player.XPosition > CameraX + 8f - CameraShiftX - CameraShiftXSpeed)
            {
                CameraX += Mathf.Min(16f, player.XPosition - CameraX - 8f + CameraShiftXSpeed + CameraShiftX) * GameController.DeltaTime;
            }
            else if (CameraX > CameraMinimumX + SceneController.WindowMidWidth && player.XPosition < CameraX - 8f - CameraShiftX - CameraShiftXSpeed)
            {
                CameraX += Mathf.Max(-16f, player.XPosition - CameraX + 8f + CameraShiftXSpeed + CameraShiftX) * GameController.DeltaTime;
            }
        }

        if (CameraMode == 0)
        {
            if (player.Ground)
            {
                if (CameraY > CameraMinimumY + SceneController.WindowMidHeight && -player.YPosition < CameraY + CameraShiftY + CameraShiftYSpeed)
                {
                    CameraY += Mathf.Max(-Mathf.Max(3f, Mathf.Abs(player.GroundSpeed * Mathf.Sin(player.GroundAngle * Mathf.Deg2Rad))) - 1f, -player.YPosition - CameraY - CameraShiftY - CameraShiftYSpeed) * GameController.DeltaTime;
                }
                else if (CameraY < CameraMaximumY - SceneController.WindowMidHeight && -player.YPosition > CameraY + CameraShiftY + CameraShiftYSpeed)
                {
                    CameraY += Mathf.Min(Mathf.Max(3f, Mathf.Abs(player.GroundSpeed * Mathf.Sin(player.GroundAngle * Mathf.Deg2Rad) * 2f)) - 1f, -player.YPosition - CameraY - CameraShiftY - CameraShiftYSpeed) * GameController.DeltaTime;
                }
            }
            else
            {
                if (CameraY > CameraMinimumY + SceneController.WindowMidHeight && -player.YPosition < CameraY - 48f + CameraShiftY + CameraShiftYSpeed)
                {
                    CameraY += Mathf.Max(-16f, -player.YPosition - CameraY + 48f - CameraShiftY - CameraShiftYSpeed) * GameController.DeltaTime;
                }
            }
            if ((player.Landed || !player.Ground && -player.YPosition > CameraY + CameraShiftY + CameraShiftYSpeed + 24f) && CameraY < CameraMaximumY - SceneController.WindowMidHeight)
            {
                CameraY += Mathf.Max(0f, Mathf.Min(16f, -player.YPosition - CameraY - CameraShiftY - 24f) - CameraShiftYSpeed) * GameController.DeltaTime;
            }

            if (player.Action == 2 && LookUpTimer < 110f)
            {
                LookUpTimer += GameController.DeltaTime;
            }

            if (LookUpTimer == 0f && CameraShiftYSpeed >= 2f)
            {
                CameraShiftYSpeed -= 2f * GameController.DeltaTime;
            }
            if (LookUpTimer >= 110f)
            {
                CameraShiftYSpeed = Mathf.Min(88f, CameraShiftYSpeed + (2f * GameController.DeltaTime));
            }

            if (player.Action != 2)
            {
                LookUpTimer = 0f;
            }

            if (player.Action == 3 && CrouchDownTimer < 110f)
            {
                CrouchDownTimer += GameController.DeltaTime;
            }

            if (CrouchDownTimer == 0f && CameraShiftYSpeed <= -2f)
            {
                CameraShiftYSpeed += 2f * GameController.DeltaTime;
            }
            if (CrouchDownTimer >= 110f)
            {
                CameraShiftYSpeed = Mathf.Max(-88f, CameraShiftYSpeed - (2f * GameController.DeltaTime));
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
            LagTimer -= GameController.DeltaTime;
        }
        else if (CameraMode > 0)
        {
            LagTimer = 0f;
        }

        if (CameraX < CameraMinimumX + SceneController.WindowMidWidth)
        {
            CameraX = Mathf.Min(CameraX + (2f * GameController.DeltaTime), CameraMinimumX + SceneController.WindowMidWidth);
        }
        else if (CameraX > CameraMaximumX - SceneController.WindowMidWidth)
        {
            CameraX = Mathf.Max(CameraX - (2f * GameController.DeltaTime), CameraMaximumX - SceneController.WindowMidWidth);
        }

        if (CameraY < CameraMinimumY + SceneController.WindowMidHeight)
        {
            CameraY = Mathf.Min(CameraY + (2f * GameController.DeltaTime), CameraMinimumY + SceneController.WindowMidHeight);
        }
        else if (CameraY > CameraMaximumY - SceneController.WindowMidHeight)
        {
            CameraY = Mathf.Max(CameraY - (2f * GameController.DeltaTime), CameraMaximumY - SceneController.WindowMidHeight);
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

    private void LateUpdate()
    {
        SceneController.XLeftFrame = Camera.main.transform.position.x - SceneController.WindowMidWidth;
        SceneController.XRightFrame = Camera.main.transform.position.x + SceneController.WindowMidWidth;
        SceneController.YTopFrame = Camera.main.transform.position.y + SceneController.WindowMidHeight;
        SceneController.YBottomFrame = Camera.main.transform.position.y - SceneController.WindowMidHeight;
    }
}
