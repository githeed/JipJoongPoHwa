using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAnimatorInterface
{
    public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex);
}
