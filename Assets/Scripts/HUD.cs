using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour
{
    public Digit[] GUI_Score = new Digit[7];
    public Digit[] GUI_Time = new Digit[6];
    public Digit[] GUI_Rings = new Digit[3];
    public Digit[] GUI_Lives = new Digit[2];
    public Digit[] GUI_FPS = new Digit[4];

    public GameObject GUI_Pause;
    public GameObject GUI_PauseCursor;
    public int PauseOption;

    private int FPS;
    private int FPS_Timer;

    private InputManager input;

    private void Start()
    {
        input = InputManager.instance;
    }

    private void Update()
    {
        if (FPS_Timer >= 60)
        {
            FPS = (int)(1f / Time.unscaledDeltaTime);
            FPS_Timer = 0;
        }
    }

    private void FixedUpdate()
    {
        GUI_Score[6].SetValue(GameController.Score / 1000000);
        GUI_Score[5].SetValue(GameController.Score / 100000 % 10);
        GUI_Score[4].SetValue(GameController.Score / 10000 % 10);
        GUI_Score[3].SetValue(GameController.Score / 1000 % 10);
        GUI_Score[2].SetValue(GameController.Score / 100 % 10);
        GUI_Score[1].SetValue(GameController.Score / 10 % 10);
        GUI_Score[0].SetValue(GameController.Score % 10);

        GUI_Score[6].GetComponent<SpriteRenderer>().enabled = GameController.Score >= 1000000;
        GUI_Score[5].GetComponent<SpriteRenderer>().enabled = GameController.Score >= 100000;
        GUI_Score[4].GetComponent<SpriteRenderer>().enabled = GameController.Score >= 10000;
        GUI_Score[3].GetComponent<SpriteRenderer>().enabled = GameController.Score >= 1000;
        GUI_Score[2].GetComponent<SpriteRenderer>().enabled = GameController.Score >= 100;
        GUI_Score[1].GetComponent<SpriteRenderer>().enabled = GameController.Score >= 10;
        GUI_Score[0].GetComponent<SpriteRenderer>().enabled = true;

        GUI_Time[5].SetValue(LevelController.CurrentLevel.Minutes / 10);
        GUI_Time[4].SetValue(LevelController.CurrentLevel.Minutes % 10);
        GUI_Time[3].SetValue(LevelController.CurrentLevel.Seconds / 10);
        GUI_Time[2].SetValue(LevelController.CurrentLevel.Seconds % 10);
        GUI_Time[1].SetValue(LevelController.CurrentLevel.Milliseconds / 10);
        GUI_Time[0].SetValue(LevelController.CurrentLevel.Milliseconds % 10);

        GUI_Time[5].GetComponent<SpriteRenderer>().enabled = LevelController.CurrentLevel.Minutes >= 10;

        GUI_Rings[2].SetValue(LevelController.CurrentLevel.Rings / 100);
        GUI_Rings[1].SetValue(LevelController.CurrentLevel.Rings / 10 % 10);
        GUI_Rings[0].SetValue(LevelController.CurrentLevel.Rings % 10);

        GUI_Rings[2].GetComponent<SpriteRenderer>().enabled = LevelController.CurrentLevel.Rings >= 100;
        GUI_Rings[1].GetComponent<SpriteRenderer>().enabled = LevelController.CurrentLevel.Rings >= 10;
        GUI_Rings[0].GetComponent<SpriteRenderer>().enabled = true;

        GUI_Lives[1].SetValue(GameController.Lives / 10);
        GUI_Lives[0].SetValue(GameController.Lives % 10);

        GUI_Lives[1].GetComponent<SpriteRenderer>().enabled = GameController.Lives >= 10;
        GUI_Lives[0].GetComponent<SpriteRenderer>().enabled = true;

        FPS_Timer++;

        GUI_FPS[3].SetValue(FPS / 1000);
        GUI_FPS[2].SetValue(FPS / 100 % 10);
        GUI_FPS[1].SetValue(FPS / 10 % 10);
        GUI_FPS[0].SetValue(FPS % 10);

        GUI_FPS[3].GetComponent<SpriteRenderer>().enabled = FPS >= 1000;
        GUI_FPS[2].GetComponent<SpriteRenderer>().enabled = FPS >= 100;
        GUI_FPS[1].GetComponent<SpriteRenderer>().enabled = FPS >= 10;
        GUI_FPS[0].GetComponent<SpriteRenderer>().enabled = true;

        if (LevelController.Paused ? input.KeyActionBPressed : input.KeyStartPressed)
        {
            PauseOption = 0;
            LevelController.PauseTrigger = true;
        }

        if (LevelController.Paused)
        {
            if (input.KeyDownPressed)
            {
                PauseOption++;
                if (PauseOption > 2)
                {
                    PauseOption = 0;
                }
            }
            if (input.KeyUpPressed)
            {
                PauseOption--;
                if (PauseOption < 0)
                {
                    PauseOption = 2;
                }
            }

            if (input.KeyStartPressed || input.KeyActionAPressed)
            {
                if (PauseOption == 0)
                {
                    LevelController.PauseTrigger = true;
                }
                if (PauseOption == 1 && !GameController.Preload)
                {
                    GameController.instance.LoadLevel(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
                }
                if (PauseOption == 2)
                {
                    Application.Quit();
                }
            }
        }

        GUI_Pause.SetActive(LevelController.Paused);
        Vector3 vector = GUI_PauseCursor.transform.position;
        vector.y = GUI_Pause.transform.position.y + (-16f * PauseOption);
        GUI_PauseCursor.transform.position = vector;
    }

    private void LateUpdate()
    {
        transform.position = new Vector3(SceneController.XLeftFrame, SceneController.YTopFrame, 0f);
    }
}
