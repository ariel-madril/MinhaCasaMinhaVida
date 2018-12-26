using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIItemShown : StateMachineBehaviour
{
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObjectVariable obj = BlackboardController.Instance.GetValue("UIItemShown") as GameObjectVariable;
        obj.m_Value = animator.gameObject;
        BlackboardController.Instance.Invoke("UIItemShown");
    }
}
