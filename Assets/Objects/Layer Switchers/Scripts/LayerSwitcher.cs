using UnityEngine;
using System.Collections;

public class LayerSwitcher : MonoBehaviour
{
    public enum LayerList
    {
        LayerA = 1 << 8,
        LayerB = 1 << 9
    }

    public enum PriorityList
    {
        Low,
        High
    }

    public LayerList SwitchToLayer;
    public PriorityList SwitchPriorityTo;

    public bool GroundedSwitcher;

    private void FixedUpdate()
    {
        PlayerPhysics player = SceneController.FindStageObject("PlayerPhysics") as PlayerPhysics;

        if (GroundedSwitcher && !player.Ground) return;

        if (player.XPosition > transform.position.x - (8f * transform.localScale.x) &&
            player.XPosition < transform.position.x + (8f * transform.localScale.x) &&
            player.YPosition > transform.position.y - (8f * transform.localScale.y) &&
            player.YPosition < transform.position.y + (8f * transform.localScale.y))
        {
            player.CollisionLayer = (int)SwitchToLayer;
            player.render.sortingLayerName = SwitchPriorityTo.ToString();
        }
    }
}
