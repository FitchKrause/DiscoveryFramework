using UnityEngine;
using System.Collections;

public class Trail : BaseObject
{
    public override void ObjectCreated()
    {
        base.ObjectCreated();
        animator.Play("MonitorIcon_Trail", -1, 0f);
    }

    private void FixedUpdate()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            StageController.DestroyStageObject(this);
        }
    }
}
