using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIItemHidden : StateMachineBehaviour
{

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObjectVariable obj = BlackboardController.Instance.GetValue("UIItemHidden") as GameObjectVariable;
        obj.m_Value = animator.gameObject;
        BlackboardController.Instance.Invoke("UIItemHidden");
    }
}
