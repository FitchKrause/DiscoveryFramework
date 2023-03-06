using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour
{
    public Digit[] GUI_Rings = new Digit[3];
    public Digit[] GUI_FPS = new Digit[4];

    private int FPS;
    private int FPS_Timer;

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
        GUI_Rings[2].SetValue(StageController.CurrentStage.Rings / 100);
        GUI_Rings[1].SetValue(StageController.CurrentStage.Rings / 10 % 10);
        GUI_Rings[0].SetValue(StageController.CurrentStage.Rings % 10);

        GUI_Rings[2].GetComponent<SpriteRenderer>().enabled = StageController.CurrentStage.Rings >= 100;
        GUI_Rings[1].GetComponent<SpriteRenderer>().enabled = StageController.CurrentStage.Rings >= 10;
        GUI_Rings[0].GetComponent<SpriteRenderer>().enabled = true;

        FPS_Timer++;

        GUI_FPS[3].SetValue(FPS / 1000);
        GUI_FPS[2].SetValue(FPS / 100 % 10);
        GUI_FPS[1].SetValue(FPS / 10 % 10);
        GUI_FPS[0].SetValue(FPS % 10);

        GUI_FPS[3].GetComponent<SpriteRenderer>().enabled = FPS >= 1000;
        GUI_FPS[2].GetComponent<SpriteRenderer>().enabled = FPS >= 100;
        GUI_FPS[1].GetComponent<SpriteRenderer>().enabled = FPS >= 10;
        GUI_FPS[0].GetComponent<SpriteRenderer>().enabled = true;
    }

    private void LateUpdate()
    {
        transform.position = new Vector3(PixelCamera.XLeftFrame, PixelCamera.YTopFrame, 0f);
    }
}
