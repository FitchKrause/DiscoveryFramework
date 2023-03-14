using UnityEngine;
using System.Collections;

public class Sparkle : BaseObject
{
    public override void ObjectCreated()
    {
        base.ObjectCreated();
        animator.Play("Sparkle", -1, 0f);
    }

    private void FixedUpdate()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            LevelController.DestroyStageObject(this);
        }
    }
}
