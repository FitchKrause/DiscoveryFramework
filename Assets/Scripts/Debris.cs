using UnityEngine;
using System.Collections;

public class Debris : BaseObject
{
    private void FixedUpdate()
    {
        ProcessMovement();

        YSpeed -= 0.4f;

        if (YPosition < PixelCamera.YBottomFrame - 16f)
        {
            StageController.DestroyStageObject(this);
        }
    }
}
