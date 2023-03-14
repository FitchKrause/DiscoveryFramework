using UnityEngine;
using System.Collections;

public class Dust : BaseObject
{
    public override void ObjectCreated()
    {
        base.ObjectCreated();
        animator.Play("Dust", -1, 0f);
    }

    private void FixedUpdate()
    {
        if (LevelController.GlobalTimer % 2 == 0)
        {
            YPosition += 1f;
        }

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            LevelController.DestroyStageObject(this);
        }
    }
}
