using UnityEngine;
using System.IO;
using System;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public static int Frame;
    private float FrameTimer;

    public int Width = 426;
    public int Height = 240;

    public static int CurrentSlot = 1;
    public static int GameCharacter = 0;
    public static int Lives = 3;
    public static int Score = 0;
    private const string DATA_SEPARATOR = "#";

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(gameObject);

        SetGameSpeed(1f);
    }

    private void Start()
    {
        if (!File.Exists(Application.dataPath + string.Format("/save{0}.sav", CurrentSlot)))
        {
            string[] dataContent = new string[]
            {
                "NULL",
                //Score.ToString(),
                GameCharacter.ToString(),
                (Lives >= 1) ? Lives.ToString() : "3",
            };
            string dataTXT = string.Join(DATA_SEPARATOR, dataContent);
            File.WriteAllText(Application.dataPath + string.Format("/save{0}.sav", CurrentSlot), dataTXT);
        }
        else
        {
            if (CurrentSlot >= 1)
            {
                string dataTXT = File.ReadAllText(Application.dataPath + string.Format("/save{0}.sav", CurrentSlot));
                string[] dataContent = dataTXT.Split(new[] { DATA_SEPARATOR }, StringSplitOptions.None);

                //Score = int.Parse(dataContent[0]);
                GameCharacter = int.Parse(dataContent[1]);

                if (Lives > 0)
                {
                    Lives = int.Parse(dataContent[2]);
                }
                else
                {
                    Lives = 3;
                }
            }
        }
    }

    private void OnApplicationQuit()
    {
        if (CurrentSlot >= 1)
        {
            string[] dataContent = new string[]
            {
                "NULL",
                //Score.ToString(),
                GameCharacter.ToString(),
                (Lives >= 1) ? Lives.ToString() : "3",
            };
            string dataTXT = string.Join(DATA_SEPARATOR, dataContent);
            File.WriteAllText(Application.dataPath + string.Format("/save{0}.sav", CurrentSlot), dataTXT);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SetGameSpeed(1f);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            SetGameSpeed(0.25f);
        }
    }

    private void FixedUpdate()
    {
        Score = Mathf.Clamp(Score, 0, 9999999);
        Lives = Mathf.Clamp(Lives, 0, 99);

        FrameTimer -= Time.timeScale;

        if (FrameTimer > 0f)
        {
            Frame = 0;
        }
        else
        {
            Frame = 1;
            FrameTimer = 1f;
        }
    }

    public static void SetGameSpeed(float speed)
    {
        Time.timeScale = speed;
        Time.fixedDeltaTime = 1f / 60f * Time.timeScale;
    }
}
