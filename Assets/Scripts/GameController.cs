using System;
using System.IO;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private static GameController instance;

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
    }

    private void Start()
    {
        if (!File.Exists(Application.dataPath + string.Format("/save{0}.txt", CurrentSlot)))
        {
            string[] dataContent = new string[]
            {
                "NULL",
                //Score.ToString(),
                GameCharacter.ToString(),
                (Lives >= 1) ? Lives.ToString() : "3",
            };
            string dataTXT = string.Join(DATA_SEPARATOR, dataContent);
            File.WriteAllText(Application.dataPath + string.Format("/save{0}.txt", CurrentSlot), dataTXT);
        }
        else
        {
            if (CurrentSlot >= 1)
            {
                string dataTXT = File.ReadAllText(Application.dataPath + string.Format("/save{0}.txt", CurrentSlot));
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
            File.WriteAllText(Application.dataPath + string.Format("/save{0}.txt", CurrentSlot), dataTXT);
        }
    }

    private void FixedUpdate()
    {
        Score = Mathf.Clamp(Score, 0, 9999999);
        Lives = Mathf.Clamp(Lives, 0, 99);
    }
}
