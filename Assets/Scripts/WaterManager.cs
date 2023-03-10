using UnityEngine;
using System.Collections;

public class WaterManager : MonoBehaviour
{
    public bool Water = true;

    public GameObject WaterMark;
    public GameObject WaterObject;
    public GameObject WaterHorizonObject;

    public float WaterLevel;
    public float WaterLevelApparent;

    private void Start()
    {
        WaterLevel = WaterMark.transform.position.y;
        StageController.CurrentStage.Water = Water;
        StageController.CurrentStage.WaterLevel = WaterLevel;

        float screenWidth = (PixelCamera.XRightFrame - PixelCamera.XLeftFrame);
        float screenHeight = (PixelCamera.YTopFrame - PixelCamera.YBottomFrame);

        WaterObject.transform.localScale = new Vector3(screenWidth / 16f, screenHeight / 16f, 1f);
    }

    private void FixedUpdate()
    {
        if (!Water) return;

        WaterLevelApparent = WaterLevel + 3f * Mathf.Cos((StageController.LevelTimer * 1.5f) * Mathf.Deg2Rad);
    }

    private void LateUpdate()
    {
        if (!Water) return;

        Vector2 vector = new Vector2(PixelCamera.XLeftFrame - (PixelCamera.XLeftFrame % 64), WaterLevelApparent);
        Vector2 vector2 = new Vector2(PixelCamera.XLeftFrame, Mathf.Min(PixelCamera.YTopFrame, WaterLevelApparent));

        WaterHorizonObject.transform.position = vector;
        WaterObject.transform.position = vector2;

        Splash splash = StageController.FindStageObject("Water Splash") as Splash;

        if (splash != null)
        {
            splash.transform.position = new Vector2(splash.transform.position.x, WaterLevelApparent);
        }
    }
}
