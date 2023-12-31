﻿using UnityEngine;
using System.Collections;

public class LampPost : BaseObject
{
    public bool Flag0;
    public bool Flag1;
    public bool Flag2;
    public float Age;
    public AudioClip Sound_LampPost;

    public float BaseX;
    public float BaseY;
    public float Range;
    public float Delta;

    private GameObject Sphere;
    private HitBox Rect;

    private new void Start()
    {
        Sphere = transform.GetChild(0).gameObject;

        base.Start();

        if (XPosition == LevelController.CheckPointX && YPosition == (LevelController.CheckPointY - 13f))
        {
            Flag0 = Flag1 = true;
            Age = 49f;
        }

        BaseX = XPosition;
        BaseY = YPosition + 46f;

        Sphere.transform.SetParent(null);
        Vector2 vector = new Vector2(BaseX + (Range * Mathf.Sin(Delta * Mathf.Deg2Rad)), BaseY + (Range * Mathf.Cos(Delta * Mathf.Deg2Rad)));
        Sphere.transform.position = vector;

        Rect.XPosition = XPosition;
        Rect.YPosition = YPosition + 24f;
        Rect.WidthRadius = 8f;
        Rect.HeightRadius = 24f;
    }

    private void FixedUpdate()
    {
        PlayerPhysics player = SceneController.FindStageObject("PlayerPhysics") as PlayerPhysics;

        if (!Flag1 && HitBox.AABB(Rect, player.Rect))
        {
            Flag0 = true;
            AudioController.PlaySFX(Sound_LampPost);
        }

        Vector2 vector = new Vector2(BaseX + (Range * Mathf.Sin(Delta * Mathf.Deg2Rad)), BaseY + (Range * Mathf.Cos(Delta * Mathf.Deg2Rad)));
        Sphere.transform.position = vector;
        
        Range = 10f;

        if (Flag0)
        {
            Flag2 = true;
            Flag0 = false;
            Flag1 = true;
            LevelController.CheckPointX = XPosition;
            LevelController.CheckPointY = YPosition + 13f;
            LevelController.CheckPointLevelTime = LevelController.LevelTimer;
            LevelController.CheckPointGameTime = LevelController.CurrentLevel.GameTimer;
            LevelController.CheckPoint = true;
        }

        if (Flag2)
        {
            if (Age < 49f)
            {
                Delta += 15f * Time.timeScale;
                Age += Time.timeScale;
            }
            else if (Age == 49f)
            {
                Sphere.GetComponent<Animator>().Play("Active");
                Delta = 0f;
                Age = 50f;
            }
        }
    }
}
