using AI.Enemies;
using UnityEngine;

namespace AI
{
    public class StandUp : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<MutantCharger>()?.StandUp();
        }
    }
}
