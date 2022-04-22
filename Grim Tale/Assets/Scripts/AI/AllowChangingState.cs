using UnityEngine;

namespace AI
{
    public class AllowChangingState : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var enemy = animator.GetComponent<Enemy>();
            if (enemy)
            {
                enemy.StateBlocked = false;
                return;
            }

            var ally = animator.GetComponent<Ally>();
            if (ally)
            {
                ally.StateBlocked = false;
            }
        }
    }
}
