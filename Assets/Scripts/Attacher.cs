using UnityEngine;
using System.Collections;

public class Attacher : MonoBehaviour
{
    public bool Attached;
    public MovingPlatform LinkedPlatform;

    private PlayerPhysics player;
    private BaseObject objRef;

    private void Start()
    {
        player = FindObjectOfType<PlayerPhysics>();
        objRef = GetComponent<BaseObject>();
    }

    private void FixedUpdate()
    {
        if (!objRef.enabled) return;

        if (Attached && LinkedPlatform != null)
        {
            objRef.XPosition -= LinkedPlatform.DifferenceX;
            objRef.YPosition -= LinkedPlatform.DifferenceY;

            if (player.Ground && player.ColliderFloor == objRef.ColliderBody)
            {
                player.XPosition -= LinkedPlatform.DifferenceX;
                player.YPosition -= LinkedPlatform.DifferenceY;

                CameraController.CameraX -= LinkedPlatform.DifferenceX;
                CameraController.CameraY -= LinkedPlatform.DifferenceY;
            }
        }
    }
}
