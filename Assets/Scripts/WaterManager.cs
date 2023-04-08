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
        LevelController.CurrentLevel.Water = Water;
        LevelController.CurrentLevel.WaterLevel = WaterLevel;
    }

    private void FixedUpdate()
    {
        if (!Water) return;

        WaterLevelApparent = WaterLevel + 3f * Mathf.Cos((LevelController.LevelTimer * 1.5f) * Mathf.Deg2Rad);
        LevelController.CurrentLevel.WaterLevelApparent = WaterLevelApparent;
    }

    private void LateUpdate()
    {
        if (!Water) return;

        Vector2 vector = new Vector2(GameController.XLeftFrame - (GameController.XLeftFrame % 64), WaterLevelApparent);
        Vector2 vector2 = new Vector2(GameController.XLeftFrame, Mathf.Min(GameController.YTopFrame, WaterLevelApparent));

        WaterHorizonObject.transform.position = vector;
        WaterObject.transform.position = vector2;
    }
}
