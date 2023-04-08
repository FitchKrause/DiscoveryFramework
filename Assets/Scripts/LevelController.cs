using UnityEngine;
using System.Collections;

public class LevelController : SceneController
{
    public static LevelController CurrentLevel;

    public static float LevelTimer;
    public static float GlobalTimer;

    public static bool AllowTime;
    public static bool Boss;
    public static bool Clear;

    public static bool CheckPoint;
    public static float CheckPointX;
    public static float CheckPointY;
    public static float CheckPointLevelTime;
    public static float CheckPointGameTime;

    [HideInInspector] public bool Water;
    [HideInInspector] public float WaterLevel;
    [HideInInspector] public float WaterLevelApparent;

    [HideInInspector] public float GameTimer;
    [HideInInspector] public int Milliseconds;
    [HideInInspector] public int Seconds;
    [HideInInspector] public int Minutes;

    [HideInInspector] public int Rings;

    private static float CreationStrength;
    private static int CreationDirection = 1;
    private static float CreationAngle = 101.25f;

    private new void Awake()
    {
        AllowTime = AllowPause = true;

        base.Awake();

        CurrentLevel = this;
    }

    private new void Start()
    {
        CreationAngle = 101.25f;

        base.Start();
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();

        Rings = Mathf.Clamp(Rings, 0, 999);

        if (!Paused)
        {
            LevelTimer += GameController.Frame;
            GlobalTimer += GameController.Frame;

            if (AllowTime)
            {
                GameTimer += 1000f / 60f * Time.timeScale;

                Milliseconds = Mathf.FloorToInt(GameTimer / 10) % 100;
                Seconds = Mathf.FloorToInt(GameTimer / 1000) % 60;
                Minutes = Mathf.FloorToInt(GameTimer / 60000);
            }
        }
    }

    public static void RingLoss(int ringsToCreate, float creationX, float creationY)
    {
        CreationAngle = 101.25f;
        CreationDirection = 1;
        CreationStrength = 4f;

        for (int i = 0; i < 32; i++)
        {
            if (ringsToCreate > 0)
            {
                Ring movingRing = CreateStageObject("Moving Ring", creationX, creationY) as Ring;
                movingRing.MovementActivated = true;
                movingRing.XPosition = creationX;
                movingRing.YPosition = creationY;
                movingRing.XSpeed = Mathf.Cos(CreationAngle * Mathf.Deg2Rad) * CreationStrength * CreationDirection;
                movingRing.YSpeed = Mathf.Sin(CreationAngle * Mathf.Deg2Rad) * CreationStrength;
                movingRing.transform.position = new Vector3(movingRing.XPosition, movingRing.YPosition, 0f);
                CreationDirection *= -1;
                CreationAngle += 22.5f + Mathf.Max(0, CreationDirection);
                ringsToCreate--;
            }
            if (i == 15)
            {
                CreationAngle = 101.25f;
                CreationDirection = 1;
                CreationStrength = 2f;
            }
        }
    }
}
