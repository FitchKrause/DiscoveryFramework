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

    private void Awake()
    {
        CameraMode = CameraAction = 0;
    }

    private void Start()
    {
        PlayerPhysics player = SceneController.FindStageObject("PlayerPhysics") as PlayerPhysics;

        CameraMaximumX = LevelController.CurrentLevel.Width;
        CameraMaximumY = LevelController.CurrentLevel.Height;

        CameraX = player.XPosition;
        CameraY = -player.YPosition;
        CameraX += 0.1f * (player.XPosition - CameraX);
        CameraY += 0.1f * (-player.YPosition - CameraY);

        Camera.main.transform.position = new Vector3()
        {
            x = Mathf.Clamp(CameraX, GameController.WindowMidWidth, SceneController.CurrentScene.Width - GameController.WindowMidWidth),
            y = -Mathf.Clamp(CameraY, GameController.WindowMidHeight, SceneController.CurrentScene.Height - GameController.WindowMidHeight),
            z = -10f
        };

        GameController.XLeftFrame = Camera.main.transform.position.x - GameController.WindowMidWidth;
        GameController.XRightFrame = Camera.main.transform.position.x + GameController.WindowMidWidth;
        GameController.YTopFrame = Camera.main.transform.position.y + GameController.WindowMidHeight;
        GameController.YBottomFrame = Camera.main.transform.position.y - GameController.WindowMidHeight;
    }

    private void FixedUpdate()
    {
        PlayerPhysics player = SceneController.FindStageObject("PlayerPhysics") as PlayerPhysics;
        GoalSign GoalPost = SceneController.FindStageObject("GoalSign") as GoalSign;

        if (CameraAction == 0)
        {
            CameraMinimumX = 0f;
            CameraMinimumY = 0f;
            CameraMaximumX = LevelController.CurrentLevel.Width;
            CameraMaximumY = LevelController.CurrentLevel.Height;
        }
        if (CameraAction == 2)
        {
            CameraMinimumX = GoalPost.XPosition - GameController.WindowMidWidth;
            CameraMinimumY = -GoalPost.YPosition - 160;
            CameraMaximumX = GoalPost.XPosition + GameController.WindowMidWidth;
            CameraMaximumY = -GoalPost.YPosition + 80;
        }
        if (CameraAction == 3)
        {
            CameraShiftX = CameraShiftY = 0f;
        }

        if (LevelController.CurrentLevel.Paused) return;

        if (LagTimer == 0f && CameraMode == 0)
        {
            if (CameraX < CameraMaximumX - GameController.WindowMidWidth && player.XPosition > CameraX + 8f - CameraShiftX - CameraShiftXSpeed)
            {
                CameraX += Mathf.Min(16f * Time.timeScale, player.XPosition - CameraX - 8f + CameraShiftXSpeed + CameraShiftX);
            }
            else if (CameraX > CameraMinimumX + GameController.WindowMidWidth && player.XPosition < CameraX - 8f - CameraShiftX - CameraShiftXSpeed)
            {
                CameraX += Mathf.Max(-16f * Time.timeScale, player.XPosition - CameraX + 8f + CameraShiftXSpeed + CameraShiftX);
            }
        }

        if (CameraMode == 0)
        {
            if (player.Ground)
            {
                if (CameraY > CameraMinimumY + GameController.WindowMidHeight && -player.YPosition < CameraY + CameraShiftY + CameraShiftYSpeed)
                {
                    CameraY += Mathf.Max(-Mathf.Max(3f, Mathf.Abs(player.GroundSpeed * Mathf.Sin(player.GroundAngle * Mathf.Deg2Rad))) - 1f, -player.YPosition - CameraY - CameraShiftY - CameraShiftYSpeed) * Time.timeScale;
                }
                else if (CameraY < CameraMaximumY - GameController.WindowMidHeight && -player.YPosition > CameraY + CameraShiftY + CameraShiftYSpeed)
                {
                    CameraY += Mathf.Min(Mathf.Max(3f, Mathf.Abs(player.GroundSpeed * Mathf.Sin(player.GroundAngle * Mathf.Deg2Rad) * 2f)) - 1f, -player.YPosition - CameraY - CameraShiftY - CameraShiftYSpeed) * Time.timeScale;
                }
            }
            else
            {
                if (CameraY > CameraMinimumY + GameController.WindowMidHeight && -player.YPosition < CameraY - 48f + CameraShiftY + CameraShiftYSpeed)
                {
                    CameraY += Mathf.Max(-16f * Time.timeScale, -player.YPosition - CameraY + 48f - CameraShiftY - CameraShiftYSpeed);
                }
            }
            if ((player.Landed || !player.Ground && -player.YPosition > CameraY + CameraShiftY + CameraShiftYSpeed + 24f) && CameraY < CameraMaximumY - GameController.WindowMidHeight)
            {
                CameraY += Mathf.Max(0f, Mathf.Min(16f * Time.timeScale, -player.YPosition - CameraY - CameraShiftY - 24f) - CameraShiftYSpeed);
            }

            if (player.PlayerAction == new ObjectState(player.Action02_StandUp) && LookUpTimer < 110f)
            {
                LookUpTimer += Time.timeScale;
            }

            if (LookUpTimer == 0f && CameraShiftYSpeed >= 2f)
            {
                CameraShiftYSpeed -= 2f * Time.timeScale;
            }
            if (LookUpTimer >= 110f)
            {
                CameraShiftYSpeed = Mathf.Min(88f, CameraShiftYSpeed + (2f * Time.timeScale));
            }

            if (player.PlayerAction != new ObjectState(player.Action02_StandUp))
            {
                LookUpTimer = 0f;
            }

            if (player.PlayerAction == new ObjectState(player.Action03_CrouchDown) && CrouchDownTimer < 110f)
            {
                CrouchDownTimer += Time.timeScale;
            }

            if (CrouchDownTimer == 0f && CameraShiftYSpeed <= -2f)
            {
                CameraShiftYSpeed += 2f * Time.timeScale;
            }
            if (CrouchDownTimer >= 110f)
            {
                CameraShiftYSpeed = Mathf.Max(-88f, CameraShiftYSpeed - (2f * Time.timeScale));
            }

            if (player.PlayerAction != new ObjectState(player.Action03_CrouchDown))
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
            LagTimer -= Time.timeScale;
        }
        else if (CameraMode > 0)
        {
            LagTimer = 0f;
        }

        if (CameraX < CameraMinimumX + GameController.WindowMidWidth)
        {
            CameraX = Mathf.Min(CameraX + (2f * Time.timeScale), CameraMinimumX + GameController.WindowMidWidth);
        }
        else if (CameraX > CameraMaximumX - GameController.WindowMidWidth)
        {
            CameraX = Mathf.Max(CameraX - (2f * Time.timeScale), CameraMaximumX - GameController.WindowMidWidth);
        }

        if (CameraY < CameraMinimumY + GameController.WindowMidHeight)
        {
            CameraY = Mathf.Min(CameraY + (2f * Time.timeScale), CameraMinimumY + GameController.WindowMidHeight);
        }
        else if (CameraY > CameraMaximumY - GameController.WindowMidHeight)
        {
            CameraY = Mathf.Max(CameraY - (2f * Time.timeScale), CameraMaximumY - GameController.WindowMidHeight);
        }

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
        Camera.main.transform.position = new Vector3()
        {
            x = Mathf.Clamp(CameraX, GameController.WindowMidWidth, SceneController.CurrentScene.Width - GameController.WindowMidWidth),
            y = -Mathf.Clamp(CameraY, GameController.WindowMidHeight, SceneController.CurrentScene.Height - GameController.WindowMidHeight),
            z = -10f
        };

        GameController.XLeftFrame = Camera.main.transform.position.x - GameController.WindowMidWidth;
        GameController.XRightFrame = Camera.main.transform.position.x + GameController.WindowMidWidth;
        GameController.YTopFrame = Camera.main.transform.position.y + GameController.WindowMidHeight;
        GameController.YBottomFrame = Camera.main.transform.position.y - GameController.WindowMidHeight;
    }
}
