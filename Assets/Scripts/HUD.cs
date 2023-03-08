using UnityEngine;
using UnityEngine.SceneManagement;
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

    private bool KeyStart;
    private bool KeyStartPressed;
    private bool KeyUp;
    private bool KeyUpPressed;
    private bool KeyDown;
    private bool KeyDownPressed;
    private bool KeyActionA;
    private bool KeyActionAPressed;
    private bool KeyActionB;
    private bool KeyActionBPressed;

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
        KeyStartPressed = Input.GetKey(KeyCode.Return) && !KeyStart;
        KeyStart = Input.GetKey(KeyCode.Return);

        KeyUpPressed = Input.GetKey(KeyCode.UpArrow) && !KeyUp;
        KeyUp = Input.GetKey(KeyCode.UpArrow);

        KeyDownPressed = Input.GetKey(KeyCode.DownArrow) && !KeyDown;
        KeyDown = Input.GetKey(KeyCode.DownArrow);

        KeyActionAPressed = Input.GetKey(KeyCode.Z) && !KeyActionA;
        KeyActionA = Input.GetKey(KeyCode.Z);

        KeyActionBPressed = Input.GetKey(KeyCode.X) && !KeyActionB;
        KeyActionB = Input.GetKey(KeyCode.X);

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

        GUI_Time[5].SetValue(StageController.CurrentStage.Minutes / 10);
        GUI_Time[4].SetValue(StageController.CurrentStage.Minutes % 10);
        GUI_Time[3].SetValue(StageController.CurrentStage.Seconds / 10);
        GUI_Time[2].SetValue(StageController.CurrentStage.Seconds % 10);
        GUI_Time[1].SetValue(StageController.CurrentStage.Milliseconds / 10);
        GUI_Time[0].SetValue(StageController.CurrentStage.Milliseconds % 10);

        GUI_Time[5].GetComponent<SpriteRenderer>().enabled = StageController.CurrentStage.Minutes >= 10;

        GUI_Rings[2].SetValue(StageController.CurrentStage.Rings / 100);
        GUI_Rings[1].SetValue(StageController.CurrentStage.Rings / 10 % 10);
        GUI_Rings[0].SetValue(StageController.CurrentStage.Rings % 10);

        GUI_Rings[2].GetComponent<SpriteRenderer>().enabled = StageController.CurrentStage.Rings >= 100;
        GUI_Rings[1].GetComponent<SpriteRenderer>().enabled = StageController.CurrentStage.Rings >= 10;
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

        if (StageController.Paused ? KeyActionBPressed : KeyStartPressed)
        {
            PauseOption = 0;
            StageController.PauseTrigger = true;
        }

        if (StageController.Paused)
        {
            if (KeyDownPressed)
            {
                PauseOption++;
                if (PauseOption > 2)
                {
                    PauseOption = 0;
                }
            }
            if (KeyUpPressed)
            {
                PauseOption--;
                if (PauseOption < 0)
                {
                    PauseOption = 2;
                }
            }

            if (KeyStartPressed || KeyActionAPressed)
            {
                if (PauseOption == 0)
                {
                    StageController.PauseTrigger = true;
                }
                if (PauseOption == 1)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
                if (PauseOption == 2)
                {
                    Application.Quit();
                }
            }
        }

        GUI_Pause.SetActive(StageController.Paused);
        Vector3 vector = GUI_PauseCursor.transform.position;
        vector.y = GUI_Pause.transform.position.y + (-16f * PauseOption);
        GUI_PauseCursor.transform.position = vector;
    }

    private void LateUpdate()
    {
        transform.position = new Vector3(PixelCamera.XLeftFrame, PixelCamera.YTopFrame, 0f);
    }
}
