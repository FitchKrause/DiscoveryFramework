using UnityEngine;
using System.Collections;

public class Spike : BaseObject
{
    public enum Spike_Directions
    {
        Up,
        Down,
        Left,
        Right
    }

    [Header("Spike Settings")]
    public Spike_Directions SpikeDirection;

    private PlayerPhysics player;

    private new void Start()
    {
        player = FindObjectOfType<PlayerPhysics>();

        base.Start();
    }

    private void FixedUpdate()
    {
        if (player.InvincibilityTimer <= 0f)
        {
            switch (SpikeDirection)
            {
                case Spike_Directions.Up:
                    if (player.YSpeed <= 0f && player.ColliderFloor == ColliderBody)
                    {
                        player.Hurt = 1;
                    }
                    break;
                case Spike_Directions.Down:
                    if (player.YSpeed >= 0f && player.ColliderCeiling == ColliderBody)
                    {
                        player.Hurt = 1;
                    }
                    break;
                case Spike_Directions.Left:
                    if ((player.Ground ? player.GroundSpeed : player.XSpeed) >= 0f && player.ColliderWallRight == ColliderBody)
                    {
                        player.Hurt = 1;
                    }
                    break;
                case Spike_Directions.Right:
                    if ((player.Ground ? player.GroundSpeed : player.XSpeed) <= 0f && player.ColliderWallLeft == ColliderBody)
                    {
                        player.Hurt = 1;
                    }
                    break;
            }
        }
    }
}
