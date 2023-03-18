using UnityEngine;
using System.Collections;

public class Explosion : BaseObject
{
    public override void ObjectCreated()
    {
        base.ObjectCreated();
        animator.Play("Explosion", -1, 0f);
    }

    private void FixedUpdate()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            SceneController.DestroyStageObject(this);
        }
    }
}
