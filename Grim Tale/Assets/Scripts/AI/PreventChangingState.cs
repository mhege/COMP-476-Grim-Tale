using UnityEngine;

namespace AI
{
    public class PreventChangingState : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var enemy = animator.GetComponent<Enemy>();
            if (enemy)
            {
                enemy.StateBlocked = true;
                return;
            }

            var ally = animator.GetComponent<Ally>();
            if (ally)
            {
                ally.StateBlocked = true;
            }
        }
    }
}
