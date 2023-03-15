using UnityEngine;
using System.Collections;

public class Splash : BaseObject
{
    public override void ObjectCreated()
    {
        base.ObjectCreated();
        animator.Play("Splash", -1, 0f);
    }

    private void FixedUpdate()
    {
        YPosition = LevelController.CurrentLevel.WaterLevelApparent;

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            SceneController.DestroyStageObject(this);
        }
    }
}
