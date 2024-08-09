using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnimatorEvent : StateMachineBehaviour
{
    IAnimatorInterface inter;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        inter = animator.GetComponent<IAnimatorInterface>();
        if(inter == null)
        {
            inter = animator.GetComponentInParent<IAnimatorInterface>();
        }
        inter.OnStateEnter(animator, stateInfo, layerIndex);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        inter = animator.GetComponent<IAnimatorInterface>();
        if (inter == null)
        {
            inter = animator.GetComponentInParent<IAnimatorInterface>();
        }
        inter.OnStateExit(animator, stateInfo, layerIndex);
    }

}
