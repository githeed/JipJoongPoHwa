using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorEvent : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        IAnimatorInterface inter = animator.GetComponent<IAnimatorInterface>();
        if(inter == null)
        {
            inter = animator.GetComponentInParent<IAnimatorInterface>();
        }
        inter.OnStateEnter(animator, stateInfo, layerIndex);
    }

    
}
