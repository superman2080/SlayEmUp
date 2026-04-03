using UnityEngine;
using System.Linq;

public class AttackStateMachine : StateMachineBehaviour
{
    private static readonly int speedParameter = Animator.StringToHash("AttackSpeed");
    private float originalSpeed;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        originalSpeed = animator.speed;

        // 0.3f
        float animTime = animator.runtimeAnimatorController.animationClips.First(c => c.name == "ATTACK").length;
        float attackSpeed = 1f / animator.GetFloat(speedParameter);
        animator.speed = animTime / attackSpeed;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.speed = originalSpeed;
    }
}
