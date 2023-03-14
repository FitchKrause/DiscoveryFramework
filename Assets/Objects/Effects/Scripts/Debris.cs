using UnityEngine;
using System.Collections;

public class Debris : BaseObject
{
    private void FixedUpdate()
    {
        ProcessMovement();

        YSpeed -= 0.4f;

        if (YPosition < SceneController.YBottomFrame - 16f)
        {
            LevelController.DestroyStageObject(this);
        }
    }
}
