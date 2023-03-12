using UnityEngine;
using System.Collections;

public class LayerSwitcher : MonoBehaviour
{
    public enum SwitcherType_Enum
    {
        LayerALow,
        LayerAHigh,
        LayerBLow,
        LayerBHigh,
    }

    public SwitcherType_Enum SwitcherType;

    public bool GroundedSwitcher;

    private PlayerPhysics player;

    private void Start()
    {
        player = FindObjectOfType<PlayerPhysics>();
    }

    private void FixedUpdate()
    {
        if (GroundedSwitcher && !player.Ground) return;

        if (player.XPosition > transform.position.x - (8f * transform.localScale.x) &&
            player.XPosition < transform.position.x + (8f * transform.localScale.x) &&
            player.YPosition > transform.position.y - (8f * transform.localScale.y) &&
            player.YPosition < transform.position.y + (8f * transform.localScale.y))
        {
            switch (SwitcherType)
            {
                case SwitcherType_Enum.LayerALow:
                    player.CollisionLayer = 8;
                    player.render.sortingLayerName = "Low";
                    break;
                case SwitcherType_Enum.LayerAHigh:
                    player.CollisionLayer = 8;
                    player.render.sortingLayerName = "High";
                    break;
                case SwitcherType_Enum.LayerBLow:
                    player.CollisionLayer = 9;
                    player.render.sortingLayerName = "Low";
                    break;
                case SwitcherType_Enum.LayerBHigh:
                    player.CollisionLayer = 9;
                    player.render.sortingLayerName = "High";
                    break;
            }
        }
    }
}
