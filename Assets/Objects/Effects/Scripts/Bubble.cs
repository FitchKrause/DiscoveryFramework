﻿using UnityEngine;

public class Bubble : BaseObject
{
    [Header("Bubble Values")]
    public float AlterableValueB;
    public AudioClip Sound_Breathe;

    public enum Bubble_Sizes
    {
        Small,
        Medium,
        Large
    }
    public Bubble_Sizes BubbleSize;

    private HitBox rect;
    private PlayerPhysics player;

    private new void Start()
    {
        player = FindObjectOfType<PlayerPhysics>();

        base.Start();

        rect.WidthRadius = 16f;
        rect.HeightRadius = 16f;
    }

    private void FixedUpdate()
    {
        XPosition += (Mathf.Cos(AlterableValueB * Mathf.Deg2Rad) / 3f) * Time.timeScale;
        AlterableValueB += 3 * Time.timeScale;
        YSpeed = 1f;

        ProcessMovement();

        rect.XPosition = XPosition;
        rect.YPosition = YPosition;

        switch (BubbleSize)
        {
            case Bubble_Sizes.Small:
                if (!GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("SmallBubble"))
                {
                    GetComponent<Animator>().Play("SmallBubble");
                }
                if (YPosition >= LevelController.CurrentLevel.WaterLevel)
                {
                    SceneController.DestroyStageObject(this);
                }
                break;
            case Bubble_Sizes.Medium:
                if (!GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("MediumBubble"))
                {
                    GetComponent<Animator>().Play("MediumBubble");
                }
                if (YPosition >= LevelController.CurrentLevel.WaterLevel - 8f)
                {
                    SceneController.DestroyStageObject(this);
                }
                break;
            case Bubble_Sizes.Large:
                if (!GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("LargeBubble") &&
                    !GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("LargeBubble_Loop"))
                {
                    GetComponent<Animator>().Play("LargeBubble");
                }
                if (YPosition >= LevelController.CurrentLevel.WaterLevel - 19f)
                {
                    SceneController.DestroyStageObject(this);
                }
                break;
        }

        if (HitBox.AABB(rect, player.Rect) && player.Shield != 4 &&
            GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("LargeBubble_Loop"))
        {
            AudioController.PlaySFX(Sound_Breathe);
            player.XSpeed = 0f;
            player.YSpeed = 0f;
            player.PlayerAction = player.Action07_Breathe;
            player.Air = 0;
            SceneController.DestroyStageObject(this);
        }
    }
}
