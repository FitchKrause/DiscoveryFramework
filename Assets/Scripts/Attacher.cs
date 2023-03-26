using UnityEngine;
using System.Collections;

public class Attacher : MonoBehaviour
{
    public bool Attached;
    public MovingPlatform LinkedPlatform;

    private BaseObject objRef;

    private void Start()
    {
        objRef = GetComponent<BaseObject>();
    }

    private void FixedUpdate()
    {
        PlayerPhysics player = SceneController.FindStageObject("PlayerPhysics") as PlayerPhysics;

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

    private void LateUpdate()
    {
        objRef.transform.position = new Vector3(Mathf.Floor(objRef.XPosition), Mathf.Floor(objRef.YPosition), objRef.transform.position.z);
    }
}
