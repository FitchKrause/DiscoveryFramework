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
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            StageController.DestroyStageObject(this);
        }
    }
}
