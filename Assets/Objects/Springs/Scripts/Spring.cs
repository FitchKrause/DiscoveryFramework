using UnityEngine;
using System.Collections;

public class Spring : BaseObject
{
    public enum Spring_Directions
    {
        Up,
        Down,
        Left,
        Right,
        UpLeft,
        UpRight
    }

    public enum Spring_Colors
    {
        Red,
        Yellow
    }

    [Header("Spring Settings")]
    public Spring_Directions SpringDirection;
    public Spring_Colors SpringColor;
    public AudioClip Sound_Spring;
    
    private bool Flag;
    private PlayerPhysics player;

    private new void Start()
    {
        player = FindObjectOfType<PlayerPhysics>();

        base.Start();
    }

    private void FixedUpdate()
    {
        switch (SpringColor)
        {
            case Spring_Colors.Red:
                switch (SpringDirection)
                {
                    case Spring_Directions.Up:
                        if (player.YSpeed <= 0f && player.ColliderFloor == ColliderBody)
                        {
                            Flag = true;
                            player.YSpeed = 16f;
                            player.Ground = false;
                            player.Action = 40;
                            player.CurrentAction = player.Action40_Springs;
                            player.JumpVariable = false;
                            SoundManager.PlaySFX(Sound_Spring);
                        }
                        break;
                    case Spring_Directions.Down:
                        if (player.YSpeed >= 0f && player.ColliderCeiling == ColliderBody)
                        {
                            Flag = true;
                            player.YSpeed = -16f;
                            player.Ground = false;
                            player.Action = 40;
                            player.CurrentAction = player.Action40_Springs;
                            SoundManager.PlaySFX(Sound_Spring);
                        }
                        break;
                    case Spring_Directions.Left:
                        if ((player.Ground ? player.GroundSpeed : player.XSpeed) >= 0f && player.ColliderWallRight == ColliderBody)
                        {
                            Flag = true;
                            if (player.Ground) player.GroundSpeed = -16f;
                            else player.XSpeed = -16f;
                            player.ControlLock = 16;
                            SoundManager.PlaySFX(Sound_Spring);
                        }
                        break;
                    case Spring_Directions.Right:
                        if ((player.Ground ? player.GroundSpeed : player.XSpeed) <= 0f && player.ColliderWallLeft == ColliderBody)
                        {
                            Flag = true;
                            if (player.Ground) player.GroundSpeed = 16f;
                            else player.XSpeed = 16f;
                            player.ControlLock = 16;
                            SoundManager.PlaySFX(Sound_Spring);
                        }
                        break;
                    case Spring_Directions.UpLeft:
                        if (player.YSpeed <= 0f && player.ColliderFloor == ColliderBody)
                        {
                            Flag = true;
                            player.XSpeed = -12f;
                            player.YSpeed = 12f;
                            player.Ground = false;
                            player.Action = 40;
                            player.CurrentAction = player.Action40_Springs;
                            player.JumpVariable = false;
                            SoundManager.PlaySFX(Sound_Spring);
                        }
                        break;
                    case Spring_Directions.UpRight:
                        if (player.YSpeed <= 0f && player.ColliderFloor == ColliderBody)
                        {
                            Flag = true;
                            player.XSpeed = 12f;
                            player.YSpeed = 12f;
                            player.Ground = false;
                            player.Action = 40;
                            player.CurrentAction = player.Action40_Springs;
                            player.JumpVariable = false;
                            SoundManager.PlaySFX(Sound_Spring);
                        }
                        break;
                }
                break;
            case Spring_Colors.Yellow:
                switch (SpringDirection)
                {
                    case Spring_Directions.Up:
                        if (player.YSpeed <= 0f && player.ColliderFloor == ColliderBody)
                        {
                            Flag = true;
                            player.YSpeed = 10f;
                            player.Ground = false;
                            player.Action = 40;
                            player.CurrentAction = player.Action40_Springs;
                            player.JumpVariable = false;
                            SoundManager.PlaySFX(Sound_Spring);
                        }
                        break;
                    case Spring_Directions.Down:
                        if (player.YSpeed >= 0f && player.ColliderCeiling == ColliderBody)
                        {
                            Flag = true;
                            player.YSpeed = -10f;
                            player.Ground = false;
                            player.Action = 40;
                            player.CurrentAction = player.Action40_Springs;
                            SoundManager.PlaySFX(Sound_Spring);
                        }
                        break;
                    case Spring_Directions.Left:
                        if ((player.Ground ? player.GroundSpeed : player.XSpeed) >= 0f && player.ColliderWallRight == ColliderBody)
                        {
                            Flag = true;
                            if (player.Ground) player.GroundSpeed = -10f;
                            else player.XSpeed = -10f;
                            player.ControlLock = 16;
                            SoundManager.PlaySFX(Sound_Spring);
                        }
                        break;
                    case Spring_Directions.Right:
                        if ((player.Ground ? player.GroundSpeed : player.XSpeed) <= 0f && player.ColliderWallLeft == ColliderBody)
                        {
                            Flag = true;
                            if (player.Ground) player.GroundSpeed = -10f;
                            else player.XSpeed = -10f;
                            player.ControlLock = 16;
                            SoundManager.PlaySFX(Sound_Spring);
                        }
                        break;
                    case Spring_Directions.UpLeft:
                        if (player.YSpeed <= 0f && player.ColliderFloor == ColliderBody)
                        {
                            Flag = true;
                            player.XSpeed = -8f;
                            player.YSpeed = 8f;
                            player.Ground = false;
                            player.Action = 40;
                            player.CurrentAction = player.Action40_Springs;
                            player.JumpVariable = false;
                            SoundManager.PlaySFX(Sound_Spring);
                        }
                        break;
                    case Spring_Directions.UpRight:
                        if (player.YSpeed <= 0f && player.ColliderFloor == ColliderBody)
                        {
                            Flag = true;
                            player.XSpeed = 8f;
                            player.YSpeed = 8f;
                            player.Ground = false;
                            player.Action = 40;
                            player.CurrentAction = player.Action40_Springs;
                            player.JumpVariable = false;
                            SoundManager.PlaySFX(Sound_Spring);
                        }
                        break;
                }
                break;
        }

        if (Flag)
        {
            GetComponent<Animator>().Play("Bounce", -1, 0f);
            Flag = false;
        }
    }
}
